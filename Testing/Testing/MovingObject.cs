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
        public enum Direction
        {
            Left,
            Right
        }

        //Constructor
        public MovingObject(Vector2 pos, Vector2 vel, Texture2D spritetex, ObjectType objType)
                : base(pos, vel, spritetex, objType)
        {
            //uses same constructor as base class.
            //no need for additional assignments.
        }

        //set the objects velocity in the horizontal axis
        //ApplyGravity() will take care of the vertical movement
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
            //do nothing here
            //used for enemy class
        }

        //add gravity to the velocity vector then check for
        //collision and move the piece in the vertical axis
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

        //called when the moving object reaches the ground
        public virtual void ReachedBottom()
        {
            velocity.Y = 0;
        }

        //called when the object is killed
        //a force option is used in some child classes
        public virtual void Die(bool force)
        {
            alive = false;
            velocity = Vector2.Zero;
            Position = new Vector2(0, 0);
        }

        //called when the object is killed
        public virtual void Die()
        {
            Die(false);
        }

        //returns a list of objects that this object is currently colliding with
        public override GameObject IntersectsWithAny(List<GameObject> gameObjects)
        {   
            //if not alive then it doesn't matter
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

        //Main update method
        public override void Update(Level currentLevel, GameTime gameTime)
        {
            ApplyGravity(currentLevel);

            if (Position.Y > Game1.ScreenHeight)
            {
                //below visible screen
                Die(true);
            }

            base.Update(currentLevel, gameTime);
        }
    }
}
