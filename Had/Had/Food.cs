using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Had
{
    public class Food : IGameObject
    {
        public Point Position { get; private set; }
        public LayerType Layer => LayerType.Main;

        private Random random = new Random();

        public Food(int x, int y)
        {
            Position = new Point(x, y);
        }

        public void Respawn(int gridWidth, int gridHeight)
        {
            Position = new Point(random.Next(0, gridWidth), random.Next(0, gridHeight));
        }

        public void Update(GameTime gameTime)
        {
            // nic nedělá
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, int cellSize, Color color)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(Position.X * cellSize, Position.Y * cellSize, cellSize, cellSize),
                color
            );
        }
    }
}
