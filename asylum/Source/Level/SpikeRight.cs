using Microsoft.Xna.Framework.Graphics;
using ruins.Source.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Level {
    class SpikeRight : Spike {

        public static String spritePath = "Sprites/spike_right";

        public static int colWidth = GameScreen.howBigIs16Pixels / 2;
        public static int colHeight = GameScreen.howBigIs16Pixels - 4;
        public static int colOffsetX = 0;
        public static int colOffsetY = 2;

        public SpikeRight(Texture2D sprite, int x, int y) : base(sprite, x, y) {
        }

    }
}
