using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Had
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;

    public class Snake
    {
        private List<Point> body = new List<Point>();
        private Point direction = new Point(1, 0); // začíná vpravo
        private double moveTimer = 0;
        private double moveInterval = 0.15; // pohyb každých 0.15 s
        public bool IsAlive { get; private set; } = true;

        private int gridWidth = 30;
        private int gridHeight = 20;

        public Snake(int startX, int startY, int startLength = 4, int gridW = 30, int gridH = 20)
        {
            gridWidth = gridW;
            gridHeight = gridH;
            for (int i = 0; i < startLength; i++)
            {
                body.Add(new Point(startX - i, startY));
            }
        }


        public void Update(GameTime gameTime)
        {
            moveTimer += gameTime.ElapsedGameTime.TotalSeconds;
            HandleInput();

            if (moveTimer >= moveInterval)
            {
                moveTimer = 0;
                Move();
            }
        }

        private void HandleInput()
        {
            var k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Up) && direction.Y == 0) direction = new Point(0, -1);
            else if (k.IsKeyDown(Keys.Down) && direction.Y == 0) direction = new Point(0, 1);
            else if (k.IsKeyDown(Keys.Left) && direction.X == 0) direction = new Point(-1, 0);
            else if (k.IsKeyDown(Keys.Right) && direction.X == 0) direction = new Point(1, 0);
        }

        private void Move()
        {
            Point newHead = new Point(body[0].X + direction.X, body[0].Y + direction.Y);

            // kolize se stěnami
            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= gridWidth || newHead.Y >= gridHeight)
            {
                IsAlive = false;
                return;
            }

            // kolize se sebou
            if (body.Contains(newHead))
            {
                IsAlive = false;
                return;
            }

            body.Insert(0, newHead);
            body.RemoveAt(body.Count - 1);
        }


        public void Grow(int segments = 2)
        {
            for (int i = 0; i < segments; i++)
                body.Add(body[body.Count - 1]);
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D texture, int cellSize, Color color)
        {
            foreach (var segment in body)
            {
                spriteBatch.Draw(texture,
                    new Rectangle(segment.X * cellSize, segment.Y * cellSize, cellSize, cellSize),
                    color);
            }
        }


        public Point Head => body[0];
        public List<Point> Body => body;
    }

}
