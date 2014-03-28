using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DragAndDrop;
using DragAndDrop.Model;
using Damas.Model;

namespace Damas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D fondoDelJuego;

        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;

        Colores fichasNegras = Colores.Black;
        Colores fichasRojas = Colores.Red;

        private DragAndDropController<Item> _dragDropController;
        Tablero tablero;
       
        //Variables para almacenar posicion actual del puntero
        MouseState _currentMouse;
        Vector2 _currentMousePosition;          //the current position of the mouse
        
        

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            SetWindowSize(800, 690);

            
        }
        public void SetWindowSize(int x, int y)
        {
            graphics.PreferredBackBufferWidth = x;
            graphics.PreferredBackBufferHeight = y;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
       
            _dragDropController = new DragAndDropController<Item>(this, spriteBatch);
            Components.Add(_dragDropController);
            
            tablero = new Tablero(Content, spriteBatch);
            SetupDraggableItems();
            fondoDelJuego = Content.Load<Texture2D>(@"Images/fondo");

            // TODO: use this.Content to load your game content here
        }
        private void SetupDraggableItems()
        {
            Texture2D itemTexture = Content.Load<Texture2D>(@"Images/ficha1");

            _dragDropController.Clear();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Casilla c1 = tablero.casillas[j, i];
                    // Si hay una ficha en la casilla insertala en el dragAndDropController
                    if (c1.FichaContenida != null)
                    {
                        if(c1.FichaContenida.Color == Colores.Red)
                        {
                             Ficha item = new Red(spriteBatch, c1.FichaContenida.Texture, c1.Posicion);                            
                            _dragDropController.Add(item);
                        }
                        else
                        {
                            Ficha item = new Black(spriteBatch, c1.FichaContenida.Texture, c1.Posicion);                            
                            _dragDropController.Add(item);
                        
                        }
                    }
                    
                }
            }
        }

        // Determina si algun jugador se quedo sin fichas
        private bool alguienSeQuedoSinFichas()
        {
            // Determina la cantidad de fichas rojas y negras y se guardan
            int cantFichasRojas = _dragDropController.cantFichasRojas();
            int cantFichasNegras = _dragDropController.cantFichasNegras();

            if (cantFichasNegras == 0 || cantFichasRojas == 0)
            {
                
                return true;

            }
            else
            {
                return false;
            
            }
        
        }

        // Determina si algun jugador no puede moverse
        // @param[in]   color       Este el color del jugador a evaluar
        // @return      true si no se puede mover, false si puede mover
        private bool NoPuedenMoverse(Colores color)
        {
            int cantFichaEnTablero =  0;
            int cantFichasNoPuedenMoverse = 0;
            int cantMovimientosNoSePuedenHacer = 0;

            //Guarda la cantidad de fichas en el tablero dependiendo del color
            if (color == Colores.Black)
                cantFichaEnTablero = _dragDropController.cantFichasNegras();
            else
                cantFichaEnTablero = _dragDropController.cantFichasRojas();

            //Recorro las fichas que estan en el tablero del color especificado
            foreach(var fichaEvaluada in _dragDropController.Items)
            {
                if (fichaEvaluada.Color == color)
                {
                    //Se verifica si la ficha que se esta evaluando se puede mover en la casilla
                    Vector2 posicionAEvaluar = new Vector2(fichaEvaluada.Position.X + 80, fichaEvaluada.Position.Y - 80);

                    if (fichaEvaluada.canMove(fichaEvaluada.Position,posicionAEvaluar) == 1 && _dragDropController.estatusCasilla(posicionAEvaluar).NohayUnaFicha == true)
                        return false;
                    else
                        cantMovimientosNoSePuedenHacer++; //Se incrementa la cantidad de movimientos que no pueden hacerse

                    posicionAEvaluar = new Vector2(fichaEvaluada.Position.X - 80, fichaEvaluada.Position.Y - 80);
                    if (fichaEvaluada.canMove(fichaEvaluada.Position, posicionAEvaluar) == 1 && _dragDropController.estatusCasilla(posicionAEvaluar).NohayUnaFicha == true)
                        return false;
                    else
                        cantMovimientosNoSePuedenHacer++; //Se incrementa la cantidad de movimientos que no pueden hacerse

                    posicionAEvaluar = new Vector2(fichaEvaluada.Position.X + 80, fichaEvaluada.Position.Y + 80);
                    if (fichaEvaluada.canMove(fichaEvaluada.Position, posicionAEvaluar) == 1 && _dragDropController.estatusCasilla(posicionAEvaluar).NohayUnaFicha == true)
                        return false;
                    else
                        cantMovimientosNoSePuedenHacer++; //Se incrementa la cantidad de movimientos que no pueden hacerse

                    posicionAEvaluar = new Vector2(fichaEvaluada.Position.X - 80, fichaEvaluada.Position.Y + 80);
                    if (fichaEvaluada.canMove(fichaEvaluada.Position, posicionAEvaluar) == 1 && _dragDropController.estatusCasilla(posicionAEvaluar).NohayUnaFicha == true)
                        return false;
                    else
                        cantMovimientosNoSePuedenHacer++; //Se incrementa la cantidad de movimientos que no pueden hacerse

                    //Se verifica la cantidad de movimientos que no se pudieron hacer
                    if (cantMovimientosNoSePuedenHacer >= 1)
                        cantFichasNoPuedenMoverse++; //Se incrementa el numero de fichas que no pueden moverse
                
                }


            
            }
                //Se verifica si la cantidad de fichas que no pueden moverse es igual a la cantidad de fichas de ese color en el tablero 
                return cantFichasNoPuedenMoverse == cantFichaEnTablero? true:false;
        
        }

        private bool endGame()
        {
            bool NegrasnoPuedenComer = !_dragDropController.jugadorDebeComer(fichasNegras);
            bool rojasNoPuedenComer = !_dragDropController.jugadorDebeComer(fichasRojas);
            // Se verifica si alguien se quedo sin fichas, si alguien no puede moverse o si alguien no puede comer
            if (alguienSeQuedoSinFichas() || ((NoPuedenMoverse(fichasNegras) && NegrasnoPuedenComer) ||
                (NoPuedenMoverse(fichasRojas) && rojasNoPuedenComer)) )
                return true;
            else
                return false;
        
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //get the current state of the mouse (position, buttons, etc.)
            _currentMouse = Mouse.GetState();

            //remember the mouseposition for use in this Update and subsequent Draw
            _currentMousePosition = new Vector2(_currentMouse.X, _currentMouse.Y);

            
            //Verifica si llego una ficha al lado contrario para convertirla en Reina
            _dragDropController.coronarAReina();

            if (endGame())
            {
                this.Exit();
            
            }

            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();
           // spriteBatch.Draw(texture, new Vector2(50,20), Color.White);
            spriteBatch.Draw( fondoDelJuego, Vector2.Zero, null, Color.White);
           tablero.draw(spriteBatch, _currentMousePosition, _dragDropController);
          foreach (var item in _dragDropController.Items) { item.Draw(gameTime); }
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
