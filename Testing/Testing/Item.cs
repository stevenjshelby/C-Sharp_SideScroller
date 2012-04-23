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
    class Item : GameObject
    {
        //Constructor
        public Item(Vector2 pos, Texture2D spritetex)
                : base(pos, Vector2.Zero, spritetex, ObjectType.Item)
        {
            solid = false;
        }
    }
}
