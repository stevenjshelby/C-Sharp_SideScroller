using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Testing
{
    class Player : MovingObject
    {
        private bool canJump = true;
        public int score = 0;
        public string name = "Steven";
        public bool hurt = false;

        public int hurtTimer = 0;
        public const int hurtTime = 60;

        public int invincibleTimer = 0;
        public const int invincibleTime = 520;

        public enum Status
        {
            Small,
            Big,
            Invincible
        }
        public Status status;
        private Status previousStatus; //used for invincibility

        //Constructor
        public Player(Vector2 position, Vector2 velocity, Texture2D sprite)
            : base(position, velocity, sprite, ObjectType.Player)
        {
            status = Status.Small;
            previousStatus = status;
        }

        public void Jump()
        {
            if (alive && canJump)
            {
                velocity.Y += -(float)Math.Cos(Math.PI/4) * 8;
                canJump = false;
            }
        }

        public override void ReachedBottom()
        {
            base.ReachedBottom();
            canJump = true;
        }

        public override void DetectEnemyHit(MovingObject mob)
        {
            if (status == Status.Invincible)
                return;

            //if collision with enemy
            if (Position.Y + Height - 8 <= mob.Position.Y + 16)
            {
                //kill enemy if coming from above
                if (velocity.Y > 0)
                {
                    mob.Die();
                    score += 1;
                    velocity.Y = Math.Min(-velocity.Y, -4);
                }
            }
            else
            {
                //if not hit enemy from above then you die
                Die();
            }
        }

        public void Move(Direction dir)
        {
            base.Move(dir, 1.0f);
        }

        public bool LevelComplete(Level currentLevel)
        {
            if (SpriteBounds.Intersects(currentLevel.FinishZone))
                return true;
            return false;
        }

        public override void Die(bool force)
        {
            if ((status == Status.Small && !hurt) || force)
            {
                alive = false;
                velocity = Vector2.Zero;
                Position = new Vector2(0, 0);
            }
            else if (status == Status.Big)
            {
                hurt = true;
                status = Status.Small;
            }
        }

        public override void Update(Level currentLevel, GameTime gameTime)
        {
            if (!alive)
                return;

            Vector2 lastPosition = Position;

            //moving X position
            Position = new Vector2(Position.X + velocity.X, Position.Y);
            var gobj = IntersectsWithAny(currentLevel.GameObjects);
            if (gobj != null && gobj.solid)
                Position = lastPosition;

            velocity.X *= 0.75f;

            lastPosition = Position;

            //moving Y position
            Position = new Vector2(Position.X, Position.Y  + velocity.Y);
            gobj = IntersectsWithAny(currentLevel.GameObjects);
            if (gobj != null)
            {
                if (gobj.Type == ObjectType.ItemBox && velocity.Y < 0)
                {
                    ItemBox ibox = (ItemBox)gobj;
                    if (Position.Y >= (ibox.Position.Y + ibox.Height - 5))
                        if ((Position.X + Width/2 > ibox.Position.X) && (Position.X + Width/2 < ibox.Position.X + ibox.Width))
                            if (!ibox.hit)
                                ibox.Hit();
                }
                else if (gobj.Type == ObjectType.Item)
                {
                    Item I = (Item)gobj;
                    switch (I.ItemIndex)
                    {
                        case 0: status = Status.Big;
                                break;

                        case 1: previousStatus = status;
                                status = Status.Invincible;
                                break;
                    }
                    I.alive = false;
                }

                if (gobj.solid)
                {
                    Position = lastPosition;
                }
                if (!canJump)
                {
                    velocity.Y = 0;
                    if (Position.Y < gobj.Position.Y)
                        canJump = true;
                }
            }
            else 
            {
                canJump = false;
            }

            //increment the hurt timer
            //So the player doesn't die right away even when big
            if (hurt)
                hurtTimer++;
            if (hurtTimer > hurtTime)
            {
                hurt = false;
                hurtTimer = 0;
            }

            //increment the invincible timer
            if (status == Status.Invincible)
                invincibleTimer++;
            if (invincibleTimer > invincibleTime)
            {
                status = previousStatus;
                invincibleTimer = 0;
            }

            base.Update(currentLevel, gameTime);
        }
    }
}
