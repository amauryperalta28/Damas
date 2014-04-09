using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;

namespace Damas.Model
{
   public abstract class Queen:Ficha
    {
        /** @brief Determina si la ficha se puede mover a una posicion indicada
         * 
         * @param[in]   posicionInicial             Es la posicion actual de la ficha
         * @param[in]   PosicionFinal               Es la posicion a donde se pretende mover la ficha
         * 
         * @return      1 si se puede mover, 0 de lo contrario.
         */
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

           if (estaDentroDelTablero(posicionInicial.X - 80, posicionInicial.Y + 80) == 1)
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

      
    }
}
