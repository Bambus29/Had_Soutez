using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        // random pro spawn duchů
        private Random _rand = new Random();

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

            // vytvoření duchů na bezpečných pozicích (nepřekrývat hada ani jídlo)
            duchove.Clear();
            for (int i = 0; i < 3; i++)
            {
                var p = FindSafeSpawn();
                duchove.Add(new Duch(p.X, p.Y, snake.Body.Count, GridWidth, GridHeight));
            }

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

        private Point FindSafeSpawn()
        {
            // zkusíme náhodně najít volnou pozici; pokud to nepůjde, projdeme grid
            for (int attempt = 0; attempt < 200; attempt++)
            {
                var x = _rand.Next(0, GridWidth);
                var y = _rand.Next(0, GridHeight);
                var p = new Point(x, y);
                if (!snake.Body.Contains(p) && (food == null || p != food.Position))
                    return p;
            }

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    var p = new Point(x, y);
                    if (!snake.Body.Contains(p) && (food == null || p != food.Position))
                        return p;
                }
            }

            // krajní fallback
            return new Point(0, 0);
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

            // vyčistit a vytvořit nové duchy na bezpečných pozicích
            duchove.Clear();
            for (int i = 0; i < 3; i++)
            {
                var p = FindSafeSpawn();
                duchove.Add(new Duch(p.X, p.Y, snake.Body.Count, GridWidth, GridHeight));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // debug: increment and show a quick status in window title so we can confirm Update runs
            updateCounter++;
            var sHead = snake?.Head ?? new Point(-1, -1);
            var d0Head = duchove.Count > 0 ? duchove[0].Head : new Point(-1, -1);

            // Více debug informací pro zjištění proč se nepohybují:
            string snakeDebug = snake != null ? $"{snake.DebugMoveTimer:F2}/{snake.DebugMoveInterval:F2} d:{snake.DebugDirection.X},{snake.DebugDirection.Y} alive:{snake.IsAlive} lastKill:{snake.LastKillReason}" : "n/a";
            string duchDebug = duchove.Count > 0 ? $"{duchove[0].DebugMoveTimer:F2}/{duchove[0].DebugMoveInterval:F2} d:{duchove[0].DebugDirection.X},{duchove[0].DebugDirection.Y}" : "n/a";
            Window.Title = $"U:{updateCounter} ET:{gameTime.ElapsedGameTime.TotalMilliseconds:F1} S:{sHead.X},{sHead.Y} [{snakeDebug}] D0:{d0Head.X},{d0Head.Y} [{duchDebug}]";

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
                            // předáme důvod pro rychlou diagnostiku
                            snake.Kill("duch-collision");
                        }
                    }
                }
            }

            base.Update(gameTime);
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

            // duchové - vykreslíme je jen pokud je hráč v hlavní vrstvě
            if (snake.Layer == LayerType.Main)
            {
                foreach (var duch in duchove)
                    duch.Draw(_spriteBatch, pixel, CellSize, Color.OrangeRed);
            }

            // když hráč zemře, překreslíme lehce červený overlay přes herní plochu
            if (snake != null && !snake.IsAlive)
            {
                // průhledná červená (alfa ~ 80/255)
                _spriteBatch.Draw(pixel, new Rectangle(0, 0, GridWidth * CellSize, GridHeight * CellSize), new Color(255, 0, 0, 80));
            }

            // skóre
            if (font != null)
            {
                _spriteBatch.DrawString(font, $"High Score: {highScore}", new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, $"Max Score: {maxScore}", new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(font, $"Current Score: {currentScore}", new Vector2(10, 50), Color.White);

                if (snake != null && !snake.IsAlive)
                {
                    // Zobrazíme i text nápovědy pro restart, pokud je font k dispozici
                    var msg = "You died - press R to restart";
                    var size = font.MeasureString(msg);
                    var pos = new Vector2((GridWidth * CellSize - size.X) / 2, (GridHeight * CellSize - size.Y) / 2);
                    _spriteBatch.DrawString(font, msg, pos, Color.White);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
