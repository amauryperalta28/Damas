using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Damas.Model;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DragAndDrop.Model
{
    public class Tablero
    {
       public Casilla[,] casillas = new Casilla[8,8];

       public Tablero(ContentManager content, SpriteBatch spriteBatch)
       { 
           //Se le asignan las posiciones a cada casilla

           for (int i = 0; i < casillas.GetLength(0); i++)
           {
               for (int j = 0; j < casillas.GetLength(1); j++)
               { 
                     int posicionXPantalla = 50 + (i+1)*64;
                     int posicionYPantalla = 20 + (j+1)*64;
                     casillas[i,j]= new Casilla(){Posicion = new Vector2(posicionXPantalla,posicionYPantalla)};
               
               }
                      
           }
           //Se le asignan los colores a las casillas
          setColorCasillas(content);

          //Se colocan las fichas en el tablero
        setFichasTablero(content, spriteBatch);
       
       
       }

       private void setColorCasillas(ContentManager content)
       {
           //Primero se colorean las filas pares
           Colores colorEnUso = Colores.Black;

           for(int j=0; j < casillas.GetLength(0); j++)
           {
               for(int i=0; i < casillas.GetLength(1); i++)
               {
                   if(colorEnUso == Colores.White)
                   {
                        casillas[i, j].Color = colorEnUso;
                        casillas[i, j].Img = content.Load<Texture2D>("Images/white");
                        colorEnUso = Colores.Black;
                   }
                   else
                   {
                       casillas[i, j].Color = colorEnUso;
                        casillas[i, j].Img = content.Load<Texture2D>("Images/black");
                        colorEnUso = Colores.White;
                   }
               }

               if (colorEnUso == Colores.White)
                   colorEnUso = Colores.Black;
               else
                   colorEnUso = Colores.White;
           }
    /*
           for (int i = 0; i < casillas.GetLength(1); i++)
           {
               if (colorEnUso == Colores.White)
               {
                   casillas[i, 0].Color = colorEnUso;
                   casillas[i, 0].Img = content.Load<Texture2D>("Images/white");

                   casillas[i, 2].Color = colorEnUso;
                   casillas[i, 2].Img = content.Load<Texture2D>(@"Images/white");

                   casillas[i, 4].Color = colorEnUso;
                   casillas[i, 4].Img = content.Load<Texture2D>(@"Images/white");

                   casillas[i, 6].Color = colorEnUso;
                   casillas[i, 6].Img = content.Load<Texture2D>(@"Images/white");
                   colorEnUso = Colores.Black;

               }
               else
               {
                   casillas[i, 0].Color = colorEnUso;
                   casillas[i, 0].Img = content.Load<Texture2D>(@"Images/black");

                   casillas[i, 2].Color = colorEnUso;
                   casillas[i, 2].Img = content.Load<Texture2D>(@"Images/black");

                   casillas[i, 4].Color = colorEnUso;
                   casillas[i, 4].Img = content.Load<Texture2D>(@"Images/black");

                   casillas[i, 6].Color = colorEnUso;
                   casillas[i, 6].Img = content.Load<Texture2D>(@"Images/black");
                   colorEnUso = Colores.White;
               
               }
               colorEnUso = Colores.White;
               for (int j = 0; j < casillas.GetLength(1); j++)
               {
                   if (colorEnUso == Colores.White)
                   {
                       casillas[j, 1].Color = colorEnUso;
                       casillas[j, 1].Img = content.Load<Texture2D>(@"Images/white");

                       casillas[j, 3].Color = colorEnUso;
                       casillas[j, 3].Img = content.Load<Texture2D>(@"Images/white");

                       casillas[j, 5].Color = colorEnUso;
                       casillas[j, 5].Img = content.Load<Texture2D>(@"Images/white");

                       casillas[j, 7].Color = colorEnUso;
                       casillas[j, 7].Img = content.Load<Texture2D>(@"Images/white");
                       colorEnUso = Colores.Black;

                   }
                   else
                   {
                       casillas[j, 1].Color = colorEnUso;
                       casillas[j, 1].Img = content.Load<Texture2D>(@"Images/black");

                       casillas[j, 3].Color = colorEnUso;
                       casillas[j, 3].Img = content.Load<Texture2D>(@"Images/black");

                       casillas[j, 5].Color = colorEnUso;
                       casillas[j, 5].Img = content.Load<Texture2D>(@"Images/black");

                       casillas[j, 7].Color = colorEnUso;
                       casillas[j, 7].Img = content.Load<Texture2D>(@"Images/black");
                       colorEnUso = Colores.White;

                   }


               }

           
           }*/


       }

       private void setFichasTablero(ContentManager content, SpriteBatch spriteBatch)
       {
           
           Texture2D ImgfichaNegra = content.Load<Texture2D>("Images/ficha1");
           Texture2D ImgfichaRoja = content.Load<Texture2D>("Images/ficha2");
           // Se colocan las fichas negras en el tablero
           for (int j = 0; j < 3; j++)
           {
               for (int i = 0; i < casillas.GetLength(1); i++)
               {
                   if (casillas[j, i].Color == 0)
                   {
                       var fichaNegra = new Black(spriteBatch,ImgfichaNegra, casillas[j,i].Posicion);
                       
                       casillas[i, j].FichaContenida = fichaNegra;     
                   }
               }
           }

          // Se colocan las fichas rojas en el tablero
           for (int k = 5; k < 8; k++)
           {
               for (int l = 0; l < casillas.GetLength(1); l++)
               {
                   if (casillas[k, l].Color == Colores.White)
                   {
                       var fichaRoja = new Red(spriteBatch, ImgfichaRoja, casillas[k, l].Posicion);
                       casillas[l, k].FichaContenida = fichaRoja;
                   }
               }
           }

 
       
       }

       public void moverFichas(Casilla casillaInicial, Casilla casillaFinal)
       {


       }

       public void coronarFichas() { }

       public Casilla[,] getCasillas()
        {
            return casillas;
        }
       public int cantFichasRojas()
       {
           return 0;
       }
       public int cantFichasNegras()
       {
           return 0;
       }

       public void draw(SpriteBatch spritebatch)
       { 
           // Se recorre el arreglo de casillas
           for (int j = 0; j < casillas.GetLength(0); j++)
           {
               for (int i = 0; i < casillas.GetLength(1); i++)
               {
                   Casilla c1 = casillas[i, j];
                   spritebatch.Draw(c1.Img, c1.Posicion,null, Color.WhiteSmoke);
               
               }
           }
       
       }

    }
}
