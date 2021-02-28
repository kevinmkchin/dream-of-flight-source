using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ruins.Source.Level;
using System.Linq;

namespace ruins.Source {
    public abstract class State {

        protected Texture2D bkg;
        protected Main main;
        protected SpriteBatch spriteBatch;

        /// <summary>
        /// Called once at the start of game.
        /// Called when graphics resources need to be loaded. 
        /// Override this method to load any component-specific resources (levels, etc.)
        /// Stores an instance of the Main class and the main SpriteBatch.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="spriteBatch"></param>
        public abstract void LoadContent(Main main, SpriteBatch spriteBatch);

        /// <summary>
        /// Called every frame (or tick) of the game.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draw to the SpriteBatch.
        /// Don't need to begin or end SpriteBatch because that is done in the Main class.
        /// </summary>
        public abstract void Draw();

    }
}
