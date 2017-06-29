using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DestinyDescent.Entities;

namespace DestinyDescent
{
    public class LedgeManager : DrawableGameComponent
    {
        #region Global Variables
        private Game game;
        private Random rand;
        private int gameWidth;
        private int gameHeight;

        private Guardian guardian;
        private List<Ledge> ledges;
        private List<Texture2D> ledgeSprites;
        private TimeSpan newLedgeTimer;
        private TimeSpan difficultyTimer;
        private int ledgeGenSpeed;
        private float speedVar;
        private int difficulty;
        private bool maxDifficulty;

        private int scoreInc;

        private Texture2D ghostSprite;
        #endregion

        public LedgeManager(Game g, Random r, Guardian player, int width, int height) : base (g)
        {
            game = g;
            gameWidth = width;
            gameHeight = height;

            guardian = player;
            ledges = new List<Ledge>();
            newLedgeTimer = new TimeSpan();
            difficultyTimer = new TimeSpan();
            increaseDifficulty();

            rand = r;

            LoadContent();
        }

        protected override void LoadContent()
        {
            ledgeSprites = new List<Texture2D>();
            ledgeSprites.Add(game.Content.Load<Texture2D>("Ledges/Ledge_1"));
            ledgeSprites.Add(game.Content.Load<Texture2D>("Ledges/Ledge_2"));
            ledgeSprites.Add(game.Content.Load<Texture2D>("Ledges/Ledge_3"));
            ledgeSprites.Add(game.Content.Load<Texture2D>("Ledges/Ledge_4"));
            ledgeSprites.Add(game.Content.Load<Texture2D>("Ledges/Ledge_Edge_R"));
            ledgeSprites.Add(game.Content.Load<Texture2D>("Ledges/Ledge_Edge_L"));

            ghostSprite = game.Content.Load<Texture2D>("Ghost");

            generateLedge();

            base.LoadContent();
        }

        public int getScoreIncrement()
        {
            int score = scoreInc;
            scoreInc = 0;
            return score;
        }

        private void increaseDifficulty()
        {
            difficulty++;

            switch (difficulty)
            {
                case 1:
                    ledgeGenSpeed = 1500;
                    speedVar = 2.5f;
                    break;
                case 2:
                    ledgeGenSpeed = 1200;
                    speedVar = 2.5f;
                    break;
                case 3:
                    ledgeGenSpeed = 1200;
                    speedVar = 3.0f;
                    break;
                case 4:
                    ledgeGenSpeed = 1200;
                    speedVar = 3.5f;
                    break;
                case 5:
                    ledgeGenSpeed = 1000;
                    speedVar = 3.5f;
                    break;
                case 6:
                    ledgeGenSpeed = 900;
                    speedVar = 4.0f;
                    break;
                case 7:
                    ledgeGenSpeed = 800;
                    speedVar = 4.5f;
                    break;
                default:
                    maxDifficulty = true;
                    break;
            }

            System.Diagnostics.Debug.WriteLine(difficulty);
        }

        #region Create New Ledge
        protected void generateLedge()
        {
            int gap = (rand.Next(gameWidth - 96) / ledgeSprites[0].Width);

            if (gap == 0)
                gap = 1;

            gap *= ledgeSprites[0].Width;
            Ledge tmpLedge = new Ledge(game, gameWidth, gameHeight, gap, (ledgeSprites[0].Width * 2), ledgeSprites, rand);

            int ghost = rand.Next(gameWidth);

            // 10% chance of spawning a Ghost
            if (ghost % 10 == 7)
                tmpLedge.triggerGhost(game, ghost, ghostSprite);

            ledges.Add(tmpLedge);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            newLedgeTimer += gameTime.ElapsedGameTime;

            if (newLedgeTimer.TotalMilliseconds > ledgeGenSpeed)
            {
                generateLedge();
                newLedgeTimer = TimeSpan.Zero;
            }

            if (!maxDifficulty)
            {
                difficultyTimer += gameTime.ElapsedGameTime;

                if (difficultyTimer.TotalMilliseconds > 15000)
                {
                    increaseDifficulty();
                    difficultyTimer = TimeSpan.Zero;
                }
            }
            
            if (ledges.Count != 0 && ledges[0].offScreen())
            {
                Ledge oldLedge = ledges[0];
                ledges.RemoveAt(0);
                oldLedge = null; // Dispose
            }

            bool checkLedge = true;
            foreach (var ledge in ledges)
            {
                if (checkLedge && !ledge.passed() && !guardian.guardianDown())
                {
                    if (ledge.Intersects(guardian.BoundingBox) || guardian.isBoosting())
                    {
                        if (ledge.ghostCheck() && guardian.BoundingBox.Intersects(ledge.getGhost.BoundingBox) && !guardian.isBoosting())
                        {
                            ledge.grabGhost();
                            scoreInc += 250;
                        }

                        if (guardian.isFalling())
                        {
                            guardian.toggleFalling(false);

                            int movement = (ledge.BoundingBox.Top - guardian.BoundingBox.Bottom) * -1;
                            guardian.moveGuardian(movement - 1.0f);
                        }
                    }
                    else
                    {
                        guardian.toggleFalling(true);

                        if (ledge.gapCheck())
                        {
                            scoreInc += 100;
                        }
                    }


                    checkLedge = false;
                }

                ledge.moveLedge(speedVar);
            }

            if (!guardian.isFalling()) guardian.moveGuardian(speedVar);
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draws Ghost first
            for (int x = 0; x < ledges.Count; x++)
            {
                if (ledges[x].ghostCheck())
                    ledges[x].getGhost.Draw(gameTime, spriteBatch);
            }

            // Draws Guardian
            guardian.Draw(gameTime, spriteBatch);

            // Draws Ledges
            for (int x = 0; x < ledges.Count; x++)
            {
                ledges[x].Draw(gameTime, spriteBatch);
            }
        }
        #endregion
    }
}
