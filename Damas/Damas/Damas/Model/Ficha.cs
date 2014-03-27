using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;

namespace DragAndDrop
{
   public abstract class Ficha: Item, IDragAndDropItem
    {

           
      private Colores color;       
      protected Vector2 posicion;       
      private Texture2D img;             
      private List<Vector2> posiblesMovidasComer = new List<Vector2>();
      
      public Colores Color { get { return this.color; } set { color = value; } }
   
      public Texture2D Img { get { return this.img; } set { img = value; } }

      public abstract int canMove(Vector2 posicionInicial, Vector2 PosicionFinal);

      public void comerFicha(Casilla final) { }

      public bool esJugadaParaComerFicha(Vector2 jugadaAEvaluar)
      {
          // Se recorre la lista de posibles movidas para comer
          foreach (var jugada in posiblesMovidasComer)
          {
              //Se verifica si se encuentra la jugada a evaluar dentro 
              //de la lista de posibles jugadas para comer
              if (jugadaAEvaluar.Equals(jugada))
              {
                  return true;
              }
              
          }
         return false;
      
      }
      public void addJugadaParaComerFicha(Vector2 posicion)
      {
          posiblesMovidasComer.Add(posicion);
      }
      public void removeJugadasParaComerFicha()
      {
          posiblesMovidasComer.Clear();
      
      }
          
     /*
    * @brief Determina si la posicion esta en los limites permitidos
    *
    * @param[in] posicion	   Es la posicion a evaluar
    *
    * @return					1 si esta en los limites, 0 si no lo esta.
    */
    public int estaDentroDelTablero(double posX, double posY)
    {

        return (((posX >= 70) && (posX <= 650)) && ((posY >= 20) && (posY <= 580)) ? 1 : 0);
    }
       

    }
}
