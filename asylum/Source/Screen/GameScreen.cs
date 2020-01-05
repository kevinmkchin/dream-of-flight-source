using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ruins.Source.Entity;
using ruins.Source.Level;
using ruins.Source.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Screen {
    class GameScreen : State {

        public static int howBigIs16Pixels = 40; //this is basically the zoom

        public int roomTracker { get; } = 1;
        private Dictionary<int, Room> rooms;
        private List<SpriteClass> tilesToDraw = new List<SpriteClass>();
        private Player player;
        private int startX = 0;
        private int startY = 0;

        //NEED LIST OF ALL COLLIDABLE OBJECTS
        List<SpriteClass> Collidable = new List<SpriteClass>();

        //textures
        private Texture2D playerSprite;
        private Texture2D playerSwitched;
        private Texture2D violet;
        private SpriteFont stocky48;
        private SpriteFont bebas32;
        private SpriteFont bebas48;
        private SpriteFont bebas64;
        private SpriteFont bebas80;

        //sfx
        private SoundEffect dead;
        private SoundEffect swit;
        private SoundEffect switBack;
        private SoundEffect jump;

        public GameScreen() {

        }

        public override void LoadContent(Main main, SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
            this.main = main;

            bkg = main.Content.Load<Texture2D>("Backgrounds/sky_bg");
            playerSprite = main.Content.Load<Texture2D>("Sprites/p_red");
            playerSwitched = main.Content.Load<Texture2D>("Sprites/p_blue");
            violet = main.Content.Load<Texture2D>("Sprites/violet");

            stocky48 = main.Content.Load<SpriteFont>("SpriteFonts/stocky48");
            bebas32 = main.Content.Load<SpriteFont>("SpriteFonts/bebas32");
            bebas48 = main.Content.Load<SpriteFont>("SpriteFonts/bebas48");
            bebas64 = main.Content.Load<SpriteFont>("SpriteFonts/bebas64");
            bebas80 = main.Content.Load<SpriteFont>("SpriteFonts/bebas80");

            dead = main.Content.Load<SoundEffect>("SFX/dead");
            swit = main.Content.Load<SoundEffect>("SFX/switch");
            switBack = main.Content.Load<SoundEffect>("SFX/switchBack");
            jump = main.Content.Load<SoundEffect>("SFX/jump");

            LevelManager lm = new LevelManager(main, howBigIs16Pixels);
            lm.LoadSprites();
            lm.LoadRooms();
            rooms = lm.GetRooms();

            player = new Player(playerSprite, 0, 0, playerSwitched, this);//TODO
            player.EnableCollision(Player.PlayerWidth - 6, Player.PlayerHeight, 3, 0);

            LoadRoom(roomTracker);
        }

        public override void Update() {
            
            player.Update(Collidable);
            
        }

        public override void Draw() {
            spriteBatch.Draw(bkg, position: Vector2.Add(Vector2.Zero, getParallaxOffset()));

            foreach (Tile t in tilesToDraw) {
                spriteBatch.Draw(t.Sprite, new Rectangle(t.X, t.Y, howBigIs16Pixels, howBigIs16Pixels), Color.White);
            }

            if (player.show) {
                spriteBatch.Draw(player.Sprite, new Rectangle(player.X, player.Y, Player.PlayerWidth, Player.PlayerHeight), Color.White);
            }

            spriteBatch.Draw(violet, new Rectangle(640 + ((int)player.energy * -18) / 2, 685, 
                (int) player.energy * 18, 25), Color.White);


            if (player.dead) {
                showDead();
            }


        } 

        public void LoadRoom(int roomNumber) {
            Room r = rooms[roomNumber];
            startX = r.startX;
            startY = r.startY;
            tilesToDraw = r.getTiles();
            Collidable = tilesToDraw;
            player.X = startX;
            player.Y = startY;
            player.SetSwitchNormal();
        }

        public void Restart() {
            player.show = true; 
            player.dead = false;
            player.deathCounter = 0;
            player.SetPosition(new Vector2(startX, startY));
        }

        private void showDead() {
            Random rn = new Random();
            spriteBatch.DrawString(bebas80, "you died", new Vector2(475 + rn.Next(-3, 3), 300 + rn.Next(-3, 3)), Color.Violet);
        }

        public void playDeadEffect() {
            dead.Play();
        }
        public void playSwitchEffect() {
            swit.Play();
        }
        public void playSwitchBackEffect() {
            switBack.Play();
        }
        public void playJumpEffect() {
            jump.Play();
        }

        private Vector2 getParallaxOffset() {
            int cw = Main.GetScreenWidth() / 2;
            int ch = Main.GetScreenHeight() / 2;

            int vx = player.X - cw;
            int vy = player.Y - ch;

            double rx = (double)vx / cw;
            double ry = (double)vy / ch;

            /*
            if (rx >= 0) {
                rx = 1 - rx;
            } else {
                rx = -1 - rx;
            }

            if (ry >= 0) {
                ry = 1 - ry;
            } else {
                ry = -1 - ry;
            }
            */

            double ox = Math.Pow(rx, 3) * 100;
            double oy = Math.Pow(ry, 3) * 100;
            
            ox = Math.Min(ox, 80);
            ox = Math.Max(ox, -80);
            oy = Math.Min(oy, 80);
            oy = Math.Max(oy, -80);

            return new Vector2((float) ox - 100, (float) oy - 100);
        }

    }
}
