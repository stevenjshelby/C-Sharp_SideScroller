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
        //Variables
        private bool canJump = true;

        //Constructors
        public Player(Vector2 position, Vector2 velocity, Texture2D sprite)
            : base(position, velocity, sprite, ObjectType.Player)
        {
        }

        //Methods
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
            //if collision with enemy
            if (Position.Y + Height - 4 <= mob.Position.Y + 12)
            {
                //kill enemy if coming from above
                if (velocity.Y > 0)
                    mob.Die();

                //bounce off of enemy
                canJump = true;
                Jump();
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

        public override void Update(Level currentLevel)
        {
            if (!alive)
                return;

            Vector2 lastPosition = Position;

            Position = new Vector2(Position.X + velocity.X, Position.Y);
            if (IntersectsWithAny(currentLevel.GameObjects) != null)
                Position = lastPosition;

            velocity.X *= 0.75f;

            lastPosition = Position;

            Position = new Vector2(Position.X, Position.Y  + velocity.Y);
            if (IntersectsWithAny(currentLevel.GameObjects) != null)
            {
                Position = lastPosition;
                if (velocity.Y < 0)
                    velocity.Y = 0;
            }
            
            ApplyGravity(currentLevel);

            if (Position.Y > Game1.ScreenHeight)
            {
                //below visible screen
                Die();
            }
        }
    }
}
