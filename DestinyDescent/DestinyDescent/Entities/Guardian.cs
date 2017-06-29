using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DestinyDescent.Entities
{
    public class Guardian : Entity
    {
        #region Global Declarataions
        private Game game;

        private int speedVar;
        private int gravityVar;
        private TimeSpan boostTimer;
        private TimeSpan boostCooldown;
        private float velocity;
        private int index;
        private TimeSpan spriteChange;
        private int xMin;
        private int xMax;

        private bool facingRight;
        private bool boosting;
        private bool boostReady;
        private bool falling;

        private string playerClass;
        private Texture2D spriteSheet;

        private List<Rectangle> guardianLeft;   // Bounds for sprite sheet
        private List<Rectangle> guardianRight;  // "                     "

        private KeyboardState oldState;
        private List<Keys> keysPressed;

        private bool gameOver;
        #endregion

        #region BoundingBox
        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, guardianRight[0].Width, guardianRight[0].Height);
            }
        }
        #endregion

        #region Constructor
        public Guardian(Game g, string pClass, int width, int height) : base(g, width, height)
        {
            game = g;

            speedVar = 500;
            gravityVar = 5;
            boostTimer = TimeSpan.Zero;
            boostCooldown = TimeSpan.Zero;
            velocity = 0;
            index = 5;
            spriteChange = TimeSpan.Zero;
            facingRight = true;
            boosting = false;
            boostReady = true;
            falling = true;
            guardianLeft = new List<Rectangle>();
            guardianRight = new List<Rectangle>();
            keysPressed = new List<Keys>();
            oldState = Keyboard.GetState();
            
            LoadSpritePositions();

            xMin = 0;
            xMax = getGameWidth() - guardianLeft[0].Width;

            playerClass = pClass;

            LoadContent();
        }
        #endregion

        #region Load Content
        private void LoadSpritePositions()
        {
            guardianLeft.Add(new Rectangle(0, 52, 50, 52));
            guardianLeft.Add(new Rectangle(50, 52, 50, 52));
            guardianLeft.Add(new Rectangle(100, 52, 50, 52));
            guardianLeft.Add(new Rectangle(150, 52, 50, 52));
            guardianLeft.Add(new Rectangle(0, 0, 50, 52));
            guardianLeft.Add(new Rectangle(50, 0, 50, 52));

            guardianRight.Add(new Rectangle(0, 104, 50, 52));
            guardianRight.Add(new Rectangle(50, 104, 50, 52));
            guardianRight.Add(new Rectangle(100, 104, 50, 52));
            guardianRight.Add(new Rectangle(150, 104, 50, 52));
            guardianRight.Add(new Rectangle(100, 0, 50, 52));
            guardianRight.Add(new Rectangle(150, 0, 50, 52));
        }

        protected override void LoadContent()
        {
           spriteSheet = game.Content.Load<Texture2D>("Guardians/" + playerClass + "/" + playerClass + "Sheet");

            base.LoadContent();
        }
        #endregion

        #region Get Functions
        #region Falling
        public bool isFalling()
        {
            return falling;
        }
        #endregion

        #region Boosting
        public bool isBoosting()
        {
            return boosting;
        }
        #endregion

        #region Boost Ready
        public bool isBoostReady()
        {
            return boostReady;
        }
        #endregion

        #region Guardian Down
        public bool guardianDown()
        {
            return gameOver;
        }
        #endregion
        #endregion

        #region Toggle Falling
        public void toggleFalling(bool b)
        {
            falling = b;
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gameOver) return;

            if (facingRight)
            {
                if (boosting)
                {
                    spriteBatch.Draw(spriteSheet, new Vector2(position.X - 30, position.Y), guardianRight[index], Color.White);
                    spriteBatch.Draw(spriteSheet, new Vector2(position.X - 15, position.Y), guardianRight[index], Color.White);
                }
                spriteBatch.Draw(spriteSheet, position, guardianRight[index], Color.White);
            }
            else
            {
                if (boosting)
                {
                    spriteBatch.Draw(spriteSheet, new Vector2(position.X + 30, position.Y), guardianLeft[index], Color.White);
                    spriteBatch.Draw(spriteSheet, new Vector2(position.X + 15, position.Y), guardianLeft[index], Color.White);
                }
                spriteBatch.Draw(spriteSheet, position, guardianLeft[index], Color.White);
            }
        }
        #endregion

        #region Move Guardian
        public void moveGuardian(float tmpSpeed)
        {
            position.Y -= tmpSpeed;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            if (speed.X < 0.001 && speed.X > -0.001)
                changeVelocity(0.0f);

            position += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region Boost Ready Logic
            if (!boostReady)
            {
                boostCooldown += gameTime.ElapsedGameTime;

                if (boostCooldown.TotalMilliseconds >= 7000)
                {
                    boostReady = true;
                    boostCooldown = TimeSpan.Zero;
                }
            }
            #endregion

            #region Boosting
            if (boosting)
            {
                boostTimer += gameTime.ElapsedGameTime;

                // Facing right
                if (facingRight)
                {
                    if (velocity < 1)
                    {
                        changeVelocity(0.1f);
                    }

                    speed.X = speedVar * velocity * 3;

                    if (position.X > xMax)
                    {
                        speed.X = 0.0f;
                        position.X = xMax;
                        boosting = false;
                    }
                }

                // Facing left
                else
                {
                    if (velocity > -1)
                    {
                        changeVelocity(-0.1f);
                    }

                    speed.X = speedVar * velocity * 3;

                    if (position.X < xMin)
                    {
                        speed.X = 0.0f;
                        position.X = xMin;
                        boosting = false;
                    }
                }

                // Boost depleted
                if (boostTimer.TotalMilliseconds > 150)
                    boosting = false;

                index = 5;

                if (!boosting)
                {
                    index = 4;
                    boostTimer = TimeSpan.Zero;
                    boostReady = false;
                    speed.X = 0.0f;
                    keysPressed.Remove(Keys.Space);
                }
            }
            #endregion

            #region Not Boosting
            else
            {
                if (keysPressed.Exists(x => x.Equals(Keys.Right)) && keysPressed.Exists(x => x.Equals(Keys.Left)))
                {
                    #region Left and Right Both Down
                    if (velocity > 0)
                        changeVelocity(-0.1f);

                    else if (velocity < 0)
                        changeVelocity(0.1f);

                    if (newState.IsKeyUp(Keys.Right))
                        keysPressed.Remove(Keys.Right);

                    if (newState.IsKeyUp(Keys.Left))
                        keysPressed.Remove(Keys.Left);

                    if (!falling)
                        index = 4;
                    #endregion
                }

                else
                {
                    #region Boost Activated
                    if (newState.IsKeyDown(Keys.Space) && !keysPressed.Exists(x => x.Equals(Keys.Space)) && !falling && boostReady)
                    {
                        keysPressed.Add(Keys.Space);
                        boosting = true;
                    }
                    #endregion

                    #region Move Right
                    if (newState.IsKeyDown(Keys.Right) && !keysPressed.Exists(x => x.Equals(Keys.Right)))
                        keysPressed.Add(Keys.Right);

                    else if (newState.IsKeyUp(Keys.Right) && keysPressed.Exists(x => x.Equals(Keys.Right)))
                        keysPressed.Remove(Keys.Right);

                    if (keysPressed.Exists(x => x.Equals(Keys.Right)) && !keysPressed.Exists(x => x.Equals(Keys.Left)))
                    {
                        if (velocity < 1)
                        {
                            changeVelocity(0.1f);

                            if (velocity > 0)
                                facingRight = true;
                            else
                                spriteChange -= gameTime.ElapsedGameTime; // Doesn't change sprites while "sliding"
                        }

                        spriteChange += gameTime.ElapsedGameTime;

                        if (position.X > xMax)
                        {
                            speed.X = 0.0f;
                            position.X = xMax;
                            spriteChange -= gameTime.ElapsedGameTime;
                        }

                        if (spriteChange.TotalMilliseconds > 100)
                        {
                            spriteChange = TimeSpan.Zero;

                            if (index < 3)
                                index++;
                            else
                                index = 0;
                        }
                    }
                    #endregion

                    #region Move Left
                    if (newState.IsKeyDown(Keys.Left) && !keysPressed.Exists(x => x.Equals(Keys.Left)))
                        keysPressed.Add(Keys.Left);

                    else if (newState.IsKeyUp(Keys.Left) && keysPressed.Exists(x => x.Equals(Keys.Left)))
                        keysPressed.Remove(Keys.Left);

                    if (keysPressed.Exists(x => x.Equals(Keys.Left)) && !keysPressed.Exists(x => x.Equals(Keys.Right)))
                    {


                        if (velocity > -1)
                        {
                            changeVelocity(-0.1f);

                            if (velocity < 0)
                                facingRight = false;
                            else
                                spriteChange -= gameTime.ElapsedGameTime; // Doesn't change sprites while "sliding"
                        }

                        spriteChange += gameTime.ElapsedGameTime;

                        if (position.X < xMin)
                        {
                            speed.X = 0.0f;
                            position.X = xMin;
                            spriteChange -= gameTime.ElapsedGameTime;
                        }

                        if (spriteChange.TotalMilliseconds > 100)
                        {
                            spriteChange = TimeSpan.Zero;

                            if (index < 3)
                                index++;
                            else
                                index = 0;
                        }
                    }
                    #endregion

                    #region Standing Still
                    if (!keysPressed.Exists(x => x.Equals(Keys.Left)) && !keysPressed.Exists(x => x.Equals(Keys.Right)))
                    {
                        if (velocity > 0)
                            changeVelocity(-0.1f);

                        else if (velocity < 0)
                            changeVelocity(0.1f);

                        if (position.X > xMax)
                            position.X = xMax;

                        if (position.X < xMin)
                            position.X = xMin;

                        index = 4;

                    }
                    #endregion
                }
            }
            #endregion

            #region Falling
            if (falling)
            {
                index = 5;
                position.Y += gravityVar;
            }
            #endregion

            // Update saved state.
            oldState = newState;

            if (this.BoundingBox.Bottom < 0 || this.BoundingBox.Top > getGameHeight()) gameOver = true;
        }
        #endregion

        #region Velocity Change
        private void changeVelocity(float val)
        {
            if (val == 0.0f)
                velocity = 0.0f;

            else
                velocity += val;

            speed.X = velocity * speedVar;
        }
        #endregion
    }
}
