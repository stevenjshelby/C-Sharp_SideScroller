﻿using System;
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
    class ItemBox : GameObject
    {
        public bool hit = false;

        private GameObject item;

        //Constructor
        public ItemBox(Vector2 pos, Vector2 vel, Texture2D spritetex, ObjectType objType, GameObject I)
                : base(pos, vel, spritetex, ObjectType.ItemBox)
        {
            item = I;
        }

        public void Hit()
        {
            hit = true;
            item.alive = true;
        }
    }
}
