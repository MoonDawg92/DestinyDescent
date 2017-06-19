using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DestinyDescent.Entities
{
    public class Text : Game
    {
        #region Global Variables
        protected int gameWidth;
        protected int gameHeight;

        protected Vector2 position;
        protected SpriteFont font;
        protected Color textColor;
        #endregion

        #region Constructor
        public Text(int width, int height, Vector2 pos, SpriteFont f, Color c)
        {
            gameWidth = width;
            gameHeight = height;

            position = pos;
            font = f;
            textColor = c;
        }
        #endregion

        public void setColor(Color col)
        {
            textColor = col;
        }

        protected int getGameWidth()
        {
            return gameWidth;
        }

        protected int getGameHeight()
        {
            return gameHeight;
        }

        public void Draw(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawString(font, text, position, textColor);
        }
    }
}
