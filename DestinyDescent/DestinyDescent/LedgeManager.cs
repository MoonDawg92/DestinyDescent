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

        private List<Ledge> ledges;
        private List<Texture2D> ledgeSprites;
        private TimeSpan newLedgeTimer;
        private int ledgeGenSpeed;
        private float speedVar;

        private int scoreInc;

        private Texture2D ghostSprite;
        #endregion

        public LedgeManager(Game g, int width, int height, List<Texture2D> sprites, Texture2D ghost, float speed) : base (g)
        {
            game = g;
            gameWidth = width;
            gameHeight = height;

            ledges = new List<Ledge>();
            ledgeSprites = sprites;
            ghostSprite = ghost;
            newLedgeTimer = new TimeSpan();
            ledgeGenSpeed = 1500; // temporary
            speedVar = speed;
            scoreInc = 0;

            rand = new Random();
        }

        public override void Update(GameTime gameTime)
        {
            newLedgeTimer += gameTime.ElapsedGameTime;

            if (newLedgeTimer.TotalMilliseconds > ledgeGenSpeed)
            {
                generateLedge();
                newLedgeTimer = TimeSpan.Zero;
            }

            if (ledges.Count != 0 && ledges[0].offScreen())
            {
                Ledge oldLedge = ledges[0];
                ledges.RemoveAt(0);
                oldLedge = null; // Dispose
            }

            foreach (var ledge in ledges)
            {
                if (!ledge.passed())
                {
                    // Logic with guardian needed here
                }
            }
        }

        public int getScoreIncrement()
        {
            int score = scoreInc;
            scoreInc = 0;
            return score;
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
    }
}
