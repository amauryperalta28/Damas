using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;

namespace DragAndDrop
{
   public abstract class Ficha: Item
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
      public void canMove(Casilla casillaInicial, Casilla casillaFinal) { }
      public void comerFicha(Casilla final) { }
      public abstract void getPosiblesMovidas();
      public abstract void getPosiblesMovidasComer();

       

    }
}
