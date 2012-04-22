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

        //the sprites will be 32x32
        private Texture2D backgroundTexture;
        private Texture2D playerTexture;
        private Texture2D enemyTexture;
        private SpriteFont spriteFont;
        private Rectangle startRect;
        private Rectangle FinishRect;
        private Level currentLevel;
        private Player player;
        private Camera camera;
        private readonly List<Texture2D> _blocks = new List<Texture2D>();
        private List<string> availableLevels = new List<string>();
        private int currentLevelIndex = -1;

        //the screen will be 27 tiles wide and 15 tiles high
        private static int screenWidth = 864;
        private static int screenHeight = 600;
        private static Rectangle HUD;
        public static int ScreenWidth { get { return screenWidth; } }
        public static int ScreenHeight { get { return screenHeight; } }
        public static Rectangle HUDRect { get { return HUD;} }

        public static string infoText = "Crap";
        public Vector2 infoPosition = new Vector2(0, 0);

        public string scoreText = "";
        public Vector2 scorePos = new Vector2(0, 0);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

            //HUD rectangle creation
            HUD = new Rectangle(0, 480, screenWidth, 120);

            infoPosition = new Vector2(HUD.X, HUD.Y);

            Content.RootDirectory = "Content";
        }

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
                        else
                        {
                            var sprite = Content.Load<Texture2D>("objects/" + split[2]);
                            sprite.Name = split[2];
                            var gameObj = new GameObject(
                                new Vector2(float.Parse(split[0]), float.Parse(split[1])),
                                new Vector2(0, 0), sprite, GameObject.ObjectType.Block);
                            level.GameObjects.Add(gameObj);
                        }
                    }
                }
            }
            return level;
            //first line = X
            //second line = Y
            //third line = spritename
        }

        protected override void Initialize()
        {
            //initialize camera to screen size
            camera = new Camera(0,0,screenWidth, screenHeight-HUD.Height);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("HUDFont");

            backgroundTexture = Content.Load<Texture2D>("bg");

            enemyTexture = Content.Load<Texture2D>("chars/enemy");
            enemyTexture.Name = "enemy";

            playerTexture = Content.Load<Texture2D>("chars/player");
            playerTexture.Name = "player";

            availableLevels = FindAvailableLevels();
            NextLevel();
        }

        private void LoadLevel(string levelname)
        {
            currentLevel = LoadLevelFromFile(levelname);

            currentLevel.Name = levelname;

            player = new Player(currentLevel.StartPosition, new Vector2(0, 0), playerTexture);
            currentLevel.GameObjects.Add(player);
            camera.LockToObject(player);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void UpdateHUD() {
            infoText = "X= " + player.Position.X.ToString() + ", Y=" + player.Position.Y.ToString();
            scoreText = "Score: " + player.score;

            // calculate positions for crap
            Vector2 infoTextSize = spriteFont.MeasureString(infoText);
            scorePos = new Vector2(HUDRect.X, HUDRect.Y+infoTextSize.Y);
        }

        private KeyboardState oldKeyboardState = Keyboard.GetState();

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyboard.IsKeyDown(Keys.R) && oldKeyboardState.IsKeyUp(Keys.R))
            {
                LoadLevel(currentLevel.Name);
                return;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                player.Move(MovingObject.Direction.Left);
            }
            else if (keyboard.IsKeyDown(Keys.D))
            {
                player.Move(MovingObject.Direction.Right);
            }

            if (keyboard.IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyUp(Keys.Up))
            {
                player.Jump();
            }

            foreach (GameObject gobj in currentLevel.GameObjects)
            {
                gobj.Update(currentLevel);
            }

            if (currentLevel.Finished)
            {
                if (DateTime.Now >= currentLevel.LevelFinishTime.AddSeconds(3))
                {
                    NextLevel();
                }
            }
            else
            {
                if (player.LevelComplete(currentLevel))
                {
                    currentLevel.Finished = true;
                    currentLevel.LevelFinishTime = DateTime.Now;
                }
            }


            //implement HUD update here

            this.UpdateHUD();

            camera.Update();

            oldKeyboardState = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
                spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
                foreach (GameObject gobj in currentLevel.GameObjects)
                {
                    if (camera.Visible(gobj) && gobj.alive)
                    {
                        Vector2 actualPosition = gobj.Position - camera.Position;
                        spriteBatch.Draw(gobj.sprite, actualPosition, Color.White);
                    }
                } 
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, 100);
                if (currentLevel.Finished)
                {
                    const string text = "YOU WIN!";
                    Vector2 textSize = spriteFont.MeasureString(text);
                    spriteBatch.DrawString(spriteFont, text, screenCenter - textSize / 2, Color.Black);
                }
                if (!player.alive)
                {
                    string text = "You died" + Environment.NewLine + "Press R to restart.";
                    Vector2 textSize = spriteFont.MeasureString(text);
                    spriteBatch.DrawString(spriteFont, text, screenCenter - textSize / 2, Color.Black);
                }
                
                //draw HUD
                spriteBatch.DrawString(spriteFont, infoText, infoPosition, Color.White);
                spriteBatch.DrawString(spriteFont, scoreText, scorePos, Color.White);


                Vector2 startPosition = currentLevel.StartPosition - camera.Position;
                Vector2 finishPosition = currentLevel.FinishPosition - camera.Position;
                spriteBatch.End();

                base.Draw(gameTime);
        }
    }
}
