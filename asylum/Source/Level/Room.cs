using ruins.Source.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Level {

    class Room {

        private List<SpriteClass> tiles = new List<SpriteClass>();

        public int startX { get; set; }
        public int startY { get; set; }

        public List<SpriteClass> getTiles() {
            return tiles;
        }

        public void setTiles(List<SpriteClass> newTiles) {
            tiles = newTiles;
        }


    }
}
