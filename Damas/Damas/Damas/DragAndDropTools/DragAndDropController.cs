using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using DragAndDrop.Model;
using Damas.Model;

namespace DragAndDrop
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
             
        
        /* @brief Determina si en una posicion especificada se encuentra una ficha
         * 
         * @param[in]  posicion         Esta es la posicion que se va a evaluar
         * 
         * @return     true si no hay una casilla, false de lo contrario 
         * 
         */
        public bool noHayUnaFichaEnLaCasilla(Vector2 posicion)
        {
            //Verifica Si hay un ficha que tenga la misma posicion a donde me quiero mover
            
            foreach (var item in _items) 
            {
                if (posicion.X == item.Position.X && posicion.Y == item.Position.Y)
                {
                    return false;
                }
                
            }

            return true;
        }

        /* @brief Convierte en Reina la ficha que llega a la primera fila del oponente
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

        /* @brief   Determina la cantidad de fichas rojas que hay en el tablero
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

       /* @brief   Determina la cantidad de fichas negras que hay en el tablero
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

        /* @brief  Determina si el jugador esta obligado a comer una ficha
         * 
         * @param[in]  ficha        ficha a la que se le toma el color
         * 
         * @return                  true si hay que comer, false de lo contrario
         * 
         */
        public bool hayQueComer(Colores color)
        { 

            return true; 
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
            itemToSelect.IsSelected = true;
            if (!_selectedItems.Contains(itemToSelect))
            {
                _selectedItems.Add(itemToSelect);
            }

            
        }

        private void DeselectItem(Ficha itemToDeselect)
        {
            itemToDeselect.IsSelected = false;
            bool canNotMove = true;

            

            Ficha fichaSeleccionada = ((Ficha)itemToDeselect);

            Vector2 posFichaSeleccionada = itemToDeselect.Position;

            
            
            Vector2 pos = CurrentMousePosition;
           
            // Se crea un tablero para saber la posicion de la casilla en la que se dio click
            Tablero t1 = new Tablero(Game.Content, _spriteBatch);

            for (int y = 0; y < t1.casillas.GetLength(0); y++)
            {
                for (int x = 0; x < t1.casillas.GetLength(1); x++)
                {
                    Casilla c1 = t1.casillas[x, y];

                    /*  Si el puntero esta dentro de una casilla, ponle la posicion de esa casilla
                     *  al objeto seleccionado.  */
                    if ((pos.X > c1.Posicion.X && pos.X <= c1.Posicion.X + 80) && (pos.Y > c1.Posicion.Y && pos.Y <= c1.Posicion.Y + 80))
                    {
                       
                        if (fichaSeleccionada.canMove(posFichaSeleccionada, c1.Posicion) == 1 && noHayUnaFichaEnLaCasilla(c1.Posicion))
                        {
                            pos = c1.Posicion;
                            itemToDeselect.Position = pos;
                            canNotMove = false;
                        }
                    }
                }
            
            }
               if(canNotMove)
               itemToDeselect.Position = posFichaSeleccionada;
            
            _selectedItems.Remove(itemToDeselect);
        } 
        #endregion

    }
}