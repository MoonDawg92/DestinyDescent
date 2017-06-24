using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace DestinyDescent
{
    public class Menu : DrawableGameComponent
    {
        #region Global Variables
        private int gameWidth;
        private int gameHeight;

        private Game game;

        private string[] menuItems;
        private int selectedIndex;

        // Graphics
        private SpriteFont spriteFont;
        private SpriteFont instructionsFont;
        private SpriteFont chooseGuardian;
        private SpriteFont titleFont;
        private SpriteFont disclaimerFont;
        private SpriteFont creatorFont;
        
        private Texture2D logo;
        private Texture2D warlockBigSheet, hunterBigSheet, titanBigSheet;
        private Rectangle[] guardianBigSpritePositions;
        private Texture2D warlockSheet, hunterSheet, titanSheet;
        private Rectangle[] guardianSpritePositions;
        private List<Texture2D> ledges;
        private List<int> generatedLedge;
        private Texture2D ghost;

        private Color normal = new Color(204, 204, 204, 255);
        private Color hover = Color.SteelBlue;

        private KeyboardState oldState;
        private KeyboardState newState;

        private TimeSpan spriteChange;
        private int spriteIndex;
        private TimeSpan guardianTimer;
        private bool guardianPresent;
        private int? guardianSheet;
        private int? lastGuardianSheet;
        private int guardianX;

        private Vector2 position;

        Random rand;

        private bool playGame;
        private string playerClass;

        private enum MenuState
        {
            MainScreen,
            ChooseGuardian,
            Instructions
        }

        private MenuState state;
        private MenuState menuState
        {
            get { return state; }
            set
            {
                state = value;

                // Reset values
                spriteIndex = 0;
                spriteChange = TimeSpan.Zero;

                if (state == MenuState.Instructions)
                {
                    generatedLedge.Clear();
                    guardianPresent = false;
                    guardianTimer = TimeSpan.Zero;
                    guardianSheet = null;
                    guardianX = -250;
                }
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (selectedIndex < 0)
                    selectedIndex = 0;

                if (menuState == MenuState.MainScreen && selectedIndex >= menuItems.Length)
                    selectedIndex = menuItems.Length - 1;
                else if (menuState == MenuState.ChooseGuardian && selectedIndex >= 3)
                    selectedIndex = 2;
            }
        }
        #endregion

        #region Constructor
        public Menu(Game g) : base(g)
        {
            game = g;

            gameWidth = game.Window.ClientBounds.Width;
            gameHeight = game.Window.ClientBounds.Height;

            menuItems = new string[3] { "Play", "Instructions", "Exit" };
            playGame = false;
            playerClass = "";

            spriteChange = TimeSpan.Zero;

            rand = new Random();

            Initialize();

            state = MenuState.MainScreen;
        }
        #endregion

        #region Initialize
        public override void Initialize()
        {
            // Load graphics
            logo = game.Content.Load<Texture2D>("Darkness");
            warlockBigSheet = game.Content.Load<Texture2D>("Guardians/Warlock/WarlockSheetBig");
            hunterBigSheet = game.Content.Load<Texture2D>("Guardians/Hunter/HunterSheetBig");
            titanBigSheet = game.Content.Load<Texture2D>("Guardians/Titan/TitanSheetBig");
            warlockSheet = game.Content.Load<Texture2D>("Guardians/Warlock/WarlockSheet");
            hunterSheet = game.Content.Load<Texture2D>("Guardians/Hunter/HunterSheet");
            titanSheet = game.Content.Load<Texture2D>("Guardians/Titan/TitanSheet");
            ledges = new List<Texture2D>();
            ledges.Add(game.Content.Load<Texture2D>("Ledges/Ledge_1"));
            ledges.Add(game.Content.Load<Texture2D>("Ledges/Ledge_2"));
            ledges.Add(game.Content.Load<Texture2D>("Ledges/Ledge_3"));
            ledges.Add(game.Content.Load<Texture2D>("Ledges/Ledge_4"));
            ledges.Add(game.Content.Load<Texture2D>("Ledges/Ledge_Edge_L"));
            ledges.Add(game.Content.Load<Texture2D>("Ledges/Ledge_Edge_R"));
            generatedLedge = new List<int>();
            ghost = game.Content.Load<Texture2D>("Ghost");

            // Load sprite sheet locations
            guardianBigSpritePositions = new Rectangle[5];
            guardianBigSpritePositions[0] = new Rectangle(0, 104, 100, 104);
            guardianBigSpritePositions[1] = new Rectangle(100, 104, 100, 104);
            guardianBigSpritePositions[2] = new Rectangle(200, 104, 100, 104);
            guardianBigSpritePositions[3] = new Rectangle(300, 104, 100, 104);
            guardianBigSpritePositions[4] = new Rectangle(0, 0, 100, 104);
            guardianSpritePositions = new Rectangle[4];
            guardianSpritePositions[0] = new Rectangle(0, 104, 50, 52);
            guardianSpritePositions[1] = new Rectangle(50, 104, 50, 52);
            guardianSpritePositions[2] = new Rectangle(100, 104, 50, 52);
            guardianSpritePositions[3] = new Rectangle(150, 104, 50, 52);

            // Load fonts
            spriteFont = game.Content.Load<SpriteFont>("Text/MenuItem");
            instructionsFont = game.Content.Load<SpriteFont>("Text/InstructionsItem");
            chooseGuardian = game.Content.Load<SpriteFont>("Text/ChooseGuardian");
            titleFont = game.Content.Load<SpriteFont>("Text/Title");
            disclaimerFont = game.Content.Load<SpriteFont>("Text/Disclaimer");
            creatorFont = game.Content.Load<SpriteFont>("Text/Creator");

            CenterMenu();

            base.Initialize();
        }

        // Sets height for the menu items
        private void CenterMenu()
        {
            float height = 0f;
            float width = 0f;

            foreach (string item in menuItems)
            {
                Vector2 size = spriteFont.MeasureString(item);

                if (size.X > width)
                    width = size.X;

                height += spriteFont.LineSpacing + 7;
            }

            position = new Vector2(25, (gameHeight - height) - 18);
        }
        #endregion

        #region Public Accessors
        public bool activeGame()
        {
            return playGame;
        }

        public string getClassChoice()
        {
            return playerClass;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            switch (menuState)
            {
                case MenuState.MainScreen:
                    UpdateMainScreen(gameTime);
                    break;
                case MenuState.ChooseGuardian:
                    UpdateChooseGuardian(gameTime);
                    break;
                case MenuState.Instructions:
                    UpdateInstructions(gameTime);
                    break;
            }
            
            oldState = newState;
        }

        private bool validateKeys(Keys currKey)
        {
            return newState.IsKeyUp(currKey) && oldState.IsKeyDown(currKey);
        }

        #region Update Main Screen
        private void UpdateMainScreen(GameTime gameTime)
        {
            if (validateKeys(Keys.Down)) SelectedIndex++;

            if (validateKeys(Keys.Up)) SelectedIndex--;

            if (validateKeys(Keys.Enter))
            {
                switch (SelectedIndex)
                {
                    case 0:
                        menuState = MenuState.ChooseGuardian;
                        break;
                    case 1:
                        menuState = MenuState.Instructions;
                        break;
                    case 2:
                        game.Exit();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Update Choose Guardian
        private void UpdateChooseGuardian(GameTime gameTime)
        {
            if (validateKeys(Keys.Right)) SelectedIndex++;

            if (validateKeys(Keys.Left)) SelectedIndex--;

            if (validateKeys(Keys.Escape))
            {
                menuState = MenuState.MainScreen;
                SelectedIndex = 0;
            }

            if (validateKeys(Keys.Enter))
            {
                switch (SelectedIndex)
                {
                    case 0:
                        playerClass = "Warlock";
                        break;
                    case 1:
                        playerClass = "Hunter";
                        break;
                    case 2:
                        playerClass = "Titan";
                        break;
                    default:
                        break;
                }

                playGame = true;
                menuState = MenuState.MainScreen;
            }
        }
        #endregion

        #region Update Instructions
        private void UpdateInstructions(GameTime gameTime)
        {
            if (generatedLedge.Count == 0) GenerateLedge();

            // Checks state of guardian running across screen
            if (guardianPresent)
            {
                if (guardianX > gameWidth)
                {
                    guardianX = -250;
                    guardianPresent = false;
                    guardianSheet = null;
                }
                else guardianX += 2;
            }
            else
            {
                guardianTimer += gameTime.ElapsedGameTime;
                if (guardianTimer.TotalMilliseconds > 2000)
                {
                    guardianSheet = rand.Next(0, 3);
                    if (guardianSheet == lastGuardianSheet) guardianSheet = rand.Next(0, 3);    // Repeat class has a lower chance
                    lastGuardianSheet = guardianSheet;
                    guardianPresent = true;
                    guardianTimer = TimeSpan.Zero;
                }
            }       

            if (validateKeys(Keys.Escape) || validateKeys(Keys.Enter))
            {
                menuState = MenuState.MainScreen;
                SelectedIndex = 0;
            }
        }

        // Generates random ledge for instruction screen
        private void GenerateLedge()
        {
            for (int count = 0; count < ((gameWidth / 48) + 1); count++)
            {
                int index = rand.Next(0, 4);
                if (index == 0) index = rand.Next(0, 4);    // Skull ledges should be rare
                generatedLedge.Add(index);
            }
        }
        #endregion
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (menuState)
            {
                case MenuState.MainScreen:
                    DrawMainMenu(gameTime, spriteBatch);
                    break;
                case MenuState.ChooseGuardian:
                    DrawChooseGuardian(gameTime, spriteBatch);
                    break;
                case MenuState.Instructions:
                    DrawInstructions(gameTime, spriteBatch);
                    break;
            }
        }

        #region Draw Main Menu
        private void DrawMainMenu(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 location = position;

            Vector2 logoPos = new Vector2((gameWidth / 2) - (logo.Bounds.X / 2), 25);
            spriteBatch.Draw(logo, logoPos, Color.White);

            spriteBatch.DrawString(titleFont, "Darkness", new Vector2(166, 45), normal);
            spriteBatch.DrawString(titleFont, "Descent", new Vector2(125, 120), normal);

            for (int count = 0; count < menuItems.Length; count++)
            {
                if (count == SelectedIndex)
                    spriteBatch.DrawString(spriteFont, menuItems[count], new Vector2(location.X + 20, location.Y), hover);

                else
                    spriteBatch.DrawString(spriteFont, menuItems[count], location, normal);

                location.Y += spriteFont.LineSpacing + 7;
            }

            string creator = "Created by Eric Moon (MoonDawg)";
            string disclaimer = "DISCLAIMER: Destiny and all Destiny related images, characters, \nlogos, and artwork are copyrighted by Bungie Studios, Activision, \nand/or their respective owners. This is a fan-made game.";

            spriteBatch.DrawString(creatorFont, creator, new Vector2(gameWidth - 255, 5), normal);
            spriteBatch.DrawString(disclaimerFont, disclaimer, new Vector2(gameWidth - 470, gameHeight - 50), normal);
        }
        #endregion

        #region Draw Choose Guardian
        private void DrawChooseGuardian(GameTime gameTime, SpriteBatch spriteBatch)
        {
            string text = "Choose Your Guardian";
            string warlockText = "Warlock";
            string hunterText = "Hunter";
            string titanText = "Titan";

            int classCenter = (gameWidth / 2) - (int)(chooseGuardian.MeasureString(hunterText).X / 2);

            spriteBatch.DrawString(chooseGuardian, text, new Vector2((gameWidth / 2) - (chooseGuardian.MeasureString(text).X / 2), 40), hover);    // Centers text
            spriteBatch.DrawString(spriteFont, warlockText, new Vector2(classCenter - 200, 275), hover);
            spriteBatch.DrawString(spriteFont, hunterText, new Vector2(classCenter, 275), hover);
            spriteBatch.DrawString(spriteFont, titanText, new Vector2(classCenter + 210, 275), hover);

            // Draws Guardians
            spriteChange += gameTime.ElapsedGameTime;

            if (spriteChange.TotalMilliseconds > 150)
            {
                spriteChange = TimeSpan.Zero;
                if (spriteIndex < 3)
                    spriteIndex++;
                else
                    spriteIndex = 0;
            }

            int spriteCenter = (gameWidth / 2) - 52;

            switch (SelectedIndex)
            {
                case 0:
                    spriteBatch.Draw(warlockBigSheet, new Vector2(spriteCenter - 200, 150), guardianBigSpritePositions[spriteIndex], Color.White);
                    spriteBatch.Draw(hunterBigSheet, new Vector2(spriteCenter, 150), guardianBigSpritePositions[4], Color.White * 0.4f);
                    spriteBatch.Draw(titanBigSheet, new Vector2(spriteCenter + 200, 150), guardianBigSpritePositions[4], Color.White * 0.4f);
                    break;
                case 1:
                    spriteBatch.Draw(warlockBigSheet, new Vector2(spriteCenter - 200, 150), guardianBigSpritePositions[4], Color.White * 0.4f);
                    spriteBatch.Draw(hunterBigSheet, new Vector2(spriteCenter, 150), guardianBigSpritePositions[spriteIndex], Color.White);
                    spriteBatch.Draw(titanBigSheet, new Vector2(spriteCenter + 200, 150), guardianBigSpritePositions[4], Color.White * 0.4f);
                    break;
                case 2:
                    spriteBatch.Draw(warlockBigSheet, new Vector2(spriteCenter - 200, 150), guardianBigSpritePositions[4], Color.White * 0.4f);
                    spriteBatch.Draw(hunterBigSheet, new Vector2(spriteCenter, 150), guardianBigSpritePositions[4], Color.White * 0.4f);
                    spriteBatch.Draw(titanBigSheet, new Vector2(spriteCenter + 200, 150), guardianBigSpritePositions[spriteIndex], Color.White);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Draw Instructions
        private void DrawInstructions(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (guardianPresent && guardianSheet != null)
            {
                // Draws Guardians
                spriteChange += gameTime.ElapsedGameTime;

                if (spriteChange.TotalMilliseconds > 150)
                {
                    spriteChange = TimeSpan.Zero;
                    if (spriteIndex < 3)
                        spriteIndex++;
                    else
                        spriteIndex = 0;
                }

                switch (guardianSheet)
                {
                    case 0:
                        spriteBatch.Draw(warlockSheet, new Vector2(guardianX, gameHeight - 100), guardianSpritePositions[spriteIndex], Color.White);
                        break;
                    case 1:
                        spriteBatch.Draw(hunterSheet, new Vector2(guardianX, gameHeight - 100), guardianSpritePositions[spriteIndex], Color.White);
                        break;
                    case 2:
                        spriteBatch.Draw(titanSheet, new Vector2(guardianX, gameHeight - 100), guardianSpritePositions[spriteIndex], Color.White);
                        break;
                    default:
                        break;
                }
            }

            int x = 0;
            foreach (var index in generatedLedge)
            {
                spriteBatch.Draw(ledges[index], new Vector2(x, (gameHeight - 72)), Color.White);
                x += 48;
            }

            string instructionsText = "Eyes up Guardian! The Darkness is hunting \nyou down from the surface, and you must \nescape it! Descend as far as you can \nbefore you are killed!";
            string ledgeText = "Ledge = 100 Points";
            string ghostText = "Ghost = 250 Points";
            string boostText = "Boost with <Space> to traverse across gaps!";
            string backText = "Back";
            spriteBatch.DrawString(instructionsFont, instructionsText, new Vector2(10, 10), normal);
            spriteBatch.DrawString(instructionsFont, ledgeText, new Vector2(10, 175), normal);
            spriteBatch.DrawString(instructionsFont, ghostText, new Vector2(10, 210), normal);
            spriteBatch.DrawString(instructionsFont, boostText, new Vector2(10, 275), normal);
            spriteBatch.DrawString(spriteFont, backText, new Vector2(50, 335), hover);
            spriteBatch.Draw(ghost, new Vector2(368, 165), Color.White);
            spriteBatch.Draw(ledges[4], new Vector2(310, 160), Color.White);
            spriteBatch.Draw(ledges[3], new Vector2(358, 160), Color.White);
            spriteBatch.Draw(ledges[5], new Vector2(406, 160), Color.White);
        }
        #endregion
        #endregion
    }
}
