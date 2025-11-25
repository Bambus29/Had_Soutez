using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Had
{
    public enum LayerType
    {
        Main,
        Shadow,
        Both
    }

    public interface IGameObject
    {
        Point Position { get; }
        LayerType Layer { get; }

        void Update(GameTime gameTime);

        // Přidán parametr color
        void Draw(SpriteBatch spriteBatch, Texture2D pixel, int cellSize, Color color);
    }
}
