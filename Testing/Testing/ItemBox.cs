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
        public bool hit = false; //has the itembox been smashed already?

        private GameObject item; //what type of item is this?
            //Player
            //Enemy
            //Block
            //ItemBox
            //Item

        Texture2D spriteTexHit; //the texture used once the box has been smashed

        //Constructor
        public ItemBox(Vector2 pos, Vector2 vel, Texture2D spritetex1, Texture2D spritetex2, ObjectType objType, GameObject I)
                : base(pos, vel, spritetex1, ObjectType.ItemBox)
        {
            item = I; //the item box is linked to an item
            spriteTexHit = spritetex2; //load the hit texture
        }

        //when the box it properly hit from below then set its linked
        //item to alive and set smashed = true.
        public void Hit()
        {
            hit = true;
            item.alive = true;
            sprite = spriteTexHit;
        }
    }
}
