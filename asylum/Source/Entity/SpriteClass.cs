using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Entity {
    public abstract class SpriteClass {

        protected Vector2 position;
        public Vector2 GetPosition() {
            return position;
        }
        public void SetPosition(Vector2 pos) {
            this.position = pos;
        }
        public int X { get { return (int)position.X; } set { position.X = value; } }
        public int Y { get { return (int)position.Y; } set { position.Y = value; } }
        public Texture2D Sprite { get; set; }


        public bool collisionEnabled { get; set; } = false;
        private int collisionWidth;
        private int collisionHeight;
        private int offsetX;
        private int offsetY;

        public Rectangle CollisionBox {
            get {
                if (collisionEnabled) {
                    return new Rectangle((int)X + offsetX, (int)Y + offsetY, collisionWidth, collisionHeight);
                } else {
                    Console.WriteLine("No Collision Box set up, but still requested.");
                    return new Rectangle(0, 0, 0, 0);
                }
            }
        }

        public abstract void Update();

        protected SpriteClass(Texture2D sprite, int x, int y) {
            X = x;
            Y = y;
            Sprite = sprite;
        }

        public void EnableCollision(int width, int height) {
            collisionEnabled = true;
            collisionWidth = width;
            collisionHeight = height;
            offsetX = 0;
            offsetY = 0;
        }

        public void EnableCollision(int width, int height, int offsetX, int offsetY) {
            collisionEnabled = true;
            collisionWidth = width;
            collisionHeight = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        //check if collide left IF moved by that velocity.
        public bool IsTouchingLeft(SpriteClass otherSprite, double velocity) {
            if (!otherSprite.collisionEnabled) {
                return false;
            }

            velocity = Math.Abs(velocity);

            return
                this.CollisionBox.Left - velocity < otherSprite.CollisionBox.Right && 
                this.CollisionBox.Right > otherSprite.CollisionBox.Right &&
                this.CollisionBox.Bottom > otherSprite.CollisionBox.Top &&
                this.CollisionBox.Top < otherSprite.CollisionBox.Bottom;
        }
        
        public bool IsTouchingRight(SpriteClass otherSprite, double velocity) {
            if (!otherSprite.collisionEnabled) {
                return false;
            }

            velocity = Math.Abs(velocity);

            return 
                this.CollisionBox.Right + velocity > otherSprite.CollisionBox.Left &&
                this.CollisionBox.Left < otherSprite.CollisionBox.Left &&
                this.CollisionBox.Bottom > otherSprite.CollisionBox.Top &&
                this.CollisionBox.Top < otherSprite.CollisionBox.Bottom;
        }


        public bool IsTouchingTop(SpriteClass otherSprite, double velocity) {
            if (!otherSprite.collisionEnabled) {
                return false;
            }

            velocity = Math.Abs(velocity);

            return
                this.CollisionBox.Top - velocity < otherSprite.CollisionBox.Bottom &&
                this.CollisionBox.Bottom > otherSprite.CollisionBox.Bottom &&
                this.CollisionBox.Right > otherSprite.CollisionBox.Left &&
                this.CollisionBox.Left < otherSprite.CollisionBox.Right;
        }

        public bool IsTouchingBottom(SpriteClass otherSprite, double velocity) {
            if (!otherSprite.collisionEnabled) {
                return false;
            }

            velocity = Math.Abs(velocity);

            return
                this.CollisionBox.Bottom + velocity > otherSprite.CollisionBox.Top &&
                this.CollisionBox.Top < otherSprite.CollisionBox.Top &&
                this.CollisionBox.Right > otherSprite.CollisionBox.Left &&
                this.CollisionBox.Left < otherSprite.CollisionBox.Right;
        }

        public bool IsTouching(SpriteClass otherSprite, double velocity) {
            return IsTouchingLeft(otherSprite, velocity) ||
                IsTouchingRight(otherSprite, velocity) ||
                IsTouchingTop(otherSprite, velocity) ||
                IsTouchingBottom(otherSprite, velocity);
        }

    }
}
