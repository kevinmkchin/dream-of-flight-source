using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ruins.Source.Level;
using ruins.Source.Screen;
using ruins.Source.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Entity {
    class Player : SpriteClass {

        private const float defaultGravity = 0.68f;
        private const float maxSpeedGround = 5.5f;
        private const float maxSpeedSwitched = 8f;
        public const float energyMax = 40;
        public float getEnergyMax() {
            return energyMax;
        }
        public float energy { get; set; } = 0;
        private float depletionRate = 0.117f;
        private float passiveDepRate = 0.07f;
        private float rechargeRate = 1f;

        private float groundAccel = 1.1f;
        private float groundDecel = 2.1f;
        private float airAccel = 2f;
        private float airDecel = 2f;
        private float maxSpeed = maxSpeedGround;
        private float Gravity = defaultGravity;
        private float JumpSpeed = -12.3f;
        private float maxFallSpeed = 12.3f;
        private float switchAccel = 0.4f;
        private float switchDecel = 0.4f;

        public bool dead { get; set; } = false;
        public bool show { get; set; } = true;
        public int deathCounter = 0;
        public int deathReset = 60;
        public bool switched { get; set; } = false;
        private Vector2 Velocity = Vector2.Zero;
        public void SetVelocity(Vector2 vel) {
            Velocity = vel;
        }
        private bool canWallJumpLeft = false;
        private bool canWallJumpRight = false;
        private bool Jumped = false;
        private bool Jumping = false;
        private double jumpGracePeriod = 0.6;
        private double jumpGraceTimer = 0.0;
        private float lastPressedJump = 10;
        private float lastPressGrace = 0.12f;

        private Texture2D rightSprite;
        private Texture2D leftSprite;
        private Texture2D switchedSprite;


        GameScreen gs;

        public static int PlayerWidth {
            get;
            set;
        } = 30;

        public static int PlayerHeight {
            get;
            set;
        } = 40;

        public Player(Texture2D sprite, int x, int y, 
            Texture2D leftSprite, Texture2D switchedSprite, 
            GameScreen gs) : base(sprite, x, y) {
            this.gs = gs;
            this.rightSprite = sprite;
            this.leftSprite = leftSprite;
            this.switchedSprite = switchedSprite;
        }

        public override void Update() {
            List<SpriteClass> collidable = gs.Collidable;

            int oldX = X;
            int oldY = Y;

            if (switched) {
                Sprite = switchedSprite;
            } else {
                if (Velocity.X > 0) {
                    Sprite = rightSprite;
                } else if (Velocity.X < 0) {
                    Sprite = leftSprite;
                }
            }

            lastPressedJump += 0.01f;


            CheckLevelFinished();
            CheckFellDown();

            if (!dead) {
                Move();
            }

            bool onGround = false;
            #region MOVEMENT AND COLLISION

            if (Math.Abs(Velocity.Y) <= maxFallSpeed)
            {
                Velocity.Y += Gravity;
            }

            bool xCollided = false;
            bool yCollided = false;
            bool canJump = false;
            canWallJumpLeft = false;
            canWallJumpRight = false;
            //check movement collision then move
            foreach (SpriteClass sprite in collidable) {

                if (IsTouching(sprite, 3) && sprite is Refuel) {
                    var refuel = (Refuel) sprite;
                    if (!refuel.used && energy < energyMax) {
                        energy = energyMax;
                        gs.screenShakeEffect(7);
                        gs.playRefuelEffect();
                        refuel.setUsed(true);
                    }
                }

                if (IsTouching(sprite, 1) && sprite is Spike) {
                    dead = true;
                }

                if (IsTouchingRight(sprite, 3) && Jumping && sprite is Wall) {
                    canWallJumpRight = true;
                }
                if (IsTouchingLeft(sprite, 3) && Jumping && sprite is Wall) {
                    canWallJumpLeft = true;
                }

                if(IsTouchingBottom(sprite, 1) && sprite is TempBlock) {
                    var temp = (TempBlock)sprite;
                    if (!temp.triggered) {
                        gs.screenShakeEffect(2);
                        gs.playBlockBreakEffect();
                    }
                    temp.triggered = true;
                }
                if(sprite is TempBlock) {
                    var tempCol = new TempBlock(sprite.Sprite, sprite.X, sprite.Y);
                    tempCol.EnableCollision(GameScreen.howBigIs16Pixels, GameScreen.howBigIs16Pixels);

                    if (!IsTouching(tempCol, 1)) {
                        var temp = (TempBlock)sprite;
                        temp.canRecover = true;
                    } else if (IsTouching(tempCol, 1)) {
                        var temp = (TempBlock)sprite;
                        temp.canRecover = false;
                    }
                }
                
                if (IsTouchingBottom(sprite, 1) && sprite is Wall) { //|| IsTouchingLeft(sprite, 1) || IsTouchingRight(sprite, 1)) {
                    if (switched) {
                        if (!Jumped) {
                            Switch();
                        }
                    } else {
                        /*
                        if (energy < 40) {
                            energy += rechargeRate;
                        }
                        */
                    }
                    onGround = true;
                    canJump = true;
                    jumpGraceTimer = 0;
                } else {
                    canJump = false;
                }

                if (lastPressedJump <= lastPressGrace) {
                    if (jumpGraceTimer <= jumpGracePeriod) { //if just a little off edge and still jumped, allow jump
                                        if (!Jumping) {
                    gs.playJumpEffect();
                }
                if (!switched) {
                    Jumped = true;
                }
                        lastPressedJump += lastPressGrace;
                        Velocity.Y = JumpSpeed;
                        Jumping = true;
                    } else {
                        if (canWallJumpLeft && lastPressedJump < 0.001f) {
                            Velocity.X = 12.3f;
                            Velocity.Y = JumpSpeed * 0.8f;
                        } else if (canWallJumpRight && lastPressedJump < 0.001f) {
                            Velocity.X = -12.3f;
                            Velocity.Y = JumpSpeed * 0.8f;
                        }
                    }
                    //if just pressed K, then jump only a little bit
                    if (switched) {
                        Velocity.Y *= 0.3f;
                    }
                }

                if (Velocity.X > 0 && IsTouchingRight(sprite, Velocity.X) && sprite is Wall)
                {
                    while (!IsTouchingRight(sprite, 1))
                    {
                        X += 1;
                    }
                    xCollided = true;
                    Velocity.X = 0;
                }
                if (Velocity.X < 0 && IsTouchingLeft(sprite, Velocity.X) && sprite is Wall)
                {
                    while (!IsTouchingLeft(sprite, 1))
                    {
                        X -= 1;
                    }
                    xCollided = true;
                    Velocity.X = 0;
                }
                if (Velocity.Y > 0 && IsTouchingBottom(sprite, Velocity.Y) && sprite is Wall) {
                    while (!IsTouchingBottom(sprite, 1)) {
                        Y += 1;
                    }
                    Jumping = false;
                    yCollided = true;
                }
                if (Velocity.Y < 0 && IsTouchingTop(sprite, Velocity.Y) && sprite is Wall) {
                    while (!IsTouchingTop(sprite, 1)) {
                        Y -= 1;
                    }
                    yCollided = true;
                }

            }
            Jumped = false;

            if (!canJump) {
                jumpGraceTimer += 0.1;
            }

            if (!xCollided) {
                X += (int)Velocity.X;
            }

            if (!yCollided) {
                Y += (int)Velocity.Y;
            } else {
                Velocity.Y = 0;
            }

            #endregion MOVEMENT AND COLLISION
            Jumping = !onGround;

            if (energy <= 0 && switched) {
                Switch();
                energy = 0;
            }

            if (switched && !dead) {
                var moveFactor = Math.Abs(Velocity.Y) + Math.Abs(Velocity.X);
                if (moveFactor > 0) {
                    if (Input.IsPressed(Keys.A) || Input.IsPressed(Keys.D)
                        || Input.IsPressed(Keys.W) || Input.IsPressed(Keys.S)) {
                        energy -= depletionRate * moveFactor;
                    } else {
                        energy -= passiveDepRate;
                    }
                } else {
                    energy -= passiveDepRate;
                }
            }

            if (dead) {
                X = oldX;
                Y = oldY;
                //show = false;
                if(deathCounter == 0) {
                    if (gs.blackAlpha != 1) {
                        gs.screenShakeEffect(20);
                    }
                    gs.playDeadEffect();
                }
                deathCounter += 1;
                if(deathCounter >= deathReset) {
                    deathCounter = 0;
                    gs.Restart();
                }
            }

        }

        private void Move() {
            Input.GetState();
            if (Input.IsPressed(Keys.D)) {
                if (!switched) {
                    if (!Jumping) {
                        if (Velocity.X <= 0) {
                            Velocity.X += groundDecel + groundAccel;
                        } else {
                            Velocity.X += groundAccel;
                        }
                    } else {
                        if (Velocity.X <= 0) {
                            Velocity.X += airDecel + airAccel;
                        } else {
                            Velocity.X += airAccel;
                        }
                    }
                } else {
                    if (Velocity.X <= 0) {
                        Velocity.X += switchDecel + switchAccel;
                    } else {
                        Velocity.X += switchAccel;
                    }
                }

                if (Velocity.X >= maxSpeed) {
                    Velocity.X = maxSpeed;
                }
            }
            if (Input.IsPressed(Keys.A)) {
                if (!switched) {
                    if (!Jumping) {
                        if (Velocity.X >= 0) {
                            Velocity.X -= groundDecel + groundAccel;
                        } else {
                            Velocity.X -= groundAccel;
                        }
                    } else {
                        if (Velocity.X >= 0) {
                            Velocity.X -= airDecel + airAccel;
                        } else {
                            Velocity.X -= airAccel;
                        }
                    }
                } else {
                    if (Velocity.X >= 0) {
                        Velocity.X -= switchDecel + switchAccel;
                    } else {
                        Velocity.X -= switchAccel;
                    }
                }

                if (Velocity.X <= -maxSpeed) {
                    Velocity.X = -maxSpeed;
                }

            }
            if (!Input.IsPressed(Keys.A) && !Input.IsPressed(Keys.D)) {
                if (!switched) {
                    if (Velocity.X > 0) {
                        if (!Jumping) {
                            Velocity.X -= groundDecel;
                        } else {
                            Velocity.X -= airDecel;
                        }
                    } else if (Velocity.X < 0) {
                        if (!Jumping) {
                            Velocity.X += groundDecel;
                        } else {
                            Velocity.X += airDecel;
                        }
                    }
                } else {
                    if (Velocity.X > 0) {
                        Velocity.X -= switchDecel;
                    } else if (Velocity.X < 0) {
                        Velocity.X += switchDecel;
                    }
                }

                if (Math.Abs(Velocity.X) <= groundDecel) {
                    Velocity.X = 0;
                }
            }
            if (Input.IsPressed(Keys.W) && switched) {
                if (Velocity.Y > 0) {
                    Velocity.Y -= switchDecel + switchAccel;
                } else {
                    Velocity.Y -= switchAccel;
                }

                if (Velocity.Y <= -maxSpeed) {
                    Velocity.Y = -maxSpeed;
                }
            }
            if (Input.IsPressed(Keys.S) && switched) {
                if (Velocity.Y < 0) {
                    Velocity.Y += switchDecel + switchAccel;
                } else {
                    Velocity.Y += switchAccel;
                }

                if (Velocity.Y >= maxSpeed) {
                    Velocity.Y = maxSpeed;
                }
            }
            if (!Input.IsPressed(Keys.W) && !Input.IsPressed(Keys.S)) {
                if (switched) {
                    if (Velocity.Y > 0) {
                        Velocity.Y -= switchDecel;
                    } else if (Velocity.Y < 0) {
                        Velocity.Y += switchDecel;
                    }

                    if (Math.Abs(Velocity.Y) <= switchDecel) {
                        Velocity.Y = 0;
                    }
                }
            }
            /*
            if (switched && (Input.IsPressed(Keys.W) || Input.IsPressed(Keys.S)
                || Input.IsPressed(Keys.A) || Input.IsPressed(Keys.D))) {
                energy -= depletionRate;
            } else if (switched) {
                energy -= passiveDepRate;
                // passive delption rate
            }
            */

            if (Input.HasBeenPressed(Keys.K)) {
                if (energy > 0) {
                    if (!switched) {
                        lastPressedJump = 0;
                        Jumped = true;
                    }
                    Switch();
                }
            }


            if (Input.HasBeenPressed(Keys.J)) {
                lastPressedJump = 0;

            }

            // variable jump height if J is released early
            if (Input.HasBeenReleased(Keys.J)) {
                if (Jumping && !switched) {
                    Velocity.Y *= 0.5f;
                }
            }

        }

        private void Switch() {
            if (switched) {
                gs.screenShakeEffect(7);
                gs.playSwitchBackEffect();
                SetSwitchNormal();
            } else {
                gs.screenShakeEffect(7);
                gs.playSwitchEffect();
                SetSwitchFlying();
            }
        }

        public void SetSwitchNormal() {
            switched = false;

            Gravity = defaultGravity;
            maxSpeed = maxSpeedGround;

            Sprite = rightSprite;
        }

        public void SetSwitchFlying() {
            if (energy >= 5) {
                switched = true;

                Gravity = 0;
                maxSpeed = maxSpeedSwitched;
            } else {
                //TODO say "not enough energy"
            }
        }


        private void CheckFellDown() {
            if (Y >= Main.GetScreenHeight()) {
                dead = true;
            }
        }


        private void CheckLevelFinished() {
            if (X + (PlayerWidth / 2) >= Main.GetScreenWidth()
                || Y + (PlayerHeight / 2) <= 0) {
                SetSwitchNormal();
                gs.roomTracker += 1;
                gs.LoadRoom(gs.roomTracker);
            }
        }

    }
}
