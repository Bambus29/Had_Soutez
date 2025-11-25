using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        private List<Duch> duchove = new List<Duch>();

        private int currentScore = 0;
        private int maxScore = 0;
        private int highScore = 0;

        private SpriteFont font;

        // debug counters
        private int updateCounter = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            snake = new Snake(GridWidth / 2, GridHeight / 2, 6, GridWidth, GridHeight);
            food = new Food(5, 5);

            // vytvoření duchů
            duchove.Clear();
            for (int i = 0; i < 3; i++)
                duchove.Add(new Duch(i * 5, i * 5, snake.Body.Count, GridWidth, GridHeight));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // font pro UI, pokud máš
            // font = Content.Load<SpriteFont>("DefaultFont");
        }

        private void RestartGame()
        {
            // uložit skóre
            maxScore = currentScore > maxScore ? currentScore : maxScore;
            highScore = currentScore > highScore ? currentScore : highScore;

            // reset skóre
            currentScore = 0;

            // vytvořit nový had
            snake = new Snake(GridWidth / 2, GridHeight / 2, 6, GridWidth, GridHeight);

            // reset jídla
            food.Respawn(GridWidth, GridHeight);

            // vyčistit a vytvořit nové duchy
            duchove.Clear();
            for (int i = 0; i < 3; i++)
            {
                // nové startovní pozice duchů
                duchove.Add(new Duch(5 + i * 5, 5 + i * 5, snake.Body.Count, GridWidth, GridHeight));
            }
        }


        protected override void Update(GameTime gameTime)
        {
            // debug: increment and show a quick status in window title so we can confirm Update runs
            updateCounter++;
            var sHead = snake?.Head ?? new Point(-1, -1);
            var d0Head = duchove.Count > 0 ? duchove[0].Head : new Point(-1, -1);
            Window.Title = $"U:{updateCounter} ET:{gameTime.ElapsedGameTime.TotalMilliseconds:F1} S:{sHead.X},{sHead.Y} D0:{d0Head.X},{d0Head.Y}";

            var kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.Escape))
                Exit();

            // Restart hry
            if (!snake.IsAlive)
            {
                if (kState.IsKeyDown(Keys.R))
                    RestartGame();
                return;
            }

            // Aktualizace hada
            snake.Update(gameTime);

            // Odečítání skóre ve stínové vrstvě
            if (snake.Layer == LayerType.Shadow && currentScore > 0)
                currentScore--;

            // Kolize s jídlem
            if (snake.Layer == LayerType.Main && snake.Head == food.Position)
            {
                snake.Grow(2);
                currentScore += 10;
                food.Respawn(GridWidth, GridHeight);
            }

            // Aktualizace duchů
            foreach (var duch in duchove)
            {
                duch.Update(gameTime);

                // Kontrola kolize s hadem pouze pokud je hráč v hlavní vrstvě
                if (snake.Layer == LayerType.Main)
                {
                    foreach (var seg in snake.Body)
                    {
                        if (duch.Body.Contains(seg))
                        {
                            snake.Kill();
                        }
                    }
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            Color bgColor = snake.Layer == LayerType.Main ? Color.Black : Color.DarkSlateBlue;
            GraphicsDevice.Clear(bgColor);

            _spriteBatch.Begin();

            // vykreslení gridu
            for (int x = 0; x <= GridWidth; x++)
                _spriteBatch.Draw(pixel, new Rectangle(x * CellSize, 0, 1, GridHeight * CellSize), Color.Gray);
            for (int y = 0; y <= GridHeight; y++)
                _spriteBatch.Draw(pixel, new Rectangle(0, y * CellSize, GridWidth * CellSize, 1), Color.Gray);

            // had
            Color snakeColor = snake.Layer == LayerType.Main ? Color.LimeGreen : Color.MediumPurple;
            snake.Draw(_spriteBatch, pixel, CellSize, snakeColor);

            // jídlo
            if (snake.Layer == LayerType.Main)
                food.Draw(_spriteBatch, pixel, CellSize, Color.Red);

            // duchové
            foreach (var duch in duchove)
                duch.Draw(_spriteBatch, pixel, CellSize, Color.OrangeRed, snake.Layer);

            // skóre
            if (font != null)
            {
                _spriteBatch.DrawString(font, $"High Score: {highScore}", new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, $"Max Score: {maxScore}", new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(font, $"Current Score: {currentScore}", new Vector2(10, 50), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
