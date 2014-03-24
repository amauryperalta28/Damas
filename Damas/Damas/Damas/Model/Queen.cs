using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;

namespace DragAndDrop.Model
{
   public class Queen:Ficha
    {
       public override void getPosiblesMovidas(){}
       public override void getPosiblesMovidasComer(){}
       public override int canMove(Vector2 posicionInicial, Vector2 PosicionFinal)
       {
           //Variables en la que se insertan las posiciones validas para moverse
           Vector2[] posicionesValidas = new Vector2[4];
           int IndexValidmove = 0;

           // Se inserta en un arreglo las posiciones correctas que esten dentro del tablero
           if (estaDentroDelTablero(posicionInicial.X + 80, posicionInicial.Y - 80) == 1)
           {
               posicionesValidas[IndexValidmove].X = posicionInicial.X + 80;
               posicionesValidas[IndexValidmove].Y = posicionInicial.Y - 80;
               IndexValidmove++;

           }

           if (estaDentroDelTablero(posicionInicial.X - 80, posicionInicial.Y - 80) == 1)
           {
               posicionesValidas[IndexValidmove].X = posicionInicial.X - 80;
               posicionesValidas[IndexValidmove].Y = posicionInicial.Y - 80;
               IndexValidmove++;

           }

           if (estaDentroDelTablero(posicionInicial.X + 80, posicionInicial.Y + 80) == 1)
           {
               posicionesValidas[IndexValidmove].X = posicionInicial.X + 80;
               posicionesValidas[IndexValidmove].Y = posicionInicial.Y + 80;
               IndexValidmove++;

           }

           if (estaDentroDelTablero(posicionInicial.X - 80, posicionInicial.Y - 80) == 1)
           {
               posicionesValidas[IndexValidmove].X = posicionInicial.X - 80;
               posicionesValidas[IndexValidmove].Y = posicionInicial.Y + 80;
               IndexValidmove++;

           }

           // Se verifica si la posicion a evaluar esta dentro de las posiciones validas
           for (int i = 0; i < posicionesValidas.Length; i++)
           {
               if (PosicionFinal.X == posicionesValidas[i].X && PosicionFinal.Y == posicionesValidas[i].Y)
               {
                   return 1;
               }

           }
           return 0;


       }

       public Queen(SpriteBatch spriteBatch, Texture2D texture, Vector2 position)
        {
            base._spriteBatch = spriteBatch;
            base.Texture = texture;
            base.Position = position;
            posicion = position;
           // base.Color = Colores.Black;
        }
    }
}
