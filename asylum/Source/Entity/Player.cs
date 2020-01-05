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

        private const float defaultGravity = 0.5f;
        private const float maxSpeedGround = 5.5f;
        private const float maxSpeedSwitched = 8f;
        public float energy { get; set; } = 40;
        private float depletionRate = 1f;
        private float passiveDepRate = 0.07f;
        private float rechargeRate = 1f;

        private float groundAccel = 0.95f;
        private float groundDecel = 2f;
        private float airAccel = 0.5f;
        private float airDecel = 0.55f;
        private float maxSpeed = maxSpeedGround;
        private float Gravity = defaultGravity;
        private int JumpSpeed = -11;
        private int maxFallSpeed = 11;
        private float switchAccel = 0.30f;
        private float switchDecel = 0.2f;

        public bool dead { get; set; } = false;
        public bool show { get; set; } = true;
        public int deathCounter = 0;
        public int deathReset = 60;
        private bool switched = false;
        private Vector2 Velocity = Vector2.Zero;
        private bool canWallJumpLeft = false;
        private bool canWallJumpRight = false;
        private bool Jumped = false;
        private bool Jumping = false;
        private double jumpGracePeriod = 0.6;
        private double jumpGraceTimer = 0.0;

        private Texture2D normalSprite;
        private Texture2D switchedSprite;


        GameScreen gs;

        public static int PlayerWidth {
            get;
            set;
        } = 32;

        public static int PlayerHeight {
            get;
            set;
        } = 40;

        public Player(Texture2D sprite, int x, int y, Texture2D switchedSprite, GameScreen gs) : base(sprite, x, y) {
            this.gs = gs;
            this.normalSprite = sprite;
            this.switchedSprite = switchedSprite;
        }

        public void Update(List<SpriteClass> collidable) {
            int oldX = X;
            int oldY = Y;

            if (switched) {
                Sprite = switchedSprite;
            } else {
                Sprite = normalSprite;
            }

            CheckLevelFinished();
            CheckFellDown();

            Move();
            #region MOVEMENT AND COLLISION

            bool xCollided = false;
            bool yCollided = false;
            bool canJump = false;
            canWallJumpLeft = false;
            canWallJumpRight = false;
            //check movement collision then move
            foreach (SpriteClass sprite in collidable) {

                if (IsTouching(sprite, 1) && sprite is Spike) {
                    dead = true;
                }

                if (IsTouchingRight(sprite, 2) && Jumping) {
                    canWallJumpRight = true;
                }
                if (IsTouchingLeft(sprite, 2) && Jumping) {
                    canWallJumpLeft = true;
                }

                if (IsTouchingBottom(sprite, 1)) { //|| IsTouchingLeft(sprite, 1) || IsTouchingRight(sprite, 1)) {
                    if (switched) {
                        if (!Jumped) {
                            Switch();
                        }
                    } else {
                        if (energy < 40) {
                            energy += rechargeRate;
                        }
                    }
                    canJump = true;
                    jumpGraceTimer = 0;
                } else {
                    canJump = false;
                }

                if (Jumped) {
                    if (jumpGraceTimer <= jumpGracePeriod) { //if just a little off edge and still jumped, allow jump
                        Velocity.Y = JumpSpeed;
                        Jumping = true;
                    } else {
                        if (canWallJumpLeft) {
                            Velocity.X = 8;
                            Velocity.Y = JumpSpeed;
                        } else if (canWallJumpRight) {
                            Velocity.X = -8;
                            Velocity.Y = JumpSpeed;
                        }
                    }
                }

                if (Velocity.Y > 0 && IsTouchingBottom(sprite, Velocity.Y)) {
                    while (!IsTouchingBottom(sprite, 1)) {
                        Y += 1;
                    }
                    Jumping = false;
                    yCollided = true;
                }
                if (Velocity.Y < 0 && IsTouchingTop(sprite, Velocity.Y)) {
                    while (!IsTouchingTop(sprite, 1)) {
                        Y -= 1;
                    }
                    yCollided = true;
                }
                if (Velocity.X > 0 && IsTouchingRight(sprite, Velocity.X)) {
                    while (!IsTouchingRight(sprite, 1)) {
                        X += 1;
                    }
                    xCollided = true;
                    Velocity.X = 0;
                }
                if (Velocity.X < 0 && IsTouchingLeft(sprite, Velocity.X)) {
                    while (!IsTouchingLeft(sprite, 1)) {
                        X -= 1;
                    }
                    xCollided = true;
                    Velocity.X = 0;
                }

            }
            Jumped = false;

            if (!canJump) {
                jumpGraceTimer += 0.1;
            }


            if (Math.Abs(Velocity.Y) <= maxFallSpeed) {
                Velocity.Y += Gravity;
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

            if (energy <= 0 && switched) {
                Switch();
                energy = 0;
            }

            if (dead) {
                X = oldX;
                Y = oldY;
                //show = false;
                if(deathCounter == 0) {
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

            if (switched && (Input.IsPressed(Keys.W) || Input.IsPressed(Keys.S)
                || Input.IsPressed(Keys.A) || Input.IsPressed(Keys.D))) {
                energy -= depletionRate;
            } else if (switched) {
                energy -= passiveDepRate;
                // passive delption rate
            }

            if (Input.HasBeenPressed(Keys.K)) {
                Jumped = true;
                Switch();
            }


            if (Input.HasBeenPressed(Keys.J)) {
                if (!Jumping) {
                    gs.playJumpEffect();
                }
                if (!switched) {
                    Jumped = true;
                }
            }

        }

        private void Switch() {
            if (switched) {
                gs.playSwitchBackEffect();
                SetSwitchNormal();
            } else {
                gs.playSwitchEffect();
                SetSwitchFlying();
            }
        }

        public void SetSwitchNormal() {
            switched = false;

            Gravity = defaultGravity;
            maxSpeed = maxSpeedGround;
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
                gs.LoadRoom(gs.roomTracker + 1);
            }
        }

    }
}
