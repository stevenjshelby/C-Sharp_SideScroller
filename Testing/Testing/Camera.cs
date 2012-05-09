using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Testing
{
    class Camera
    {
        private enum CameraMode
        {
            Free,
            Locked
        }
        private CameraMode mode;

        private Viewport viewport;
        private GameObject focusObject; //what object the camera is following
        private Rectangle boundingRect;

        public bool LockToPlayingArea = true;

        //Constructor
        public Camera(int x, int y, int width, int height)
        {
            viewport = new Viewport(x, y, width, height);
            boundingRect = new Rectangle(x, y, width, height);
        }

        public Vector2 Position
        {
            get { return new Vector2(viewport.X, viewport.Y); }
        }

        //set the camera to a game object
        public void LockToObject(GameObject gameObject)
        {
            focusObject = gameObject;
            mode = CameraMode.Locked;
        }
    

        public void Update()
        {
            //if the camera mode is locked to something then position the
            //camera so the object it is following is centered horizontally on screen
            if (mode == CameraMode.Locked)
            {
                if (viewport.Bounds.Center != focusObject.SpriteBounds.Center)
                {
                    viewport.X = (int)focusObject.Position.X - viewport.Width / 2;
                    boundingRect.X = viewport.X;
                }
            }
            //prevents the camera from going off the map when the player
            //is at the beginning or end of the map
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
