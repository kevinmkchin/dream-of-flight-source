using Microsoft.Xna.Framework.Graphics;
using ruins.Source.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Level {
    class LevelManager {

        Random rng = new Random();
        private int howBigIs16Pixels;
        private int roomWidth = 32; // 1280 / 40 = 32
        private int roomHeight = 18; // 720 / 40 = 18

        private Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        private Main main;

        //maybe store sprites in a dictionary
        private Texture2D wallTest;
        private Texture2D spikeUp;
        private Texture2D spikeDown;
        private Texture2D spikeLeft;
        private Texture2D spikeRight;

        public LevelManager(Main main, int howBigIs16Pixels) {
            this.main = main;
            this.howBigIs16Pixels = howBigIs16Pixels;
        }

        /// <summary>
        /// Load level sprites here.
        /// </summary>
        public void LoadSprites() {

            wallTest = main.Content.Load<Texture2D>("Sprites/cloud");
            spikeUp = main.Content.Load<Texture2D>(SpikeUp.spritePath);
            spikeDown = main.Content.Load<Texture2D>(SpikeDown.spritePath);
            spikeLeft = main.Content.Load<Texture2D>(SpikeLeft.spritePath);
            spikeRight = main.Content.Load<Texture2D>(SpikeRight.spritePath);

        }

        public void LoadRooms() {
            string sourceDirectory = Environment.CurrentDirectory;
            string roomDirectory = sourceDirectory + "/Rooms";

            string[] filePaths = Directory.GetFiles(roomDirectory);

            foreach (string str in filePaths) {
                var sr = new StreamReader(str);
                string roomData = sr.ReadToEnd();
                roomData = roomData.Replace(".", "");
                char[,] mapArray = new char[roomHeight, roomWidth];
                for (int i = 0; i < roomHeight; i++) {
                    for (int j = 0; j < roomWidth; j++) {
                        mapArray[i, j] = roomData[i * roomWidth + j];
                    }
                }

                Room r = MakeRoomAndTiles(mapArray);
                var fileName = str.Substring(roomDirectory.Length + 1);
                int key = Int32.Parse(fileName.Substring(3, 1));

                rooms.Add(key, r);
                
                //Console.WriteLine(fileName.Substring(3,1).ToString());

            }
        }
        
        private Room MakeRoomAndTiles(char[,] level) {

            Room r = new Room();

            int height = level.GetLength(0);
            int width = level.GetLength(1);
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    AddNewTile(level[i, j], i, j, r);
                }
            }

            return r;

        }
        
        private void AddNewTile(char c, int i, int j, Room room) {
            Tile newTile = null;

            //TODO diff tiles for diff chars
            switch (c) {
                case '0':
                    break;
                case 'A':
                    newTile = new Tile(wallTest, j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(howBigIs16Pixels, howBigIs16Pixels);
                    break;
                case 'X':
                    room.startX = j * howBigIs16Pixels;
                    room.startY = i * howBigIs16Pixels + (Player.PlayerHeight - howBigIs16Pixels);
                    break;
                case 'H':
                    newTile = new SpikeUp(spikeUp, j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(SpikeUp.colWidth, SpikeUp.colHeight, SpikeUp.colOffsetX, SpikeUp.colOffsetY);
                    break;
                case 'I':
                    newTile = new SpikeDown(spikeDown, j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(SpikeDown.colWidth, SpikeDown.colHeight, SpikeDown.colOffsetX, SpikeDown.colOffsetY);
                    break;
                case 'J':
                    newTile = new SpikeLeft(spikeLeft, j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(SpikeLeft.colWidth, SpikeLeft.colHeight, SpikeLeft.colOffsetX, SpikeLeft.colOffsetY);
                    break;
                case 'K':
                    newTile = new SpikeRight(spikeRight, j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(SpikeRight.colWidth, SpikeRight.colHeight, SpikeRight.colOffsetX, SpikeRight.colOffsetY);
                    break;
            }

            if (newTile != null) {
                room.getTiles().Add(newTile);
            }
        }

        public Dictionary<int, Room> GetRooms() {
            return rooms;
        }

    }
}
