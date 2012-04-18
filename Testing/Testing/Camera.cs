using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Testing
{
    class Camera
    {
        //Variables
        private enum CameraMode
        {
            Free,
            Locked
        }
        private Viewport viewport;
        private GameObject focusObject;
        private CameraMode mode;
        private Rectangle boundingRect;

        public bool LockToPlayingArea = true;

        //Constructor
        public Camera(int x, int y, int width, int height)
        {
            viewport = new Viewport(x, y, width, height);
            boundingRect = new Rectangle(x, y, width, height);
        }

        //Methods
        public Vector2 Position
        {
            get { return new Vector2(viewport.X, viewport.Y); }
        }

        public void LockToObject(GameObject gameObject)
        {
            focusObject = gameObject;
            mode = CameraMode.Locked;
        }

        public void Update()
        {
            if (mode == CameraMode.Locked)
            {
                if (viewport.Bounds.Center != focusObject.SpriteBounds.Center)
                {
                    viewport.X = (int)focusObject.Position.X - viewport.Width / 2;
                    boundingRect.X = viewport.X;
                }
            }
            if (LockToPlayingArea && viewport.X < 0)
            {
                viewport.X = 0;
                boundingRect.X = viewport.X;
            }
        }

        //check if the GameObj is in the view area
        public bool Visible(GameObject gobj)
        {
            return boundingRect.Intersects(gobj.SpriteBounds);
        }
    }
}
