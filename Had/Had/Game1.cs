using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;

namespace Had
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private const int CellSize = 20;
        private const int GridWidth = 30;
        private const int GridHeight = 20;

        private Snake snake;
        private Food food;
        private Texture2D pixel;
        public bool IsAlive { get; private set; } = true;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            snake = new Snake(GridWidth / 2, GridHeight / 2, 6); // začátek délka hada = poslední číslo
            food = new Food(5, 5);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }
        private void RestartGame()
        {
            snake = new Snake(GridWidth / 2, GridHeight / 2, 6, GridWidth, GridHeight);
            food.Respawn(GridWidth, GridHeight);
        }


        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!snake.IsAlive)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    RestartGame();
                }
                return; // stop update, dokud se nerestartuje
            }

            snake.Update(gameTime);

            // kolize s jídlem
            if (snake.Head == food.Position)
            {
                snake.Grow(2);
                food.Respawn(GridWidth, GridHeight);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            snake.Draw(_spriteBatch, pixel, CellSize, snake.IsAlive ? Color.LimeGreen : Color.DarkRed);
            food.Draw(_spriteBatch, pixel, CellSize);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
