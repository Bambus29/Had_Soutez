using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Had
{
    public class Duch : IGameObject
    {
        public Point Position => Head;
        public LayerType Layer => LayerType.Main;

        private List<Point> body = new List<Point>();
        private Point direction;
        private int gridWidth;
        private int gridHeight;
        private Random random = new Random();
        private double moveTimer = 0;
        private double moveInterval = 0.3;

        // new: change direction every N steps
        private int stepsSinceDirectionChange = 0;
        private readonly int stepsBeforeChange = 5;

        public bool IsAlive { get; set; } = true;

        public Duch(int startX, int startY, int length, int gridW, int gridH)
        {
            gridWidth = gridW;
            gridHeight = gridH;

            for (int i = 0; i < length; i++)
                body.Add(new Point(startX - i, startY));

            direction = GetValidDirection();
            stepsSinceDirectionChange = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsAlive) return;
            if (body.Count == 0) { IsAlive = false; return; }

            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            if (delta <= 0) return;
            moveTimer += delta;

            if (moveTimer >= moveInterval)
            {
                moveTimer -= moveInterval;
                Move();
            }
        }

        private void Move()
        {
            if (body.Count == 0) return;

            // If we've moved enough steps, pick a new random valid direction
            if (stepsSinceDirectionChange >= stepsBeforeChange)
            {
                var newDir = GetValidDirection();
                // If GetValidDirection returned (0,0) or same direction and it's invalid, try a few times
                int attempts = 0;
                while ((newDir == new Point(0, 0) || newDir == direction) && attempts < 10)
                {
                    newDir = GetValidDirection();
                    attempts++;
                }

                // only assign if not (0,0)
                if (newDir != new Point(0, 0))
                {
                    direction = newDir;
                }
                stepsSinceDirectionChange = 0;
            }

            Point newHead = new Point(body[0].X + direction.X, body[0].Y + direction.Y);

            // If next step would be invalid, pick another valid direction immediately
            if (!IsValid(newHead))
            {
                // try up to 10 times to find a valid direction
                int attempts = 0;
                Point candidate;
                do
                {
                    candidate = GetValidDirection();
                    newHead = new Point(body[0].X + candidate.X, body[0].Y + candidate.Y);
                    attempts++;
                } while ((!IsValid(newHead) || candidate == new Point(0,0)) && attempts < 10);

                if (!IsValid(newHead))
                {
                    // no valid move found — stay in place (avoid inserting duplicate head)
                    return;
                }

                // accept candidate direction
                direction = candidate;
                stepsSinceDirectionChange = 0;
            }

            body.Insert(0, newHead);
            body.RemoveAt(body.Count - 1);

            // increment step counter after a successful move
            stepsSinceDirectionChange++;
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

        // Signatura odpovídá IGameObject
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, int cellSize, Color color)
        {
            foreach (var segment in body)
                spriteBatch.Draw(pixel, new Rectangle(segment.X * cellSize, segment.Y * cellSize, cellSize, cellSize), color);
        }

        public List<Point> Body => body;
        public Point Head => body.Count > 0 ? body[0] : new Point(-1, -1);

        // Debug properties
        public double DebugMoveTimer => moveTimer;
        public double DebugMoveInterval => moveInterval;
        public Point DebugDirection => direction;
        public int DebugBodyCount => body.Count;
    }
}
