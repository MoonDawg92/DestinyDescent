using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DestinyDescent.Entities
{
    class Ledge : Entity
    {
        #region Global Declarataions
        private int gapSize;
        private int endGap;
        private int ledgeWidth;
        private int ledgeHeight;
        private int ledgeSize;      // Represents area that player stands on 

        private bool passedThrough;

        private int widthOne;
        private int widthTwo;

        private bool ghostPresent;
        private int ghostLocation;

        private List<Texture2D> ledges;
        private List<Texture2D> ledgeSprites;

        private Texture2D ghostSprite;
        private Ghost ghost;

        //Random rand;
        #endregion

        #region Bounding Box
        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(0, (int)(position.Y + (ledgeSize / 2)), getGameWidth(), ledgeSize);
            }
        }
        #endregion

        #region Constructor
        public Ledge(Game g, int width, int height, int loc, int size, List<Texture2D> sprites, Random rand) : base (g, width, height)
        {
            ledges = new List<Texture2D>();
            ledgeSprites = new List<Texture2D>();
            ledgeSprites.AddRange(sprites);

            ledgeWidth = ledgeSprites[0].Width;
            ledgeHeight = ledgeSprites[0].Height;
            ledgeSize = ledgeHeight / 2;
            widthOne = loc;
            gapSize = size;
            endGap = widthOne + gapSize;
            widthTwo = gameWidth - endGap;

            passedThrough = false;
            ghostPresent = false;
            ghostLocation = 0;

            position.X = 0;
            position.Y = getGameHeight() + ledgeSprites[0].Height;

            generateLedge(rand);
        }

        #region Generate Ledge
        private void generateLedge(Random rand)
        {
            // --------\     /--------     <-- How ledge should look

            // Draw first part of ledge
            for (int x = 0; x < (widthOne - ledgeWidth); x += ledgeWidth)
            {
                ledges.Add(getLedgeSprite(ledgeSprites, rand));
            }

            // Draw gap of first ledge
            ledges.Add(ledgeSprites[4]);

            // Draw gap of second ledge
            if (endGap != getGameWidth())
                ledges.Add(ledgeSprites[5]);

            // Draw second part of ledge 
            for (int x = (endGap + ledgeWidth); x < getGameWidth(); x += ledgeWidth)
            {
                ledges.Add(getLedgeSprite(ledgeSprites, rand));
            }
        }

        private Texture2D getLedgeSprite(List<Texture2D> ledgeSprites, Random rand)
        {
            int ledgeIndex = rand.Next(4);

            if (ledgeIndex == 0) // Has a rare chance
                ledgeIndex = rand.Next(4);

            return ledgeSprites[ledgeIndex];
        }
        #endregion
        #endregion

        #region Passed Through
        public bool passed()
        {
            return passedThrough;
        }
        #endregion

        #region Ghost Logic
        #region Get Ghost
        public Ghost getGhost
        {
            get
            {
                return ghost;
            }
        }
        #endregion

        #region Trigger Ghost
        public void triggerGhost(Game g, int ghostLoc, Texture2D spr)
        {
            ghostPresent = true;
            ghostLocation = ghostLoc;
            ghostSprite = spr;

            if ((ghostLocation - widthOne) < gapSize && (ghostLocation - widthOne > 0))
            {
                if ((ghostLocation - gapSize) > 0)
                    ghostLocation -= gapSize;

                else
                    ghostLocation += gapSize;
            }

            ghost = new Ghost(g, getGameWidth(), getGameHeight(), ghostLocation, position.Y, ghostSprite);
        }
        #endregion

        #region Ghost Present
        public bool ghostCheck()
        {
            return ghostPresent;
        }
        #endregion

        #region Grab Ghost
        public void grabGhost()
        {
            ghostPresent = false;
        }
        #endregion
        #endregion

        #region Gap Check
        public bool gapCheck(int spriteWidth, float pos, bool boosting)
        {
            if (widthOne < pos && ((pos + spriteWidth) < endGap) && !boosting)
            {
                passedThrough = true;
                return true;
            }

            return false;
        }
        #endregion

        #region Move Ledge
        public void moveLedge(float tmpSpeed)
        {
            position.Y -= tmpSpeed;

            if (ghostPresent)
                ghost.moveGhost(tmpSpeed);
        }
        #endregion

        #region Ledge Off Screen
        public bool offScreen()
        {
            if ((position.Y + ledgeHeight) < 0)
                return true;

            return false;
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            position.X = 0;
            int index = 0;
            
            for (int x = 0; x < widthOne; x += ledgeWidth)
            {
                spriteBatch.Draw(ledges[index], position, Color.White);
                position.X += ledgeSprites[0].Width;
                index++;
            }

            //Become ledge-end
            position.X = endGap;

            for (int x = endGap; x < getGameWidth(); x += ledgeWidth)
            {
                spriteBatch.Draw(ledges[index], position, Color.White);
                position.X += ledgeSprites[0].Width;
                index++;
            }
        }
        #endregion
    }
}
