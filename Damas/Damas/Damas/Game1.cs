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
        Texture2D texture;

        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;
        private DragAndDropController<Item> _dragDropController;
        Tablero t1;
       
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
            
            t1 = new Tablero(Content, spriteBatch);
            SetupDraggableItems();
            texture = Content.Load<Texture2D>(@"Images/ficha1");

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
                    Casilla c1 = t1.casillas[j, i];
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
           t1.draw(spriteBatch, _currentMousePosition);
          foreach (var item in _dragDropController.Items) { item.Draw(gameTime); }
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
