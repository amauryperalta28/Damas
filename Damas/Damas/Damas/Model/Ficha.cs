using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;
using DragAndDrop;

namespace Damas.Model
{
   public abstract class Ficha: Item, IDragAndDropItem
    {

           
      private Colores color;       
      protected Vector2 posicion;       

      /* Atributo para almacenadar la posiciones finales despues de comer una ficha*/
      private List<Vector2> posiblesMovidasComer = new List<Vector2>();


      /*Propiedad del atributo color*/
      public Colores Color { get { return this.color; } set { color = value; } }
       
      /** @brief Determina si la ficha se puede mover a una posicion indicada
       * 
       * @param[in]   posicionInicial             Es la posicion actual de la ficha
       * @param[in]   PosicionFinal               Es la posicion a donde se pretende mover la ficha
       * 
       * @return      1 si se puede mover, 0 de lo contrario.
       */
      public abstract int canMove(Vector2 posicionInicial, Vector2 PosicionFinal);

      /** @brief Determina si una posicion indicada, se encuentra dentro de las posiciones validas para comer una ficha 
       * 
       * @param[in]   jugadaAEvaluar              Es la posicion que se va verificar
       * 
       * @return      true si es una jugada para comer, false de lo contrario.
       */     
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
      /** @brief Inserta una posicion indicada en la lista de posibles Movidas para comer ficha
      * 
      * @param[in]   posicion              Es la posicion a insertar
      * 
      * @return      no retorna nada
      */ 
      public void addJugadaParaComerFicha(Vector2 posicion)
      {
          posiblesMovidasComer.Add(posicion);
      }

    /** @brief Elimina una posicion indicada en la lista de posibles Movidas para comer ficha
     * 
     * @return      no retorna nada
     */ 
      public void removeJugadasParaComerFicha()
      {
          posiblesMovidasComer.Clear();
      
      }
          
    /**
    * @brief Determina si la posicion esta en los limites permitidos
    *
    * @param[in]    posX        	 Es la posicion x a evaluar
    * @param[in]    posY             Es la posicion y a evaluar
    *
    * @return		1 si esta en los limites, 0 si no lo esta.
    */
    public int estaDentroDelTablero(double posX, double posY)
    {

        return (((posX >= 70) && (posX <= 650)) && ((posY >= 20) && (posY <= 580)) ? 1 : 0);
    }
       

    }
}
