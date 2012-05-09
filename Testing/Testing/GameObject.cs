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
        public enum ObjectType
        {
            Player, //Player Class
            Enemy, //Enemy Class
            Block, //part of the GameObject Class
            ItemBox, //ItemBox Class
            Item //Item Class
        }
        public ObjectType Type; 

        private Vector2 position; //(x,y) position of the player
        public Vector2 velocity;
        public Texture2D sprite;
        public Rectangle SpriteBounds; //bounds of the sprite as a rectangle object
        public bool alive = true;
        public bool solid = true;

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

        //Constructor
        public GameObject(Vector2 pos, Vector2 vel, Texture2D spritetex, ObjectType objType)
        {
            position = pos;
            velocity = vel;
            sprite = spritetex;
            Type = objType;
            SpriteBounds = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
        }

        //First checks collision based on the rects intersectin, then calls the pixelsIntersect
        //method to check if actually a collision
        public bool Collision(GameObject o)
        {
            if (SpriteBounds.Intersects(o.SpriteBounds) && o.alive)
            {
                if (PixelsIntersect(SpriteBounds,
                                    TextureToArray(sprite),
                                    o.SpriteBounds,
                                    TextureToArray(o.sprite)))
                    return true;
            }
            return false;
        }

        //IntersectPixels method taken directly from the XNA 2D per pixel collision check. 
        //Doesn't need to be changed as far as I can see.
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

        //Checks for collision with any other object in the game and returns
        //a list of all of those objects
        public virtual GameObject IntersectsWithAny(List<GameObject> gameObjects)
        {
            foreach (GameObject o in gameObjects)
            {
                if (Collision(o))
                    return o;
            }
            return null;
        }

        //To be overriden by child classes
        public virtual void Update(Level currentLevel, GameTime gameTime)
        {

        }

        //Most classes will probably override this
        //Takes in a sprite batch and uses it to draw the sprite at the provided 
        //location. Does not use the objects position variable as that is relative
        //to the map. Need to draw relative to the camera position
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 actualPos)
        {
            spriteBatch.Draw(sprite,
                             actualPos,
                             Color.White);
        }

    }
}
