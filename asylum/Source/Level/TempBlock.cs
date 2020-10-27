using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ruins.Source.Level {
    class TempBlock : Wall {

        public bool triggered { get; set; } = false;
        public bool canRecover { get; set; } = false;
        public static int timerDefault = 60;
        public int timer { get; set; } = timerDefault; // in frames

        public override void Update() {
            if (triggered) {
                timer -= 1;
            }

            if (timer <= 0) {
                collisionEnabled = false;
            }

            if(timer <= -140) {
                if (canRecover) {
                    triggered = false;
                    collisionEnabled = true;
                    timer = timerDefault;
                }
            }
        }

        public TempBlock(Texture2D sprite, int x, int y) : base(sprite, x, y) {

        }

    }
}
