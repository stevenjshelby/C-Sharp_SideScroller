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
        public string Name;
        public List<GameObject> GameObjects; //list of all of the objects on the map
        public Vector2 StartPosition; //where the player starts
        public Rectangle FinishZone; //where the player needs to get to to complete the level
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
