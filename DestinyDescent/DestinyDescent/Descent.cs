using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DestinyDescent.Entities;

namespace DestinyDescent
{
    public class Descent : DrawableGameComponent
    {
        #region Global Variables
        private int gameWidth;
        private int gameHeight;

        private Game game;

        private Text scoreText, boostText, versionText;
        private string scoreString, versionString, boostString;
        private int playerScore;

        private LedgeManager ledgeManager;

        private Guardian guardian;
        private string playerClass;

        private Random rand;

        private enum GameState
        {
            StartMenu,
            Loading,
            Playing,
            Paused
        }
        private GameState gameState;
        #endregion

        public Descent(Game g, string pClass) : base(g)
        {
            game = g;
            playerClass = pClass;
            rand = new Random();
            gameState = GameState.Loading;

            gameWidth = game.Window.ClientBounds.Width;
            gameHeight = game.Window.ClientBounds.Height;

            guardian = new Guardian(game, playerClass, gameWidth, gameHeight);
            ledgeManager = new LedgeManager(game, rand, guardian, gameWidth, gameHeight);

            scoreString = "Score: 0";
            boostString = "Ability Ready!";
            versionString = "Version 0.1";
            game.Window.Title = "Destiny Descent - " + scoreString;

            LoadContent();
        }

        protected override void LoadContent()
        {
            Color scoreColor;
            switch (playerClass)
            {
                case "Hunter":
                    scoreColor = Color.Goldenrod;
                    break;
                case "Titan":
                    scoreColor = Color.DeepSkyBlue;
                    break;
                case "Warlock":
                    scoreColor = Color.MediumOrchid;
                    break;
                default:
                    // Should never hit this
                    scoreColor = Color.White;
                    break;
            }

            scoreText = new Text(gameWidth, gameHeight, new Vector2(10, 10), game.Content.Load<SpriteFont>("Text/Score"), scoreColor);
            boostText = new Text(gameWidth, gameHeight, new Vector2(gameWidth - 325, 10), game.Content.Load<SpriteFont>("Text/Boost"), Color.Green);
            versionText = new Text(gameWidth, gameHeight, new Vector2(gameWidth - 80, gameHeight - 20), game.Content.Load<SpriteFont>("Text/Version"), Color.CornflowerBlue);

            gameState = GameState.Playing;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (gameState == GameState.Playing)
            {
                guardian.Update(gameTime);
                ledgeManager.Update(gameTime);

                int scoreInc = ledgeManager.getScoreIncrement();
                if (scoreInc > 0)
                {
                    playerScore += scoreInc;
                    scoreString = "Score: " + playerScore;
                    game.Window.Title = "Destiny Descent - " + scoreString;
                }

                if (guardian.isBoostReady())
                {
                    boostString = "Ability Ready!";
                    boostText.setColor(Color.Green);
                }
                else
                {
                    boostString = "Ability Recharging...";
                    boostText.setColor(Color.Red);
                }
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gameState == GameState.Playing)
            {   
                // Ledge Manager will draw the guardian
                //guardian.Draw(gameTime, spriteBatch);
                ledgeManager.Draw(gameTime, spriteBatch);

                scoreText.Draw(spriteBatch, scoreString);
                boostText.Draw(spriteBatch, boostString);
                versionText.Draw(spriteBatch, versionString);
            }

            base.Draw(gameTime);
        }
    }
}
