using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Had
{
    public class Snake : IGameObject
    {
        public Point Position => Head;
        public LayerType Layer { get; set; } = LayerType.Main;

        private List<Point> body = new List<Point>();
        private Point direction = new Point(1, 0);
        private double moveTimer = 0;
        private double moveInterval = 0.15;
        public bool IsAlive { get; private set; } = true;

        private int gridWidth = 30;
        private int gridHeight = 20;

        private bool spacePressed = false;

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

                if (Layer == LayerType.Shadow)
                {
                    Move();
                    Shrink(1);
                    if (body.Count == 0)
                        Kill();
                }
                else
                {
                    Move();
                }
            }
        }

        private void HandleInput()
        {
            var k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.Space) && !spacePressed)
            {
                ToggleLayer();
                spacePressed = true;
            }
            else if (k.IsKeyUp(Keys.Space))
            {
                spacePressed = false;
            }

            if (Layer == LayerType.Main)
            {
                if (k.IsKeyDown(Keys.Up) && direction.Y == 0) direction = new Point(0, -1);
                else if (k.IsKeyDown(Keys.Down) && direction.Y == 0) direction = new Point(0, 1);
                else if (k.IsKeyDown(Keys.Left) && direction.X == 0) direction = new Point(-1, 0);
                else if (k.IsKeyDown(Keys.Right) && direction.X == 0) direction = new Point(1, 0);
            }
        }

        private void ToggleLayer()
        {
            Layer = Layer == LayerType.Main ? LayerType.Shadow : LayerType.Main;
        }

        private void Move()
        {
            Point newHead = new Point(body[0].X + direction.X, body[0].Y + direction.Y);

            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= gridWidth || newHead.Y >= gridHeight)
            {
                Kill();
                return;
            }

            if (body.Contains(newHead))
            {
                Kill();
                return;
            }

            body.Insert(0, newHead);
            body.RemoveAt(body.Count - 1);
        }

        private void Shrink(int segments)
        {
            for (int i = 0; i < segments; i++)
            {
                if (body.Count > 0)
                    body.RemoveAt(body.Count - 1);
            }
        }

        public void Grow(int segments = 2)
        {
            for ( int i = 0; i < segments; i++)
            {
                if (body.Count > 0)
                    body.Add(body[body.Count - 1]);
            }
        }

        public void Kill()
        {
            IsAlive = false;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, int cellSize, Color color)
        {
            foreach (var segment in body)
            {
                spriteBatch.Draw(pixel,
                    new Rectangle(segment.X * cellSize, segment.Y * cellSize, cellSize, cellSize),
                    color);
            }
        }

        public Point Head => body.Count > 0 ? body[0] : new Point(-1, -1);
        public List<Point> Body => body;

        // Debug properties
        public double DebugMoveTimer => moveTimer;
        public double DebugMoveInterval => moveInterval;
        public Point DebugDirection => direction;
        public int DebugBodyCount => body.Count;
    }
}
