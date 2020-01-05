using Microsoft.Xna.Framework.Graphics;
using ruins.Source.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Level {
    class SpikeLeft : Spike{

        public static String spritePath = "Sprites/spike_left";

        public static int colWidth = GameScreen.howBigIs16Pixels / 2;
        public static int colHeight = GameScreen.howBigIs16Pixels - 2;
        public static int colOffsetX = 0;
        public static int colOffsetY = 1;

        public SpikeLeft(Texture2D sprite, int x, int y) : base(sprite, x, y) {
        }

    }
}
