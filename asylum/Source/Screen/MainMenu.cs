using Microsoft.Xna.Framework;
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

        public override void LoadContent(Main main, SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
            this.main = main;

            bkg = main.Content.Load<Texture2D>("Backgrounds/mainmenu_bg");

        }

        public override void Update() {
            Input.GetState();
            

            //TODO delete this 
            if (Input.HasBeenPressed(Keys.Enter)) {
                Console.WriteLine("enter");
            }

            if (Input.HasBeenPressed(Keys.Space)) {
                main.SetGameState(1);
            }

        }

        public override void Draw() {
            spriteBatch.Draw(bkg, position: Vector2.Zero);
        }

    }
}
