using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;

namespace DragAndDrop.Model
{
   public class Black: Ficha
    {
       public override void getPosiblesMovidas(){}       public override void getPosiblesMovidasComer(){}

       public override void canMove(Casilla casillaInicial, Casilla casillaFinal) { }        public Black(SpriteBatch spriteBatch, Texture2D texture, Vector2 position)
        {
            base._spriteBatch = spriteBatch;
            base.Texture = texture;
            base.Position = position;
            posicion = position;
            base.Color = Colores.Black;
        }
       
    }
}
