using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Testing
{
    class Level
    {
        //Variables
        public string Name;
        public List<GameObject> GameObjects;
        public Vector2 StartPosition;
        public Rectangle FinishZone;
        public bool Finished;
        public DateTime LevelFinishTime;

        //Constructor
        public Level()
        {
            GameObjects = new List<GameObject>();
        }

        public Vector2 FinishPosition
        {
            get { return new Vector2(FinishZone.X, FinishZone.Y); }
        }
    }
}
