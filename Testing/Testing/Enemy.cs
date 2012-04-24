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

        public override void Update(Level currentLevel, GameTime gameTime)
        {
            if (!alive)
                return;

            Move(MovementDirection, 0.1f);

            Vector2 lastPosition = Position;
            Position += new Vector2(velocity.X, 0);
            velocity.X *= 0.75f;

            var gobj = IntersectsWithAny(currentLevel.GameObjects);
            if (gobj != null && gobj.solid)
            {
                Position = lastPosition;
                HitWall();
            }

            base.Update(currentLevel, gameTime);
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

            //if player is invincible then die
            if (mob.Type == ObjectType.Player)
            {
                Player p = (Player)mob;
                if (p.status == Player.Status.Invincible)
                {
                    Die();
                    p.score++;
                }
            }

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
