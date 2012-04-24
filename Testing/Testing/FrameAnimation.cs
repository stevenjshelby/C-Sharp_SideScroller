using System;

using Microsoft.Xna.Framework;

namespace Testing
{
    public class FrameAnimation : ICloneable
    {
        Rectangle[] frames; //Holds an array of rectangles that hold the Rectangle coordinates of each frame within the Texture2D
        int currentFrame = 0; //Index of current frame in the Rectangle[]

        float frameLength = 0.5f; //Number of seconds to wait before switching frames
        float timer = 0; //Keeps track of timing for switching frames

        public int FramesPerSecond
        {
            get
            {
                return (int)(1f / frameLength);
            }
            set
            {
                frameLength = (float)Math.Max(1f / (float)value, .001f);
            }
        }

        public Rectangle CurrentRect
        {
            get { return frames[currentFrame]; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
            set
            {
                currentFrame = (int)MathHelper.Clamp(value, 0, frames.Length - 1);
            }
        }

        //Constructor
        public FrameAnimation(int numberOfFrames, int frameWidth, int frameHeight, int xOffSet, int yOffSet)
        {
            frames = new Rectangle[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = frameWidth;
                rect.Height = frameHeight;
                rect.X = xOffSet + (i * frameWidth);
                rect.Y = yOffSet;

                frames[i] = rect;
            }
        }

        //Add elapsed time to timer, then check if timer is greater than
        //frameLength and if so, reset timer and increment the current
        //frame. If the currentFrame is greater than total number of frames
        //then reset currentFrame
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameLength)
            {
                timer = 0f;

                currentFrame++;
                if (currentFrame >= frames.Length)
                    currentFrame = 0;
            }
        }

        //For clone
        private FrameAnimation()
        {
            //for clone do not delete
            //for clone do not delete
            //for clone do not delete
        }

        public object Clone()
        {
            FrameAnimation anim = new FrameAnimation();
            anim.frameLength = frameLength;
            anim.frames = frames;

            return anim;
        }
    }
}
