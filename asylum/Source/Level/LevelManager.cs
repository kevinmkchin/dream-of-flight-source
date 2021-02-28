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
        private Texture2D r_default;
        private Texture2D r_used;
        private List<Texture2D> NormalWalls = new List<Texture2D>();
        private List<Texture2D> SpecialWalls = new List<Texture2D>();

        public LevelManager(Main main, int howBigIs16Pixels) {
            this.main = main;
            this.howBigIs16Pixels = howBigIs16Pixels;
        }

        /// <summary>
        /// Load level sprites here.
        /// </summary>
        public void LoadSprites() {

            for(int i = 1; i < 17; i++) {
                string str1 = "Normal (" + i + ")";
                string str2 = "Special (" + i + ")";
                Texture2D t1 = main.Content.Load<Texture2D>("Sprites/Normal/" + str1);
                Texture2D t2 = main.Content.Load<Texture2D>("Sprites/Special/" + str2);
                NormalWalls.Add(t1);
                SpecialWalls.Add(t2);
            }

            wallTest = main.Content.Load<Texture2D>("Sprites/yolo");
            spikeUp = main.Content.Load<Texture2D>(SpikeUp.spritePath);
            spikeDown = main.Content.Load<Texture2D>(SpikeDown.spritePath);
            spikeLeft = main.Content.Load<Texture2D>(SpikeLeft.spritePath);
            spikeRight = main.Content.Load<Texture2D>(SpikeRight.spritePath);
            r_default = main.Content.Load<Texture2D>("Sprites/refuel_default");
            r_used = main.Content.Load<Texture2D>("Sprites/refuel_used");

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
                fileName = fileName.Substring(0, fileName.Length - 5);
                int key = Int32.Parse(fileName.Substring(3));
                rooms.Add(key, r);

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
                    newTile = new Wall(NormalWalls[2], j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(howBigIs16Pixels, howBigIs16Pixels);
                    break;
                case 'B':
                    int index = rng.Next(0, 16);
                    newTile = new Wall(NormalWalls[index], j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(howBigIs16Pixels, howBigIs16Pixels);
                    break;
                case 'X':
                    room.startX = j * howBigIs16Pixels;
                    room.startY = i * howBigIs16Pixels + (Player.PlayerHeight - howBigIs16Pixels);
                    break;
                case 'F':
                    newTile = new Refuel(r_default, j * howBigIs16Pixels, i * howBigIs16Pixels, r_used);
                    newTile.EnableCollision(howBigIs16Pixels, howBigIs16Pixels);
                    break;
                case 'G':
                    newTile = new TempBlock(SpecialWalls[0], j * howBigIs16Pixels, i * howBigIs16Pixels);
                    newTile.EnableCollision(howBigIs16Pixels, howBigIs16Pixels);
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
