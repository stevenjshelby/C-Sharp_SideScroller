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
    class Enemy : MovingObject
    {
        public Direction MovementDirection;
        public Point SpawnLocation;

        public Enemy(Vector2 position, Vector2 velocity, Texture2D sprite)
            : base(position, velocity, sprite, ObjectType.Enemy)
        {
            SpawnLocation = new Point((int)position.X, (int)position.Y);
        }

        public override void Update(Level currentLevel)
        {
            if (!alive)
                return;

            Move(MovementDirection, 0.1f);

            Vector2 lastPosition = Position;
            if (velocity.Y > 0)
                Position += new Vector2(velocity.X, 0);
            else
                Position += velocity;
            velocity.X *= 0.75f;
            if (velocity.Y < 0)
                velocity.Y *= 0.90f;

            var gobj = IntersectsWithAny(currentLevel.GameObjects);

            if (gobj != null && gobj.solid)
            {
                Position = lastPosition;
                HitWall();
            }

            base.Update(currentLevel);
        }

        public override void HitWall()
        {
            MovementDirection = MovementDirection == Direction.Left ? Direction.Right : Direction.Left;
        }

        public override void DetectEnemyHit(MovingObject mob)
        {
            //dont kill friends
            if (mob.Type == this.Type)
                return;

            //collision with enemy detected
            if (Position.Y > mob.Position.Y+mob.Height)
            {
                //enemy is above, cant kill
                return;
            }
            mob.Die();
        }
    }
}
