using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ruins.Source.Level {
    class Refuel : Tile {

        private Texture2D defaultSprite;
        private Texture2D usedSprite;
        private int usedCounter = 0;
        private int coolDown = 200;

        public bool used { get; set; } = false;

        public Refuel(Texture2D sprite, int x, int y, Texture2D usedSprite) : base(sprite, x, y) {
            this.defaultSprite = sprite;
            this.usedSprite = usedSprite;
        }

        public override void Update() {
            if (used) {
                usedCounter++;
            }

            if(usedCounter >= 300) {
                usedCounter = 0;
                setUsed(false);
            }
        }

        public void setUsed(bool u) {
            if (u) {
                used = true;
                Sprite = usedSprite;
            } else {
                used = false;
                Sprite = defaultSprite;
            }
        }

    }
}
