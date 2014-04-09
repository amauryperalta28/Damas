using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Damas.Model;
using DragAndDrop;

namespace Damas.DragAndDropTools
{
    /// <summary>
    /// Controllerclass for easy drag and drop in XNA
    /// </summary>
    /// <typeparam name="T">The type of items to interact with using the mouse. Must implement IDragAndDropItem</typeparam>
    public class DragAndDropController<T> : DrawableGameComponent  where T : IDragAndDropItem
    {

        #region Variables and properties

        private KeyboardState _keyboard;
        private MouseState _oldMouse, _currentMouse;
        private SpriteBatch _spriteBatch;
        private Vector2 _mouseDown;
        private  List<Ficha> _selectedItems;
        private  List<Ficha> _items;
        private bool _isDraggingRectangle;
        
        public Ficha ItemUnderTheMouseCursor { get; private set; }
        public bool IsThereAnItemUnderTheMouseCursor { get; private set; }

        public IEnumerable<Ficha> Items { get { foreach (var item in _items) { yield return item; } } }
        public IEnumerable<Ficha> SelectedItems { get { foreach (var item in _selectedItems) { yield return item; } } }

        public int Count { get { return _items.Count; } }
        public int SelectedCount { get { return _selectedItems.Count; } }

        private Texture2D _selectionTexture;
        private Tablero _copiaTablero;
        

        private bool MouseWasJustPressed
        {
            get
            {
                return _currentMouse.LeftButton == ButtonState.Pressed && _oldMouse.LeftButton == ButtonState.Released;
            }
        }

        private bool IsGroupAction
        {
            get
            {
                return _keyboard.IsKeyDown(Keys.LeftControl) || _keyboard.IsKeyDown(Keys.RightControl);
            }
        }

        private Vector2 CurrentMousePosition
        {
            get { return new Vector2(_currentMouse.X, _currentMouse.Y); }
        }

        public Vector2 OldMousePosition
        {
            get { return new Vector2(_oldMouse.X, _oldMouse.Y); }
        }

        public Vector2 MouseMovementSinceLastUpdate
        {
            get { return CurrentMousePosition - OldMousePosition; }
        }

        public Ficha ItemUnderMouseCursor()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].Contains(CurrentMousePosition))
                {
                    return _items[i];
                }
            }
            return default(Ficha);
        }

        #endregion

        #region Constructor, Draw() and Update()

        public DragAndDropController(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _selectedItems = new List<Ficha>();
            _items = new List<Ficha>();
            _spriteBatch = spriteBatch;
            _selectionTexture = Game.Content.Load<Texture2D>(@"Images/white");
            _copiaTablero = new Tablero(Game.Content);
            
            
            
        }

        public override void Update(GameTime gameTime)
        {
            GetCurrentMouseState();
            GetCurrentKeyboardState();
            HandleMouseInput();
            HandleKeyboardInput();
            SaveCurrentMouseState();
        }

        public override void Draw(GameTime gameTime)
        {
            if (_isDraggingRectangle)
            {
                Rectangle selectionRectangle = GetSelectionRectangle();
                _spriteBatch.Begin();
                _spriteBatch.Draw(_selectionTexture, selectionRectangle, Color.White * .4f);
                _spriteBatch.End();
            }
        }

        #endregion

        #region implementacion propia de metodos

        /** @brief Determina si en una posicion especificada se encuentra una ficha
         * 
         * @param[in]  posicion         Esta es la posicion que se va a evaluar
         * 
         * @return     true si no hay una casilla, false de lo contrario 
         * 
         */
        public estatusCasillas estatusCasilla(Vector2 posicion)
        {
            estatusCasillas estatusCasillaEvaluada;
            //Verifica Si hay un ficha que tenga la misma posicion a donde me quiero mover
            
            foreach (var casillaEvaluada in _items) 
            {
                // Si hay una ficha en la posicion que estamos evaluando
                if (posicion.X == casillaEvaluada.Position.X && posicion.Y == casillaEvaluada.Position.Y)
                {
                    
                    estatusCasillaEvaluada.NohayUnaFicha = false;
                    estatusCasillaEvaluada.colorDeLaFicha = casillaEvaluada.Color;

                    return estatusCasillaEvaluada;
                }
                
            }
            estatusCasillaEvaluada.NohayUnaFicha = true;
            estatusCasillaEvaluada.colorDeLaFicha = Colores.White;
            return estatusCasillaEvaluada;
        }

        /** @brief Convierte en Reina la ficha que llega a la primera fila del oponente
         * 
         * @return      no retorna nada
         */
        public void coronarAReina()
        {
            //Se Verifica el lado del oponente negro
            for(int i =  _items.Count - 1; i >= 0; i--) 
            {
                for (int x = 70; x <= 630; x = x + 80)
                {
                    Ficha item = _items.ElementAt(i);
                    Vector2 posAEvaluar = new Vector2(x, 20);
                    if ( item.Position.Equals(posAEvaluar) && (item is Red))
                    {
                        Remove(item);
                        RedQueen reina = new RedQueen(_spriteBatch, Game.Content, posAEvaluar);
                         Add(reina);
                    }
                    
                }                   

            }

            //Se Verifica el lado del oponente Rojo
            for (int j = _items.Count - 1; j >= 0; j--)
            {
                for (int x1 = 70; x1 <= 630; x1 = x1 + 80)
                {
                    Ficha item = _items.ElementAt(j);
                    Vector2 posAEvaluar = new Vector2(x1, 580);
                    if (item.Position.Equals(posAEvaluar) &&  (item is Black))
                    {
                        Remove(item);
                        BlackQueen reina = new BlackQueen(_spriteBatch, Game.Content, posAEvaluar);
                        Add(reina);
                    }

                }

            }
            
            
            
        
        }

        /** @brief   Determina la cantidad de fichas rojas que hay en el tablero
         * 
         * @return      El numero de fichas rojas
         */
       public int cantFichasRojas()
        {
            int contadorFichas=0;
            foreach(var item in _items)
            {
                if(item.Color == Colores.Red)
                    contadorFichas++;           
            }

            return contadorFichas;
        
        }

       /** @brief   Determina la cantidad de fichas negras que hay en el tablero
        * 
        * @return      El numero de fichas negras
        */
       public int cantFichasNegras()
       {
           int contadorFichas = 0;
           foreach (var item in _items)
           {
               if (item.Color == Colores.Black)
                   contadorFichas++;
           }

           return contadorFichas;

       }

        /** @brief  Determina si una ficha puede comer otra ficha
         *         Si puede realizar una movida para comer otra ficha, esta se agrega en la lista
         *         de posiciones en la que puede comer la ficha.
         * 
         * 
         * @param[in]  fichaAEvaluar        ficha a evaluar
         * 
         * @return                  true si puede comer, false de lo contrario
         * 
         */
        public bool fichaPuedeComer(Ficha fichaAEvaluar)
        {
            int posEnLasQuePuedeComer = 0;
            Vector2 posicionAEvaluar1 = new Vector2(fichaAEvaluar.Position.X + 80,fichaAEvaluar.Position.Y + 80 );            
            Vector2 posicionAEvaluar2 = new Vector2(fichaAEvaluar.Position.X - 80,fichaAEvaluar.Position.Y + 80 );
            Vector2 posicionAEvaluar3 = new Vector2(fichaAEvaluar.Position.X - 80,fichaAEvaluar.Position.Y - 80 );
            Vector2 posicionAEvaluar4 = new Vector2(fichaAEvaluar.Position.X + 80,fichaAEvaluar.Position.Y - 80 );

            Vector2 posDespuesDeComer1 = new Vector2(fichaAEvaluar.Position.X + 160,fichaAEvaluar.Position.Y + 160 );            
            Vector2 posDespuesDeComer2 = new Vector2(fichaAEvaluar.Position.X - 160,fichaAEvaluar.Position.Y + 160 );
            Vector2 posDespuesDeComer3 = new Vector2(fichaAEvaluar.Position.X - 160,fichaAEvaluar.Position.Y - 160 );
            Vector2 posDespuesDeComer4 = new Vector2(fichaAEvaluar.Position.X + 160,fichaAEvaluar.Position.Y - 160 );
            
            //Se verifica si la ficha no se puede mover porque hay una ficha de otro color impidiendole avanzar
            //y si puede comersela
            if (fichaAEvaluar.canMove(fichaAEvaluar.Position, posicionAEvaluar1)==1 && estatusCasilla(posicionAEvaluar1).NohayUnaFicha == false &&
               estatusCasilla(posicionAEvaluar1).colorDeLaFicha != fichaAEvaluar.Color && estatusCasilla(posDespuesDeComer1).NohayUnaFicha == true)
            {
                
                // Si se puede comer, inserta la posicion en la lista de posiciones en las que puede comer la ficha
                if (fichaAEvaluar.estaDentroDelTablero(posDespuesDeComer1.X, posDespuesDeComer1.Y) == 1)
                {
                    fichaAEvaluar.addJugadaParaComerFicha(posDespuesDeComer1);
                    posEnLasQuePuedeComer++;
                }
                    
            }
             if (fichaAEvaluar.canMove(fichaAEvaluar.Position, posicionAEvaluar2)==1 && estatusCasilla(posicionAEvaluar2).NohayUnaFicha == false &&
               estatusCasilla(posicionAEvaluar2).colorDeLaFicha != fichaAEvaluar.Color && estatusCasilla(posDespuesDeComer2).NohayUnaFicha == true)
            {
                
                // Si se puede comer, inserta la posicion en la lista de posiciones en las que puede comer la ficha
                if (fichaAEvaluar.estaDentroDelTablero(posDespuesDeComer2.X, posDespuesDeComer2.Y) == 1)
                {
                    fichaAEvaluar.addJugadaParaComerFicha(posDespuesDeComer2);
                    posEnLasQuePuedeComer++;
                }
                       
 
            }
             if (fichaAEvaluar.canMove(fichaAEvaluar.Position, posicionAEvaluar3)==1 && estatusCasilla(posicionAEvaluar3).NohayUnaFicha == false &&
               estatusCasilla(posicionAEvaluar3).colorDeLaFicha != fichaAEvaluar.Color && estatusCasilla(posDespuesDeComer3).NohayUnaFicha == true)
            {
                
                // Si se puede comer, inserta la posicion en la lista de posiciones en las que puede comer la ficha
                if (fichaAEvaluar.estaDentroDelTablero(posDespuesDeComer3.X, posDespuesDeComer3.Y) == 1)
                {
                    fichaAEvaluar.addJugadaParaComerFicha(posDespuesDeComer3);
                    posEnLasQuePuedeComer++;
                }
                    
            }
             if (fichaAEvaluar.canMove(fichaAEvaluar.Position, posicionAEvaluar4)==1 && estatusCasilla(posicionAEvaluar4).NohayUnaFicha == false &&
               estatusCasilla(posicionAEvaluar4).colorDeLaFicha != fichaAEvaluar.Color && estatusCasilla(posDespuesDeComer4).NohayUnaFicha == true)
            {
                
                // Si se puede comer, inserta la posicion en la lista de posiciones en las que puede comer la ficha
                if (fichaAEvaluar.estaDentroDelTablero(posDespuesDeComer4.X, posDespuesDeComer4.Y) == 1)
                {
                    fichaAEvaluar.addJugadaParaComerFicha(posDespuesDeComer4);
                    posEnLasQuePuedeComer++;
                }
                    
            }

             return posEnLasQuePuedeComer > 0 ? true : false;
        }

        /** @brief  Determina si un jugador debe comer otra ficha
         * 
         * @param[in]  colorJugador        color del jugador
         * 
         * @return                  true si debe que comer, false de lo contrario
         * 
         */
        public bool jugadorDebeComer(Colores colorJugador)
        {
            //Recorro las fichas que se encuentran en el tablero
            foreach(Ficha fichaAEvaluar in _items)
            {
                //Verifico si es del color del jugador indicado
                if (fichaAEvaluar.Color.Equals(colorJugador) && fichaPuedeComer(fichaAEvaluar) == true)
                {
                    return true;
                
                }

            }
            return false;
        
        }

        /** @brief Determina la posicion de la ficha que debe eliminarse y la elimina
         * 
         * @param[in] posInicial            Esta es la posicion inicial de la ficha
         * @param[in] posFinal              Esta es la posicion final de la ficha
         * 
         * @return    no retorna nada
         * 
         */
        public void identificarYEliminarFicha(Vector2 posInicial, Vector2 posFinal)
        {
            Vector2 posFichaAEliminar= new Vector2(0,0);

            if (posFinal.X == posInicial.X + 160 && posFinal.Y == posInicial.Y - 160)
                    posFichaAEliminar = new Vector2(posInicial.X + 80, posInicial.Y - 80);

            else if (posFinal.X == posInicial.X - 160 && posFinal.Y == posInicial.Y - 160)
                    posFichaAEliminar = new Vector2(posInicial.X - 80, posInicial.Y - 80);

            else if (posFinal.X == posInicial.X + 160 && posFinal.Y == posInicial.Y + 160)
                    posFichaAEliminar = new Vector2(posInicial.X + 80, posInicial.Y + 80);

            else if (posFinal.X == posInicial.X - 160 && posFinal.Y == posInicial.Y + 160)
                    posFichaAEliminar = new Vector2(posInicial.X - 80, posInicial.Y + 80);

            //Recorro las fichas del tablero 
          
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                for (int x = 70; x <= 630; x = x + 80)
                {
                    Ficha item = _items.ElementAt(i);
                   
                    if (item.Position.Equals(posFichaAEliminar) )
                    {
                        Remove(item);
                        return;
                                             
                    }

                }

            }
        }

        #endregion
     
               

        #region public interaction methods

        public void Add(Ficha item) { _items.Add(item); }
        public void Remove(Ficha item) { _items.Remove(item); _selectedItems.Remove(item); }

        public void DeselectAll()
        {
            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                DeselectItem(_selectedItems[i]);
            }
        }

        public void SelectAll()
        {
            _items.ForEach(SelectItem);
        }

        public void Clear()
        {
            _selectedItems.Clear();
            _items.Clear();
        }

        public void RemoveSelected()
        {
            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                _items.Remove(_selectedItems[i]);
                _selectedItems.Remove(_selectedItems[i]);
            }
        }

        #endregion

        #region Private helpermethods

        private Rectangle GetSelectionRectangle()
        {
            int left = (int)MathHelper.Min(_mouseDown.X, CurrentMousePosition.X);
            int right = (int)MathHelper.Max(_mouseDown.X, CurrentMousePosition.X);
            int top = (int)MathHelper.Min(_mouseDown.Y, CurrentMousePosition.Y);
            int bottom = (int)MathHelper.Max(_mouseDown.Y, CurrentMousePosition.Y);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        private void HandleKeyboardInput()
        {
            if (_keyboard.IsKeyDown(Keys.A) && (_keyboard.IsKeyDown(Keys.LeftControl) || _keyboard.IsKeyDown(Keys.RightControl)))
            {
                SelectAll();
            }
            if (_keyboard.IsKeyDown(Keys.Delete) && _selectedItems.Count > 0)
            {
                RemoveSelected();
            }
        }

        private void GetCurrentMouseState() { _currentMouse = Mouse.GetState(); }
        private void GetCurrentKeyboardState() { _keyboard = Keyboard.GetState(); }

        private void HandleMouseInput()
        {
            SetAllItemsToIsMouseOverFalse();
            ItemUnderTheMouseCursor = ItemUnderMouseCursor();
            if (!Equals(ItemUnderTheMouseCursor, default(T)))
            {
                UpdateItemUnderMouse();
            }
            else
            {
                if (MouseWasJustPressed) 
                {
                    DeselectAll();
                    _mouseDown = CurrentMousePosition;
                    // Se quito la opcion de que se dibuje un rectangulo de seleccion
                   // _isDraggingRectangle = true; 
                }
            }

            if (_currentMouse.LeftButton == ButtonState.Released)
            {
                _isDraggingRectangle = false;
            }
            else
            {
                if (_isDraggingRectangle)
                {
                    Rectangle selectionRectangle = GetSelectionRectangle();
                    DeselectAll();
                    _items.Where(item => selectionRectangle.Contains(item.Border)).ToList().ForEach(SelectItem);
                }
                else
                {   //Se quito la opcion de poder arrastrar las fichas
                    //MoveSelectedItemsIfMouseButtonIsPressed();
                }
            }
        }

        private void SetAllItemsToIsMouseOverFalse()
        {
            _items.ForEach(item => item.IsMouseOver = false);
        }

        private void MoveSelectedItemsIfMouseButtonIsPressed()
        {
            if (_currentMouse.LeftButton == ButtonState.Pressed)
            {
                _selectedItems.ForEach(item => item.Position += MouseMovementSinceLastUpdate);
            }
        }

        private void UpdateItemUnderMouse()
        {
            ItemUnderTheMouseCursor.IsMouseOver = true;

            if (MouseWasJustPressed)
            {
                IfItemUnderMouseIsNotInSelectedGroupThenDeselectAll();
                SelectItem(ItemUnderTheMouseCursor);
            }
        }

        private void IfItemUnderMouseIsNotInSelectedGroupThenDeselectAll()
        {
            if (!IsGroupAction && !_selectedItems.Contains(ItemUnderTheMouseCursor))
            {
                DeselectAll();
            }
        }

        private void SaveCurrentMouseState() { _oldMouse = _currentMouse; }

        private void SelectItem(Ficha itemToSelect)
        {
            
           
                //Se verifica si es el turno del jugador rojo para jugar
                if (itemToSelect.Color.Equals(Colores.Red) && manejadorDeTurnos.turnoJugadorRojo == true)
                {
                    itemToSelect.IsSelected = true;
                    _selectedItems.Add(itemToSelect);
                    
                }
                //Se verifica si es el turno del jugador negro para jugar
                else if (itemToSelect.Color.Equals(Colores.Black) && manejadorDeTurnos.turnoJugadorNegro == true)
                {
                    itemToSelect.IsSelected = true;
                    _selectedItems.Add(itemToSelect);
                    
                }
          

            
        }

        private void DeselectItem(Ficha itemToDeselect)
        {
            itemToDeselect.IsSelected = false;
            bool canNotMove = true;                      

            Ficha fichaSeleccionada = ((Ficha)itemToDeselect);

            Vector2 posFichaSeleccionada = itemToDeselect.Position;
            
            Vector2 posDestino = CurrentMousePosition;
           
            // Se crea un tablero para saber la posicion de la casilla en la que se dio click
            Tablero t1 = new Tablero(Game.Content, _spriteBatch);

            for (int y = 0; y < t1.Casillas.GetLength(0); y++)
            {
                for (int x = 0; x < t1.Casillas.GetLength(1); x++)
                {
                    Casilla casillaEvaluada = t1.Casillas[x, y];

                    /*  Si el puntero esta dentro de una casilla, ponle la posicion de esa casilla
                     *  al objeto seleccionado.  */
                    if ((posDestino.X > casillaEvaluada.Posicion.X && posDestino.X <= casillaEvaluada.Posicion.X + 80) && (posDestino.Y > casillaEvaluada.Posicion.Y && posDestino.Y <= casillaEvaluada.Posicion.Y + 80))
                    {
                             // Se verifica si la ficha seleccionada tiene que comer otra ficha                   
                           if (jugadorDebeComer(fichaSeleccionada.Color) && fichaPuedeComer(fichaSeleccionada) && fichaSeleccionada.esJugadaParaComerFicha(casillaEvaluada.Posicion))
                             {
                                 posDestino = casillaEvaluada.Posicion;

                                 // Se elimina la ficha comida
                                 identificarYEliminarFicha(fichaSeleccionada.Position, casillaEvaluada.Posicion);
                                 itemToDeselect.Position = posDestino;
                                 canNotMove = false;

                                   // Se verifica el color para ceder el turno al otro jugador
                                     if (fichaSeleccionada.Color == Colores.Red)
                                     {
                                             // Se verifica si la ficha puede seguir comiendo
                                             if (fichaPuedeComer(itemToDeselect) == false)
                                             {
                                                 manejadorDeTurnos.turnoJugadorRojo = false;
                                                 manejadorDeTurnos.turnoJugadorNegro = true;
                                             }
                                     }
                                     else
                                     {
                                             // Se verifica si la ficha puede seguir comiendo
                                             if (fichaPuedeComer(itemToDeselect) == false)
                                             {
                                                 manejadorDeTurnos.turnoJugadorNegro = false;
                                                 manejadorDeTurnos.turnoJugadorRojo = true;
                                             }
                                 
                                     }

                        
                             }
                            else if (fichaSeleccionada.canMove(posFichaSeleccionada, casillaEvaluada.Posicion) == 1 && estatusCasilla(casillaEvaluada.Posicion).NohayUnaFicha && !jugadorDebeComer(fichaSeleccionada.Color))
                            {
                                    posDestino = casillaEvaluada.Posicion;
                                    itemToDeselect.Position = posDestino;
                                    canNotMove = false;

                                    // Se verifica el color para ceder el turno al otro jugador
                                    if (fichaSeleccionada.Color == Colores.Red)
                                    {
                                        manejadorDeTurnos.turnoJugadorRojo = false;
                                        manejadorDeTurnos.turnoJugadorNegro = true;
                                    }
                                    else
                                    {
                                        manejadorDeTurnos.turnoJugadorNegro = false;
                                        manejadorDeTurnos.turnoJugadorRojo = true;
                                    }

                            
                            }
                    }
                }
            
            }
               if(canNotMove)
                 itemToDeselect.Position = posFichaSeleccionada;
            
            _selectedItems.Remove(itemToDeselect);
            fichaSeleccionada.removeJugadasParaComerFicha();
        } 
        #endregion

    }
}