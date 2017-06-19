using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DestinyDescent.Entities
{
    public class Ghost : Entity
    {
        #region Global Variables
        private Texture2D ghost;
        #endregion

        #region Bounding Box
        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, ghost.Width, ghost.Height);
            }
        }
        #endregion

        #region Constructor
        public Ghost (Game g, int width, int height, int ghostLoc, float yPos, Texture2D sprite) : base (g, width, height)
        {
            position.X = ghostLoc;
            position.Y = yPos + 5; // Makes it appear buried

            ghost = sprite;
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ghost, position, Color.White);
        }
        #endregion

        #region Move Ghost
        public void moveGhost(float tmpSpeed)
        {
            position.Y -= tmpSpeed;
        }
        #endregion
    }
}
