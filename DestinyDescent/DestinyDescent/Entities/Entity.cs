using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DestinyDescent.Entities
{
    public abstract class Entity : DrawableGameComponent
    {
        #region Global Variables
        protected int gameWidth;
        protected int gameHeight;

        protected Vector2 position;
        protected Vector2 speed;
        #endregion

        public abstract Rectangle BoundingBox { get; }

        #region Constructor
        public Entity (Game g, int width, int height) :base(g)
        {
            gameWidth = width;
            gameHeight = height;

            position = Vector2.Zero;
            speed = new Vector2(0.0f, 0.0f);
        }
        #endregion

        protected int getGameWidth()
        {
            return gameWidth;
        }

        protected int getGameHeight()
        {
            return gameHeight;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
