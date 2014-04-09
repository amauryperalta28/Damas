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
       private Vector2 posicion;
       Texture2D _whiteSquare;

       public Tablero(ContentManager content)
       {
           //Se inicializa la posicion del tablero
           posicion = new Vector2(70, 20);

           //Se le asignan las posiciones a cada casilla
           for (int i = 0; i < casillas.GetLength(0); i++)
           {
               for (int j = 0; j < casillas.GetLength(1); j++)
               {
                   int posicionXPantalla = 70 + (i) * 80;
                   int posicionYPantalla = 20 + (j) * 80;
                   casillas[i, j] = new Casilla() { Posicion = new Vector2(posicionXPantalla, posicionYPantalla) };

               }

           }
           //Se le asignan los colores a las casillas
           setColorCasillas(content);
       
       }
       public Tablero(ContentManager content, SpriteBatch spriteBatch)
           : this(content)
       {
           _whiteSquare = content.Load<Texture2D>("Images/white");

           
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
               //Si se va a cambiar de fila, cambiame el color anterior
               if (colorEnUso == Colores.White)
                   colorEnUso = Colores.Black;
               else
                   colorEnUso = Colores.White;
           }
   

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

       
       
       public Casilla[,] getCasillas()
        {
            return casillas;
        }
            

       public void draw(SpriteBatch spritebatch, Vector2 _currentMousePosition, DragAndDropController<Item> _dragAndDropController)
       {
           float opacity;
           Color colorToUse = Color.White;
           const int _tileSize = 80;
           //the square to draw (local variable to avoid creating a new variable per square)
           Rectangle squareToDrawPosition = new Rectangle();  
           
           //Lista para guardar las fichas que pueden comer
          // List<Ficha> fichasPuedenComer= new List<Ficha>();

           Red fichaPrueba = new Red(spritebatch, _whiteSquare, new Vector2(0, 0));

           /*Se determinan las fichas que pueden comer y se guardan  */
           foreach (var fichaAEvaluar in _dragAndDropController.Items)
           {
                    //Se colorean las casillas de las fichas que deben comer   
                   if (_dragAndDropController.fichaPuedeComer(fichaAEvaluar) && fichaAEvaluar.Color.Equals(Colores.Red) && 
                       manejadorDeTurnos.turnoJugadorRojo == true)
                   {
                       fichaPrueba.addJugadaParaComerFicha(fichaAEvaluar.Position);
                       fichaAEvaluar.removeJugadasParaComerFicha();
                   }
                   else if (_dragAndDropController.fichaPuedeComer(fichaAEvaluar) && fichaAEvaluar.Color.Equals(Colores.Black) &&
                       manejadorDeTurnos.turnoJugadorNegro == true)
                   {
                       fichaPrueba.addJugadaParaComerFicha(fichaAEvaluar.Position);
                       fichaAEvaluar.removeJugadasParaComerFicha();
                   
                   }
               
           }

           



           // Se recorre el arreglo de casillas
           for (int y = 0; y < casillas.GetLength(0); y++)
           {
               for (int x = 0; x < casillas.GetLength(1); x++)
               {
                   //figure out where to draw the square
                   squareToDrawPosition = new Rectangle((int)(x * _tileSize + posicion.X), (int)(y * _tileSize + posicion.Y), _tileSize, _tileSize);

                   Vector2 squarePosition = new Vector2(x * _tileSize + posicion.X, y * _tileSize + posicion.Y);
                   //if we add the x and y value of the tile
                   //and it is even, we make it one third opaque
                   if ((x + y) % 2 == 0)
                   {
                       opacity = .33f;
                   }
                   else
                   {
                       //otherwise it is one tenth opaque
                       opacity = .1f;
                   }
                   //make the square the mouse is over red
                   if (IsMouseOnTile(x, y,_currentMousePosition))
                   {
                       colorToUse = Color.Red;
                       opacity = .5f;
                   }
                   else
                   {
                       colorToUse = Color.White;
                   }

                   
                   Casilla c1 = casillas[x, y];
                   spritebatch.Draw(c1.Img, c1.Posicion,null, Color.WhiteSmoke);
                   //draw the white square at the given position, offset by the x- and y-offset, in the opacity desired
                   spritebatch.Draw(_whiteSquare, squareToDrawPosition, colorToUse * opacity);

                   if (fichaPrueba.esJugadaParaComerFicha(squarePosition) == true)
                   {
                       colorToUse = Color.DarkGreen;
                       opacity = 0.5f;
                       spritebatch.Draw(_whiteSquare, squareToDrawPosition, colorToUse * opacity);
                   }
               
               }
           }
       
       }

       // Checks to see whether a given coordinate is within the board
       private bool IsMouseOnTile(int x, int y, Vector2 _currentMousePosition)
       {
           const int _tileSize = 80;
           //do an integerdivision (whole-number) of the coordinates relative to the board offset with the tilesize in mind
           return (int)(_currentMousePosition.X - posicion.X) / _tileSize == x && (int)(_currentMousePosition.Y - posicion.Y) / _tileSize == y;
       }

   

    }
}
