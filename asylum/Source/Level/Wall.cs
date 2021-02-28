using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ruins.Source.Level {
    class Wall : Tile {
        public Wall(Texture2D sprite, int x, int y) : base(sprite, x, y) {
        }
    }
}
