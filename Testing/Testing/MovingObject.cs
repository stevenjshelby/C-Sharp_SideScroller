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
    /// <summary>
    /// Base Class for all objects requiring movement
    /// </summary>
    abstract class MovingObject : GameObject
    {
        FrameAnimation leftanimation;
        FrameAnimation rightanimation;

        public enum Direction
        {
            Left,
            Right
        }

        //Constructor
        public MovingObject(Vector2 pos, Vector2 vel, Texture2D spritetex, ObjectType objType)
                : base(pos, vel, spritetex, objType)
        {
            leftanimation = new FrameAnimation(2, 32, 32, 0, 0);
            leftanimation.FramesPerSecond = 4;
            Animations.Add("LeftAnimation", leftanimation);

            rightanimation = new FrameAnimation(2, 32, 32, 64, 0);
            rightanimation.FramesPerSecond = 4;
            Animations.Add("RightAnimation", rightanimation);

            CurrentAnimationName = "RightAnimation";
        }

        public virtual void Move(Direction dir, float speed)
        {
            if (alive)
            {
                if (dir == Direction.Left)
                    velocity.X -= speed;
                else if (dir == Direction.Right)
                    velocity.X += speed;
            }
        }

        public virtual void HitWall()
        {
        }

        public void ApplyGravity(Level level)
        {
            if (alive)
            {
                Vector2 originalPosition = Position;

                velocity.Y += 0.3f;
                Vector2 verticalVelocity = new Vector2(0, velocity.Y);

                Position += verticalVelocity;

                GameObject iobj = IntersectsWithAny(level.GameObjects);
                if (iobj != null)
                {
                    Position = originalPosition;
                    //object is below this
                    if (iobj.Position.Y > Position.Y)
                        ReachedBottom(); //stop falling
                }
            }
        }

        public virtual void ReachedBottom()
        {
            velocity.Y = 0;
        }

        public void Die()
        {
            alive = false;
            velocity = Vector2.Zero;
            Position = new Vector2(0, 0);
        }

        public override GameObject IntersectsWithAny(List<GameObject> gameObjects)
        {
            if (!alive)
                return null;

            foreach (GameObject gobj in gameObjects)
            {
                if (gobj == this)
                    continue;
                if (Collision(gobj))
                {
                    if (gobj.Type == ObjectType.Enemy || gobj.Type == ObjectType.Player)
                    {
                        if (Type == ObjectType.Enemy && gobj.Type == ObjectType.Enemy)
                            return gobj;
                        var mob = (MovingObject)gobj;
                        if (mob.alive)
                        {
                            DetectEnemyHit(mob);
                        }
                    }
                    else
                    {
                        return gobj;
                    }
                }
            }

            return null;
        }

        public virtual void DetectEnemyHit(MovingObject mob)
        {
        }

        public override void Update(Level currentLevel, GameTime gameTime)
        {
            ApplyGravity(currentLevel);

            if (Position.Y > Game1.ScreenHeight)
            {
                //below visible screen
                Die();
            }

            base.Update(currentLevel, gameTime);
        }
    }
}
