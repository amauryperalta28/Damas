using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DragAndDrop.Model;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Damas.Model
{
    class BlackQueen: Queen
    {
        public BlackQueen(SpriteBatch spriteBatch, ContentManager content, Vector2 position)
        {

            Color = Colores.Black;
            base._spriteBatch = spriteBatch;
            base.Texture = content.Load<Texture2D>("Images/reinablack"); ;
            base.Position = position;
            posicion = position;
            
        }
    }
}
