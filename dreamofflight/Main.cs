using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ruins.Source;
using ruins.Source.Entity;
using ruins.Source.Level;
using ruins.Source.Screen;
using ruins.Source.Tool;
using System;
using System.Linq;

namespace ruins {

    public class Main : Game {

        public static Random Random;

        public float shakeRadius { get; set; } = 0;
        public float shakeAngle { get; set; } = 0;

        //TODO change this to a property
        private int GameState; //0 = mainmenu, 1 = gamescreen
        public int GetGameState() {
            return GameState;
        }
        public void SetGameState(int gs) {
            GameState = gs;
        }

        public State Screen {
            get;
            set;
        }
        private MainMenu mainMenu;
        private GameScreen gameScreen;

        static GraphicsDeviceManager graphics;
        SpriteBatch mainBatch;

        private Song music1;
        private Song music2;

        public Main() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {

            this.Window.Title = "dream of flight";

            mainMenu = new MainMenu();
            gameScreen = new GameScreen();
            GameState = 0;
            Screen = mainMenu;




            base.Initialize();
        }


        protected override void LoadContent() {
            mainBatch = new SpriteBatch(GraphicsDevice);

            music1 = Content.Load<Song>("Music/music1");
            music2 = Content.Load<Song>("Music/music2");

            MediaPlayer.Play(music1);

            mainMenu.LoadContent(this, mainBatch);
            gameScreen.LoadContent(this, mainBatch);

            
        }
        
        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {

            if(MediaPlayer.PlayPosition.TotalSeconds == 0.0f && MediaPlayer.State != MediaState.Playing) {
                MediaPlayer.Play(music2);
                MediaPlayer.IsRepeating = true;
            }
            
            if (GameState == 0 && !(Screen is MainMenu)) {
                Screen = mainMenu;
            } else if(GameState == 1 && !(Screen is GameScreen)) {
                Screen = gameScreen;
            }
            Screen.Update();
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Turquoise);

            if (Screen is GameScreen) {

                Random = new Random();
                Vector2 offset = new Vector2(0, 0);
                offset = new Vector2((float)(Math.Sin(shakeAngle) * shakeRadius), 
                    (float)(Math.Cos(shakeAngle) * shakeRadius));
                shakeRadius *= 0.9f;
                shakeAngle += Random.Next(-60, 60);
                if (shakeRadius <= 0.5f) {
                    shakeRadius = 0;
                }

                //PointWrap makes scaling sprites pixel perfect.
                mainBatch.Begin(samplerState: SamplerState.PointWrap, 
                    transformMatrix: Matrix.CreateTranslation(offset.X,offset.Y,0));
            } else {
                mainBatch.Begin();
            }

            Screen.Draw();


            mainBatch.End();
            base.Draw(gameTime);
        }


        public static int GetScreenWidth() {
            return graphics.PreferredBackBufferWidth;
        }
        public static int GetScreenHeight() {
            return graphics.PreferredBackBufferHeight;
        }

    }

    public static class Program {
        static void Main() {
            using (var game = new Main())
                game.Run();
        }
    }

}
