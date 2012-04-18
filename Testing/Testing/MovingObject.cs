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
        //Variables
        public enum Direction
        {
            Left,
            Right
        }
        public bool alive;

        //Properties

        //Constructors
        public MovingObject(Vector2 pos, Vector2 vel, Texture2D spritetex, ObjectType objType)
                : base(pos, vel, spritetex, objType)
        {
            alive = true;
        }

        //Methods
        public override void Update(Level currentLevel)
        {
            if (alive)
            {
                Vector2 lastPosition = Position;
                if (velocity.Y > 0)
                    Position += new Vector2(velocity.X, 0);
                else
                    Position += velocity;
                velocity.X *= 0.75f;
                if (velocity.Y < 0)
                    velocity.Y *= 0.90f;

                if (IntersectsWithAny(currentLevel.GameObjects) != null)
                {
                    Position = lastPosition;
                    velocity = new Vector2(0, velocity.Y);
                    HitWall();
                }
                ApplyGravity(currentLevel);
                if (Position.Y > Game1.ScreenHeight + 200)
                {
                    //below visible screen
                    Die();
                }
            }
            else
            {
                if (Position.Y < -5000)
                    velocity = Vector2.Zero;
                Position += velocity;
            }
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
            if (alive)
            {
                alive = false;
                velocity = new Vector2(0, -10f);
            }
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
    }
}
