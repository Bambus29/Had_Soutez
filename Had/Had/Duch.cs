using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Had
{
    public class Duch
    {
        private List<Point> body = new List<Point>();
        private Point direction;
        private int gridWidth;
        private int gridHeight;
        private Random random = new Random();
        private double moveTimer = 0;
        private double moveInterval = 0.3;

        public bool IsAlive { get; set; } = true;

        public Duch(int startX, int startY, int length, int gridW, int gridH)
        {
            gridWidth = gridW;
            gridHeight = gridH;

            for (int i = 0; i < length; i++)
                body.Add(new Point(startX - i, startY));

            direction = GetValidDirection();
        }

        public void Update(GameTime gameTime)
        {
            moveTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (moveTimer >= moveInterval)
            {
                moveTimer = 0;
                Move();
            }
        }

        private void Move()
        {
            Point newHead = new Point(body[0].X + direction.X, body[0].Y + direction.Y);

            // Pokud by duch narazil na stěnu nebo své tělo, vyber náhodný platný směr
            if (!IsValid(newHead))
            {
                direction = GetValidDirection();
                newHead = new Point(body[0].X + direction.X, body[0].Y + direction.Y);

                // Pokud ani nový směr není validní (např. roh), zkuste až 10x
                int attempts = 0;
                while (!IsValid(newHead) && attempts < 10)
                {
                    direction = GetValidDirection();
                    newHead = new Point(body[0].X + direction.X, body[0].Y + direction.Y);
                    attempts++;
                }
            }

            body.Insert(0, newHead);
            body.RemoveAt(body.Count - 1);
        }

        private bool IsValid(Point p)
        {
            return p.X >= 0 && p.X < gridWidth && p.Y >= 0 && p.Y < gridHeight && !body.Contains(p);
        }

        private Point GetValidDirection()
        {
            List<Point> possibleDirections = new List<Point>();

            if (body[0].Y > 0 && !body.Contains(new Point(body[0].X, body[0].Y - 1))) possibleDirections.Add(new Point(0, -1));
            if (body[0].Y < gridHeight - 1 && !body.Contains(new Point(body[0].X, body[0].Y + 1))) possibleDirections.Add(new Point(0, 1));
            if (body[0].X > 0 && !body.Contains(new Point(body[0].X - 1, body[0].Y))) possibleDirections.Add(new Point(-1, 0));
            if (body[0].X < gridWidth - 1 && !body.Contains(new Point(body[0].X + 1, body[0].Y))) possibleDirections.Add(new Point(1, 0));

            if (possibleDirections.Count == 0)
                return new Point(0, 0); // uvnitř rohu, nemůže se pohnout

            return possibleDirections[random.Next(possibleDirections.Count)];
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, int cellSize, Color color, LayerType playerLayer)
        {
            if (playerLayer == LayerType.Shadow) return;

            foreach (var segment in body)
                spriteBatch.Draw(pixel, new Rectangle(segment.X * cellSize, segment.Y * cellSize, cellSize, cellSize), color);
        }

        public List<Point> Body => body;
        public Point Head => body[0];

        // Debug properties
        public double DebugMoveTimer => moveTimer;
        public double DebugMoveInterval => moveInterval;
        public Point DebugDirection => direction;
        public int DebugBodyCount => body.Count;
    }
}
