using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Damas.Model;

namespace Damas.Model
{
   public class Casilla
    {
       /* Atributos*/
        private Colores color;
        private Vector2 posicion;
        private Texture2D img;
        private Ficha fichaContenida;

       /*Propiedades de los atributos*/
        public Colores Color { get { return this.color; } set { color = value; } }
        public Vector2 Posicion { get { return this.posicion; } set { posicion = value;} }
        public Texture2D Img {get { return this.img; } set { img = value;} }
        public Ficha FichaContenida { get { return this.fichaContenida; } set { fichaContenida = value; } }


    }
}
