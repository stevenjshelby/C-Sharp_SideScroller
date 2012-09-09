using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Testing
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //textures
        private Texture2D backgroundTexture;
        private Texture2D playerTexture;
        private Texture2D enemyTexture;
        private Texture2D[] ItemTextures = new Texture2D[2];
        private SpriteFont spriteFont;
        private Texture2D MenuArt;
        private Texture2D MenuBG;

        //some game variabbles
        private Level currentLevel;
        private Player player;
        private Camera camera;
        private readonly List<Texture2D> _blocks = new List<Texture2D>();
        private List<string> availableLevels = new List<string>();
        private int currentLevelIndex = -1;

        //screen variables
        //the screen will be 27 tiles wide and 15 tiles high
        private static int screenWidth = 864;
        private static int screenHeight = 600;
        private static Rectangle HUD;
        public static int ScreenWidth { get { return screenWidth; } }
        public static int ScreenHeight { get { return screenHeight; } }
        public static Rectangle HUDRect { get { return HUD;} }

        //For now all of the HUD elements are created, assigned, and drawn within this class.
        //Will hopefully implement a HUD class at some point and make it look nice and not just
        //white text on a black background
        public string infoText;
        public Vector2 infoPosition = new Vector2(0, 0);
        public string scoreText;
        public Vector2 scorePos;
        public string statusText;
        public Vector2 statusPos;

        //high score variables
        private Dictionary<string, int> highScores = new Dictionary<string, int>();
        private string highScoreFileName = "C:\\Users\\steven\\Desktop\\side_scroll\\C-Sharp_SideScroller\\Testing\\TestingContent\\highscores.txt";

        //The state of the game. Used throughout the program to allow all of the seperate
        //pieces of the program know what they should be doing right now
        public enum GameState
        {
            Menu,
            Loading,
            Game,
            Options,
            Paused,
            Credits
        }
        public GameState gameState;

        //timers
        private int loadTimer = 0;
        private const int loadTime = 80;
        
        //Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

            HUD = new Rectangle(0, 480, screenWidth, 120);
            infoPosition = new Vector2(HUD.X, HUD.Y);
            gameState = GameState.Menu;

            Content.RootDirectory = "Content";
        }

        //checks for text files in the Levels\ directory
        public List<string> FindAvailableLevels()
        {
            List<string> levels = new List<string>();
            foreach (string file in System.IO.Directory.GetFiles("Levels\\"))
            {
                levels.Add(file);
            }
            levels.Sort();
            return levels;
        }

        //increments level index and checks if there is another level then calls LoadLevel
        public void NextLevel()
        {
            currentLevelIndex++;
            if (currentLevelIndex >= availableLevels.Count())
            {
                //no more levels
                Exit();
                return;
            }
            LoadLevel(availableLevels[currentLevelIndex]);
        }

        //sets the currentLevel varaible to the returned value of the loadLevelFromFile method then
        //creates the player and adds it to the game objects list
        private void LoadLevel(string levelname)
        {
            currentLevel = LoadLevelFromFile(levelname);

            currentLevel.Name = levelname;

            player = new Player(currentLevel.StartPosition, new Vector2(0, 0), playerTexture);
            currentLevel.GameObjects.Add(player);
            camera.LockToObject(player);
        }

        //parse the .txt file and create a new Level object that contains all of the objects and positions
        //of the new level. returns the new Level object.
        internal Level LoadLevelFromFile(string filename)
        {
            var level = new Level();
            using (var sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s != null && (s.StartsWith("//") || s == ""))
                        continue;
                    if (s != null)
                    {
                        string[] split = s.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (split[2] == "startzone")
                        {
                            Vector2 sp = new Vector2(float.Parse(split[0]), float.Parse(split[1]));
                            level.StartPosition = sp;
                        }
                        else if (split[2] == "finishzone")
                        {
                            Vector2 fp = new Vector2(float.Parse(split[0]), float.Parse(split[1]));
                            level.FinishZone = new Rectangle((int)fp.X, (int)fp.Y, 32, 32);
                        }
                        else if (split[2] == "enemy")
                        {
                            Texture2D sprite = enemyTexture;
                            Enemy enemy = new Enemy(new Vector2(float.Parse(split[0]), float.Parse(split[1])), Vector2.Zero,
                                                    sprite);
                            level.GameObjects.Add(enemy);
                        }
                        else if (split[2] == "itembox")
                        {
                            Texture2D sprite1 = Content.Load<Texture2D>("objects/itembox");
                            Texture2D sprite2 = Content.Load<Texture2D>("objects/itembox_hit");
                            Item newItem = new Item(new Vector2(float.Parse(split[4]),
                                                                            float.Parse(split[5])),
                                                                ItemTextures[int.Parse(split[3])],
                                                                int.Parse(split[3]));
                            newItem.alive = false;
                            ItemBox itemBoxObj = new ItemBox(new Vector2(float.Parse(split[0]),
                                                                         float.Parse(split[1])),
                                                             Vector2.Zero,
                                                             sprite1,
                                                             sprite2,
                                                             GameObject.ObjectType.ItemBox,
                                                             newItem);
                            level.GameObjects.Add(itemBoxObj);
                            level.GameObjects.Add(newItem);
                        }
                        else
                        {
                            Texture2D sprite = Content.Load<Texture2D>("objects/" + split[2]);
                            GameObject gameObj = new GameObject(
                                new Vector2(float.Parse(split[0]), float.Parse(split[1])),
                                Vector2.Zero, sprite, GameObject.ObjectType.Block);
                            level.GameObjects.Add(gameObj);
                        }
                    }
                }
            }
            return level;
        }

        protected override void Initialize()
        {
            //initialize camera to screen size
            camera = new Camera(0,0,screenWidth, screenHeight-HUD.Height);

            loadHighScores();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("HUDFont");

            backgroundTexture = Content.Load<Texture2D>("bg");

            enemyTexture = Content.Load<Texture2D>("chars/enemy");
            //enemyTexture.Name = "enemy";

            playerTexture = Content.Load<Texture2D>("chars/player");
            //playerTexture.Name = "player";

            MenuArt = Content.Load<Texture2D>("titleplaceholder");
            MenuBG = Content.Load<Texture2D>("menubg");

            ItemTextures[0] = Content.Load<Texture2D>("objects/mushroom");
            ItemTextures[1] = Content.Load<Texture2D>("objects/star");

            availableLevels = FindAvailableLevels();
            NextLevel();
        }

        //The High Scores sections is minimally implemented and needs great improvement.
        //Might fix it up at some point in the future but really don't need it as this is clearly
        //not much of a functioning game
        /////////////////////////////////////////////////////////////////////////////////////////
        //checks if the players current score is a high score then adds it to the keyValuePair
        //and removes the lowest score
        protected void updateHighScores()
        {
            KeyValuePair<string, int> lowScore = new KeyValuePair<string,int>("",-1);
            foreach (KeyValuePair<string, int> pair in highScores)
            {
                if (lowScore.Value == -1)
                    lowScore = new KeyValuePair<string,int>(player.name, player.score);

                if (lowScore.Value > pair.Value)
                {
                    //prompt for player name
                    if (highScores.ContainsKey(lowScore.Key))
                        highScores.Remove(lowScore.Key);
                    highScores.Add(lowScore.Key, lowScore.Value);
                    lowScore = pair;
                    break;
                }

                if (highScores.Count() > 10)
                    highScores.Remove(lowScore.Key);
            }
            // save high scores here
            saveHighScores();
        }

        //loads the highscores from a text file and parses that file to create a 
        //keyValuePair of the high scores mapped to names
        protected void loadHighScores()
        {
            using (var sr = new StreamReader(highScoreFileName))
            {
                while (!sr.EndOfStream)
                {
                    //reads a line
                    string line = sr.ReadLine();
                    if (line != null && (line.StartsWith("//") || line == ""))
                        continue;
                    if (line != null)
                    {
                        //load in lines
                        string[] split = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        highScores.Add(split[0].ToString(), Convert.ToInt32(split[1]));
                    }
                }
            }
        }

        //saves the new highscores keyValuePair to the highscores text file
        protected void saveHighScores()
        {
            using (var sw = new StreamWriter(highScoreFileName))
            {
                foreach (KeyValuePair<string, int> pair in highScores)
                {
                    sw.WriteLine(pair.Key+", "+pair.Value);
                }
            }
        }

        private KeyboardState oldKeyboardState = Keyboard.GetState(); //needed in the update method to prevent player from holding certain keys down
        //Main update method
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            //Pause Game
            if (keyboard.IsKeyDown(Keys.P) && oldKeyboardState.IsKeyUp(Keys.P))
            {
                if (gameState == GameState.Paused)
                    gameState = GameState.Game;
                else if (gameState == GameState.Game)
                    gameState = GameState.Paused;
            }

            if (gameState == GameState.Menu)
            {
                if (keyboard.IsKeyDown(Keys.Enter))
                    gameState = GameState.Loading;
                else if(keyboard.IsKeyDown(Keys.O))
                    gameState = GameState.Options;
            }
            else if (gameState == GameState.Options)
            {
                if (keyboard.IsKeyDown(Keys.Back))
                    gameState = GameState.Menu;

                //code
            }
            else if (gameState == GameState.Game)
            {

                //Reload current level
                if (keyboard.IsKeyDown(Keys.R) && oldKeyboardState.IsKeyUp(Keys.R))
                {
                    LoadLevel(currentLevel.Name);
                    return;
                }

                //Player movement
                if (keyboard.IsKeyDown(Keys.A))
                {
                    player.Move(MovingObject.Direction.Left);
                }
                else if (keyboard.IsKeyDown(Keys.D))
                {
                    player.Move(MovingObject.Direction.Right);
                }

                //Player jump
                if (keyboard.IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyUp(Keys.Up))
                {
                    player.Jump();
                }

                foreach (GameObject gobj in currentLevel.GameObjects)
                {
                    gobj.Update(currentLevel, gameTime);
                }

                if (currentLevel.Finished)
                {
                    if (DateTime.Now >= currentLevel.LevelFinishTime.AddSeconds(3))
                    {
                        gameState = GameState.Loading;
                        NextLevel();
                    }
                }
                else
                {
                    if (player.LevelComplete(currentLevel))
                    {
                        currentLevel.Finished = true;
                        currentLevel.LevelFinishTime = DateTime.Now;

                        //high score system
                        updateHighScores();
                    }
                }
                camera.Update();
            }

            UpdateHUD();

            oldKeyboardState = keyboard;

            base.Update(gameTime);
        }

        //Games main draw method
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameState == GameState.Menu)
            {
                spriteBatch.Begin();
                    spriteBatch.Draw(MenuBG, Vector2.Zero, Color.White);
                    spriteBatch.Draw(MenuArt, new Vector2(207, 115), Color.White);
                    string str = "Press Enter";
                    Vector2 stringlen = spriteFont.MeasureString(str);
                    Vector2 stringpos = new Vector2(ScreenWidth / 2 - stringlen.X / 2, 390);
                    spriteBatch.DrawString(spriteFont, str, stringpos, Color.White);
                    str = "o = options";
                    stringlen = spriteFont.MeasureString(str);
                    stringpos = new Vector2(0, screenHeight - (HUDRect.Height + stringlen.Y));
                    spriteBatch.DrawString(spriteFont, str, stringpos, Color.White);
                spriteBatch.End();
            }
            else if (gameState == GameState.Loading || gameState == GameState.Paused)
            {
                DrawLevelSplash();
            }
            else if (gameState == GameState.Options)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(MenuBG, Vector2.Zero, Color.White);
                string str = "This is the Options Menu"+Environment.NewLine+"Press Back to Return to Menu";
                Vector2 stringlen = spriteFont.MeasureString(str);
                Vector2 stringpos = new Vector2(ScreenWidth / 2 - stringlen.X / 2, screenHeight / 2 - stringlen.Y);
                spriteBatch.DrawString(spriteFont, str, stringpos, Color.White);
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
                foreach (GameObject gobj in currentLevel.GameObjects)
                {
                    if (camera.Visible(gobj) && gobj.alive)
                    {
                        Vector2 actualPosition = gobj.Position - camera.Position;
                        //spriteBatch.Draw(gobj.sprite, actualPosition, Color.White);
                        gobj.Draw(spriteBatch, actualPosition);
                    }
                }
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, 100);
                if (currentLevel.Finished)
                {
                    string text = ("You Beat Level " + (currentLevelIndex + 1));
                    Vector2 textSize = spriteFont.MeasureString(text);
                    spriteBatch.DrawString(spriteFont, text, screenCenter - textSize / 2, Color.Black);
                }
                if (!player.alive)
                {
                    string text = "You died" + Environment.NewLine + "Press R to restart level.";
                    Vector2 textSize = spriteFont.MeasureString(text);
                    spriteBatch.DrawString(spriteFont, text, screenCenter - textSize / 2, Color.Black);
                }

                Vector2 startPosition = currentLevel.StartPosition - camera.Position;
                Vector2 finishPosition = currentLevel.FinishPosition - camera.Position;
                spriteBatch.End();

                DrawHUD();
            }
            
            base.Draw(gameTime);
        }
        
        //update HUD info
        protected void UpdateHUD()
        {
            infoText = "X= " + player.Position.X.ToString() + ", Y=" + player.Position.Y.ToString();

            // calculate position for crap
            Vector2 infoTextSize = spriteFont.MeasureString(infoText);
            scorePos = new Vector2(HUDRect.X, HUDRect.Y + infoTextSize.Y);

            scoreText = "Score: " + player.score;

            // calculate position for crap
            Vector2 scoreTextSize = spriteFont.MeasureString(scoreText);
            statusPos = new Vector2(HUDRect.X, HUDRect.Y + scoreTextSize.Y + infoTextSize.Y);

            statusText = "Status: " + player.status;
        }

        //draw HUD
        private void DrawHUD()
        {
            spriteBatch.Begin();
                //spriteBatch.DrawString(spriteFont, infoText, infoPosition, Color.White);
                spriteBatch.DrawString(spriteFont, scoreText, scorePos, Color.White);
                spriteBatch.DrawString(spriteFont, statusText, statusPos, Color.White);
            spriteBatch.End();
        }

        //Draws the pre-level splash screen you see as well as the pause menu
        private void DrawLevelSplash()
        {
            spriteBatch.Begin();
                spriteBatch.Draw(MenuBG, Vector2.Zero, Color.White);
                string levtext = "Level " + (currentLevelIndex + 1);
                Vector2 levsize = spriteFont.MeasureString(levtext);
                spriteBatch.DrawString(spriteFont, levtext, new Vector2(screenWidth / 2 - levsize.X / 2, screenHeight / 2 - levsize.Y / 2), Color.White);

            spriteBatch.End();

            if (gameState == GameState.Loading)
            {
                loadTimer++;

                if (loadTimer > loadTime)
                {
                    loadTimer = 0;
                    gameState = GameState.Game;
                }
                spriteBatch.Begin();

                    string str = "Loading...";
                    Vector2 strlen = spriteFont.MeasureString(str);
                    spriteBatch.DrawString(spriteFont, str, new Vector2(screenWidth / 2 - levsize.X / 2, screenHeight / 2 + levsize.Y / 2), Color.White);

                spriteBatch.End();
            }
            else if (gameState == GameState.Paused)
            {
                spriteBatch.Begin();

                    string str = "Paused";
                    Vector2 strlen = spriteFont.MeasureString(str);
                    spriteBatch.DrawString(spriteFont, str, new Vector2(screenWidth / 2 - levsize.X / 2, screenHeight / 2 + levsize.Y / 2), Color.White);

                spriteBatch.End();
            }
        }
    }
}
