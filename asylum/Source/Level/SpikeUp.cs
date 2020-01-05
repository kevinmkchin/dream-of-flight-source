using Microsoft.Xna.Framework.Graphics;
using ruins.Source.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Level {

    class SpikeUp : Spike{

        public static String spritePath = "Sprites/spike_up";

        public static int colWidth = GameScreen.howBigIs16Pixels - 2;
        public static int colHeight = GameScreen.howBigIs16Pixels / 2;
        public static int colOffsetX = 1;
        public static int colOffsetY = GameScreen.howBigIs16Pixels / 2;

        public SpikeUp(Texture2D sprite, int x, int y) : base(sprite, x, y) {
        }
    }
}
