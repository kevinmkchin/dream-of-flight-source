using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ruins.Source.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Screen {
    class MainMenu : State {

        private Texture2D snow;

        private SpriteFont bebas48;
        private SpriteFont bebas64;
        private SpriteFont bebas80;

        private SoundEffect select;

        private int selected = 0;

        private bool showMenu = false;

        public override void LoadContent(Main main, SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
            this.main = main;

            snow = main.Content.Load<Texture2D>("Sprites/snow");
            bebas48 = main.Content.Load<SpriteFont>("SpriteFonts/bebas48");
            bebas64 = main.Content.Load<SpriteFont>("SpriteFonts/bebas64");
            bebas80 = main.Content.Load<SpriteFont>("SpriteFonts/bebas80");
            select = main.Content.Load<SoundEffect>("SFX/select");

            bkg = main.Content.Load<Texture2D>("Backgrounds/main_bg");

        }

        public override void Update() {
            Input.GetState();

            if (Input.HasBeenPressed(Keys.Escape)) {
                main.Exit();
            }

            if (Input.HasBeenPressed(Keys.J)) {
                if (showMenu) {
                    switch (selected) {
                        case 0:
                            main.SetGameState(1);
                            break;
                        case 2:
                            main.Exit();
                            break;
                    }
                }
                if (!showMenu) {
                    showMenu = true;
                }
            }


            if (Input.HasBeenPressed(Keys.S)) {
                switch (selected) {
                    case 0:
                        select.Play();
                        selected = 1;
                        break;
                    case 1:
                        select.Play();
                        selected = 2;
                        break;
                }
            }
            if (Input.HasBeenPressed(Keys.W)) {
                switch (selected) {
                    case 1:
                        select.Play();
                        selected = 0;
                        break;
                    case 2:
                        select.Play();
                        selected = 1;
                        break;
                }
            }

        }

        public override void Draw() {

            Random rn = new Random();
            spriteBatch.Draw(bkg, position: Vector2.Zero);
            spriteBatch.Draw(snow, position: new Vector2(rn.Next(-100, 100), rn.Next(-50, 150)));
            spriteBatch.Draw(snow, position: new Vector2(rn.Next(400, 600), rn.Next(-50, 150)));

            Color pColor = Color.White;
            Color oColor = Color.White;
            Color qColor = Color.White;
            switch (selected) {
                case 0:
                    pColor = Color.LightGoldenrodYellow;
                    break;
                case 1:
                    oColor = Color.LightGoldenrodYellow;
                    break;
                case 2:
                    qColor = Color.LightGoldenrodYellow;
                    break;
            }

            if (showMenu) {
                spriteBatch.DrawString(bebas48, "Play", new Vector2(930, 320), pColor);
                spriteBatch.DrawString(bebas48, "Options", new Vector2(1030, 410), oColor);
                spriteBatch.DrawString(bebas48, "Quit", new Vector2(930, 500), qColor);
            } else {
                spriteBatch.DrawString(bebas48, "Press J", new Vector2(980, 360), Color.White);
            }
        }
    }
}
