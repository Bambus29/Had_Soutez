using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Had
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Food
    {
        public Point Position { get; private set; }

        public Food(int x, int y)
        {
            Position = new Point(x, y);
        }

        public void Respawn(int gridWidth, int gridHeight)
        {
            Random rnd = new Random();
            Position = new Point(rnd.Next(gridWidth), rnd.Next(gridHeight));
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, int cellSize)
        {
            spriteBatch.Draw(texture, new Rectangle(Position.X * cellSize, Position.Y * cellSize, cellSize, cellSize), Color.Red);
        }
    }

}
