using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Testing
{
    /// <summary>
    /// Base class for all the objects in the game.
    /// </summary>
    class GameObject
    {
        //Variables
        public enum ObjectType
        {
            Player,
            Enemy,
            Block
        }

        private Vector2 position;
        public Vector2 velocity;
        public Texture2D sprite;
        public Rectangle SpriteBounds;
        public ObjectType Type;
        public bool alive = true;

        //Properties
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                SpriteBounds.X = (int)position.X;
                SpriteBounds.Y = (int)position.Y;
            }
        }

        public int Width
        {
            get { return sprite.Width; }
        }

        public int Height
        {
            get { return sprite.Height; }
        }

        //Constructors
        public GameObject(Vector2 pos, Vector2 vel, Texture2D spritetex, ObjectType objType)
        {
            position = pos;
            velocity = vel;
            sprite = spritetex;
            Type = objType;
            SpriteBounds = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
        }

        //Methods
        public bool Collision(GameObject o)
        {
            if (SpriteBounds.Intersects(o.SpriteBounds))
            {
                if (PixelsIntersect(SpriteBounds,
                                    TextureToArray(sprite),
                                    o.SpriteBounds,
                                    TextureToArray(o.sprite)))
                    return true;
            }
            return false;
        }

        //IntersectPixels method taken directly from the XNA 2D per pixel collision check. Doesn't need to be changed as far as I can see. 
        private bool PixelsIntersect(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = dataA[(x - rectangleA.Left) +
                                (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                (y - rectangleB.Top) * rectangleB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Color[] TextureToArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            return colors1D;
        }

        public virtual GameObject IntersectsWithAny(List<GameObject> gameObjects)
        {
            foreach (GameObject o in gameObjects)
            {
                if (Collision(o))
                    return o;
            }
            return null;
        }

        public virtual void Update(Level currentLevel)
        {
        }

    }
}
