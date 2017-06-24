using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DestinyDescent
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D background;

        private int gameWidth;
        private int gameHeight;

        private Menu menu;
        private Descent descent;

        private enum GameState
        {
            StartMenu,
            Loading,
            Playing,
            Paused
        }

        private GameState gameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.Title = "Descent of Darkness";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameState = GameState.StartMenu;
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;
            graphics.ApplyChanges();

            gameWidth = graphics.GraphicsDevice.Viewport.Width;
            gameHeight = graphics.GraphicsDevice.Viewport.Height;

            background = this.Content.Load<Texture2D>("MenuBackground");

            menu = new Menu(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.StartMenu)
            {
                // Update main menu
                menu.Update(gameTime);
                if (menu.activeGame()) gameState = GameState.Playing;
            }

            else if (gameState == GameState.Playing)
            {
                // Update game
                if (descent == null) descent = new Descent(this, menu.getClassChoice());
                descent.Update(gameTime);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw background
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            if (gameState == GameState.StartMenu)
            {
                // Draw menu
                menu.Draw(gameTime, spriteBatch);
            }

            else if (gameState == GameState.Playing)
            {
                // Draw game
                if (descent == null) descent = new Descent(this, menu.getClassChoice());
                descent.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
