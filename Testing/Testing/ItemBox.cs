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
    class ItemBox : GameObject
    {
        public bool hit = false;

        private GameObject item;

        Texture2D spriteTexHit;

        //Constructor
        public ItemBox(Vector2 pos, Vector2 vel, Texture2D spritetex1, Texture2D spritetex2, ObjectType objType, GameObject I)
                : base(pos, vel, spritetex1, ObjectType.ItemBox)
        {
            item = I;
            spriteTexHit = spritetex2;
        }

        public void Hit()
        {
            hit = true;
            item.alive = true;
            sprite = spriteTexHit;
        }
    }
}
