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

        public int roomTracker { get; set; } = 1; //1;
        private Dictionary<int, Room> rooms;
        private List<SpriteClass> tilesToDraw = new List<SpriteClass>(); //MAKE THIS ALL ENTITIES
        private Player player;
        private int startX = 0;
        private int startY = 0;

        public float blackAlpha { get; set; } = 1;
        private int blackCounter = 0;

        //NEED LIST OF ALL COLLIDABLE OBJECTS
        public List<SpriteClass> Collidable { get; set; } = new List<SpriteClass>();

        //textures
        private Texture2D playerRight;
        private Texture2D playerLeft;
        private Texture2D playerSwitched;
        private Texture2D violet;
        private Texture2D black;
        private Texture2D cyan;
        private SpriteFont stocky48;
        private SpriteFont bebas18;
        private SpriteFont bebas32;
        private SpriteFont bebas48;
        private SpriteFont bebas64;
        private SpriteFont bebas80;
        private SpriteFont century48;

        //sfx
        private SoundEffect dead;
        private SoundEffect swit;
        private SoundEffect switBack;
        private SoundEffect jump;
        private SoundEffect refuel;
        private SoundEffect block_break;
        private double blockBreakCounter = 0;
        private bool blockBreakPlayed = false;

        public GameScreen() {

        }

        public override void LoadContent(Main main, SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
            this.main = main;

            bkg = main.Content.Load<Texture2D>("Backgrounds/sky_bg");
            playerRight = main.Content.Load<Texture2D>("Sprites/p_right");
            playerLeft = main.Content.Load<Texture2D>("Sprites/p_left");
            playerSwitched = main.Content.Load<Texture2D>("Sprites/p_switched");
            violet = main.Content.Load<Texture2D>("Sprites/violet");
            black = main.Content.Load<Texture2D>("Sprites/black");
            cyan = main.Content.Load<Texture2D>("Sprites/cyan");

            stocky48 = main.Content.Load<SpriteFont>("SpriteFonts/stocky48");
            bebas18 = main.Content.Load<SpriteFont>("SpriteFonts/bebas18");
            bebas32 = main.Content.Load<SpriteFont>("SpriteFonts/bebas32");
            bebas48 = main.Content.Load<SpriteFont>("SpriteFonts/bebas48");
            bebas64 = main.Content.Load<SpriteFont>("SpriteFonts/bebas64");
            bebas80 = main.Content.Load<SpriteFont>("SpriteFonts/bebas80");
            century48 = main.Content.Load<SpriteFont>("SpriteFonts/18century48");
            

            dead = main.Content.Load<SoundEffect>("SFX/dead");
            swit = main.Content.Load<SoundEffect>("SFX/switch");
            switBack = main.Content.Load<SoundEffect>("SFX/switchBack");
            jump = main.Content.Load<SoundEffect>("SFX/jump");
            refuel = main.Content.Load<SoundEffect>("SFX/refuel");
            block_break = main.Content.Load<SoundEffect>("SFX/block_break");

            LevelManager lm = new LevelManager(main, howBigIs16Pixels);
            lm.LoadSprites();
            lm.LoadRooms();
            rooms = lm.GetRooms();
            
            player = new Player(playerRight, 0, 0, playerLeft, playerSwitched, this);
            player.EnableCollision(Player.PlayerWidth - 2, Player.PlayerHeight, 1, 0);

            LoadRoom(roomTracker);
            if (!tilesToDraw.Contains(player)) {
                tilesToDraw.Add(player);
            }

            blackAlpha = 1f;
            blackCounter = 0;
        }

        public override void Update() {
            if (blackCounter >= 120) {
                blackAlpha -= 0.015f;
            } else {
                player.dead = true;
                blackCounter += 1;
            }

            blockBreakCounter += 17;

            foreach(SpriteClass s in tilesToDraw) {
                s.Update();
            }
           
            
        }

        public override void Draw() {
            spriteBatch.Draw(bkg, position: Vector2.Add(Vector2.Zero, getParallaxOffset()));

            foreach (SpriteClass t in tilesToDraw) {
                if (t is Player) {
                    if (player.show) {
                        float r;
                        if (player.switched) {
                            r = (float)player.energy / player.getEnergyMax();
                        } else {
                            r = 1;
                        }
                        spriteBatch.Draw(player.Sprite, new Rectangle(player.X, player.Y, Player.PlayerWidth, Player.PlayerHeight),
                            Color.White * r);
                    }
                } else if (t is TempBlock) {
                    var temp = (TempBlock)t;
                    float r = (float)temp.timer / TempBlock.timerDefault;
                    spriteBatch.Draw(t.Sprite, new Rectangle(t.X, t.Y, howBigIs16Pixels, howBigIs16Pixels), 
                        Color.White * r);
                } else {
                    spriteBatch.Draw(t.Sprite, new Rectangle(t.X, t.Y, howBigIs16Pixels, howBigIs16Pixels), Color.White);
                }
            }

            spriteBatch.Draw(black, new Rectangle(275, 680, 730, 35), Color.White);
            spriteBatch.Draw(cyan, new Rectangle(640 + ((int)player.energy * -18) / 2, 685, 
                (int) player.energy * 18, 25), Color.White);
            spriteBatch.DrawString(bebas18, "Energy", new Vector2(616, 682), Color.White);

            if(roomTracker == 1) {
                spriteBatch.DrawString(bebas32, "W A S D & J", new Vector2(464,375), Color.White);
            }
            if(roomTracker == 8) {
                spriteBatch.DrawString(bebas32, "K - FLY", new Vector2(70, 610), Color.White);
            }

            if (player.dead) {
                showDead();
            }

            
            //fade screen
            if(blackAlpha >= 0) {
                spriteBatch.Draw(black, new Rectangle(0, 0, Main.GetScreenWidth(), Main.GetScreenHeight()),
                    Color.White * blackAlpha);
                string roomText = "";
                switch (roomTracker) {
                    case 1:
                        roomText = "wake up...";
                        break;
                    case 2:
                        roomText = "where are you?";
                        break;
                    case 3:
                        roomText = "you are not ready...";
                        break;
                    case 4:
                        roomText = "come back...";
                        break;
                    case 5:
                        roomText = "it'll all be worth it";
                        break;
                    case 6:
                        roomText = "don't be afraid";
                        break;
                    case 7:
                        roomText = "i'm not going to hurt you...";
                        break;
                    case 8:
                        roomText = "don't. fly.";
                        break;
                    case 9:
                        roomText = "stop";
                        break;
                    case 10:
                        roomText = "why won't you listen to me";
                        break;
                    case 11:
                        roomText = "you are full of determination, aren't you?";
                        break;
                    case 12:
                        roomText = "i'm just trying to help you...";
                        break;

                }
                spriteBatch.DrawString(century48, roomText, new Vector2(100, 100),
                            Color.White * blackAlpha);
            }

        } 

        public void LoadRoom(int roomNumber) {
            main.shakeRadius = 0;

            blackCounter = 60;
            blackAlpha = 1f;
            Room r;
            try {
                r = rooms[roomNumber];
            } catch(KeyNotFoundException e) {
                Console.WriteLine(e);
                r = rooms[1];
            }
            startX = r.startX;
            startY = r.startY;
            tilesToDraw = r.getTiles();
            Collidable = tilesToDraw;
            player.X = startX;
            player.Y = startY;
            player.SetVelocity(Vector2.Zero);
            player.SetSwitchNormal();
            player.energy = 0;

            if (!tilesToDraw.Contains(player)) {
                tilesToDraw.Add(player);
            }
        }

        public void Restart() {
            foreach(SpriteClass t in tilesToDraw) {
                if (t is Refuel) {
                    var r = (Refuel)t;
                    r.setUsed(false);
                }
                if (t is TempBlock) {
                    var temp = (TempBlock)t;
                    temp.triggered = false;
                    temp.collisionEnabled = true;
                    temp.canRecover = false;
                    temp.timer = TempBlock.timerDefault;
                }
            }
            player.show = true; 
            player.dead = false;
            player.deathCounter = 0;
            player.energy = 0;
            player.SetPosition(new Vector2(startX, startY));
            player.SetSwitchNormal();
            player.SetVelocity(Vector2.Zero);
        }

        private void showDead() {
            if (blackAlpha <= 0.01f) {
                Random rn = new Random();
                spriteBatch.DrawString(bebas80, "you died", new Vector2(475 + rn.Next(-3, 3), 300 + rn.Next(-3, 3)), 
                    Color.White);
            }
        }

        public void playDeadEffect() {
            if (blackCounter >= 100) {
                dead.Play();
            }
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
        public void playRefuelEffect() {
            refuel.Play();
        }
        public void playBlockBreakEffect() {
            if (blockBreakCounter >= block_break.Duration.Milliseconds * 0.1) {
                blockBreakCounter = 0;
                blockBreakPlayed = false;
            }
            if (!blockBreakPlayed) {
                block_break.Play();
                blockBreakPlayed = true;
            }
        }

        public void screenShakeEffect(float radius) {
            Random rn = new Random();
            main.shakeRadius = radius;
            main.shakeAngle = rn.Next(360);
        }

        private Vector2 getParallaxOffset() {
            int cw = Main.GetScreenWidth() / 2;
            int ch = Main.GetScreenHeight() / 2;

            int vx = -(cw + player.X);
            int vy = -(ch + player.Y);

            vx /= 10;
            vy /= 10;

            return new Vector2(vx, vy);

            /*
            if(vx < 0) {
                vx += Main.GetScreenWidth();
            }
            if(vy < 0) {
                vy += Main.GetScreenHeight();
            }
            */



            /*
            double rx = (double)vx / cw;
            double ry = (double)vy / ch;

            
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
            

            double ox = Math.Pow(rx, 3) * 100;
            double oy = Math.Pow(ry, 3) * 100;
            
            ox = Math.Min(ox, 80);
            ox = Math.Max(ox, -80);
            oy = Math.Min(oy, 30);
            oy = Math.Max(oy, -30);

            return new Vector2((float) ox - 100, (float) oy - 80);
            */
        }

    }
}
