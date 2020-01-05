using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ruins.Source;
using ruins.Source.Entity;
using ruins.Source.Level;
using ruins.Source.Screen;
using ruins.Source.Tool;
using System.Linq;

namespace ruins {

    public class Main : Game {

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

            mainMenu = new MainMenu();
            gameScreen = new GameScreen();
            GameState = 0;
            Screen = mainMenu;




            base.Initialize();
        }


        protected override void LoadContent() {
            mainBatch = new SpriteBatch(GraphicsDevice);
            mainMenu.LoadContent(this, mainBatch);
            gameScreen.LoadContent(this, mainBatch);

            
        }
        
        protected override void UnloadContent() {

        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (GameState == 0 && !(Screen is MainMenu)) {
                Screen = mainMenu;
            } else if(GameState == 1 && !(Screen is GameScreen)) {
                Screen = gameScreen;
            }
            Screen.Update();
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Beige);  //(Color.CornflowerBlue);

            if (Screen is GameScreen) {
                //PointWrap makes scaling sprites pixel perfect.
                mainBatch.Begin(samplerState: SamplerState.PointWrap);
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
