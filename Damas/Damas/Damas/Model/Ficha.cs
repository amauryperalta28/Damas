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

      private Vector2[] posiblesMovidas = new Vector2[6];
      private Vector2[] posiblesMovidasComer = new Vector2[6];
      

           
      public Colores Color { get { return this.color; } set { color = value; } }
    //  public Vector2 Posicion { get { return this.posicion; } set{ posicion = value;}  }
      public Texture2D Img { get { return this.img; } set { img = value; } }
    

     // public Vector2[] getPosiblesMovidas() { return this.posiblesMovidas; }
      //public Vector2[] getPosiblesMovidasComer() { return this.posiblesMovidasComer; }
      public abstract int canMove(Vector2 posicionInicial, Vector2 PosicionFinal);
      public void comerFicha(Casilla final) { }
      public abstract void getPosiblesMovidas();
      public abstract void getPosiblesMovidasComer();
    
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
