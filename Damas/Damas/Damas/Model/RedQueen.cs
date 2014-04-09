using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Damas.Model
{
    class RedQueen: Queen
    {
        public RedQueen(SpriteBatch spriteBatch, ContentManager content, Vector2 position)
        {

            Color = Colores.Red;
            base._spriteBatch = spriteBatch;
            base.Texture = content.Load<Texture2D>("Images/reinared"); ;
            base.Position = position;
            posicion = position;

            
            
        }
    }
}
