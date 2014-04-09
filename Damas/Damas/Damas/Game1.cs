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
using Microsoft.Xna.Framework.Net;
using Damas.DragAndDropTools;

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
        // Fonts
        SpriteFont scoreFont;

        public enum GameState { SignIn, FindSession, CreateSession, 
                                Start, InGame, GameOver}

       

        // Network stuff
        NetworkSession networkSession;
        
        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();

        GameState currentGameState = GameState.SignIn;

        Colores fichasNegras = Colores.Black;
        Colores fichasRojas = Colores.Red;

        private DragAndDropControllerNET<Item> _dragDropController;
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
            Components.Add(new GamerServicesComponent(this));

            //Network stuff
            NetGameManager.InitPacketReader();
            NetGameManager.InitPacketWriter();
            NetGameManager.posInJugadorRemoto = new Vector2(0,0);
            NetGameManager.posFinJugadorRemoto = new Vector2(0, 0);

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
       
            _dragDropController = new DragAndDropControllerNET<Item>(this, spriteBatch);
            Components.Add(_dragDropController);
            
            tablero = new Tablero(Content, spriteBatch);
            SetupDraggableItems();
            fondoDelJuego = Content.Load<Texture2D>(@"Images/fondo");
            scoreFont = Content.Load<SpriteFont>(@"Fonts/ScoreFont");

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
           
                            

            //get the current state of the mouse (position, buttons, etc.)
            _currentMouse = Mouse.GetState();

            //remember the mouseposition for use in this Update and subsequent Draw
            _currentMousePosition = new Vector2(_currentMouse.X, _currentMouse.Y);

            
            //Verifica si llego una ficha al lado contrario para convertirla en Reina
            _dragDropController.coronarAReina();

            // Only run the Update code if the game is currently active.
            // This prevents the game from progressing while
            // gamer services windows are open.
            if (this.IsActive)
            {
                // Run different methods based on game state
                switch (currentGameState)
                {
                    case GameState.SignIn:
                        Update_SignIn();
                        break;
                    case GameState.FindSession:
                        Update_FindSession();
                        break;
                    case GameState.CreateSession:
                        Update_CreateSession();
                        break;
                    case GameState.Start:
                        Update_Start(gameTime);
                        break;
                    case GameState.InGame:
                        Update_InGame(gameTime);
                        break;
                    case GameState.GameOver:
                        Update_GameOver(gameTime);
                        break;
                }
            }
            // Update the network session and pump network messages
            if (networkSession != null)
                networkSession.Update();

            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Only draw when game is active
            if (this.IsActive)
            {
                // Based on the current game state,
                // call the appropriate method
                switch (currentGameState)
                {
                    case GameState.SignIn:
                    case GameState.FindSession:
                    case GameState.CreateSession:
                        GraphicsDevice.Clear(Color.DarkBlue);
                        break;
                    case GameState.Start:
                        DrawStartScreen();
                        break;
                    case GameState.InGame:
                        DrawInGameScreen(gameTime);
                        break;
                    case GameState.GameOver:
                       // DrawGameOverScreen();
                        break;
                }
            }
            //GraphicsDevice.Clear(Color.Gray);

            //spriteBatch.Begin();
           // spriteBatch.Draw(texture, new Vector2(50,20), Color.White);
           // spriteBatch.Draw( fondoDelJuego, Vector2.Zero, null, Color.White);
           //tablero.draw(spriteBatch, _currentMousePosition, _dragDropController);
          //foreach (var item in _dragDropController.Items) { item.Draw(gameTime); }
            //spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        private void DrawStartScreen( )
        {
            // Clear screen
            GraphicsDevice.Clear(Color.AliceBlue);
            // Draw text for intro splash screen
            spriteBatch.Begin( );
            // Draw instructions
            string text = "The guest player is Red color\n";
            text += networkSession.Host.Gamertag + " is the Black color";
            spriteBatch.DrawString(scoreFont, text,
            new Vector2((Window.ClientBounds.Width / 2)
            - (scoreFont.MeasureString(text).X / 2),
            (Window.ClientBounds.Height / 2)
            - (scoreFont.MeasureString(text).Y / 2)),
            Color.SaddleBrown);
            // If both gamers are there, tell gamers to press space bar or Start to begin
            if (networkSession.AllGamers.Count == 2)
            {
                text = "(Game is ready. Press Spacebar or Start button to begin)";
                spriteBatch.DrawString(scoreFont, text,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(text).X / 2),
                (Window.ClientBounds.Height / 2)
                - (scoreFont.MeasureString(text).Y / 2) + 60),
                Color.SaddleBrown);
            }
            // If only one player is there, tell gamer you're waiting for players
            else
            {
                text = "(Waiting for players)";
                spriteBatch.DrawString(scoreFont, text,
                new Vector2((Window.ClientBounds.Width / 2)
                - (scoreFont.MeasureString(text).X / 2),
                (Window.ClientBounds.Height / 2) + 60),
                Color.SaddleBrown);
            }
            // Loop through all gamers and get their gamertags,
            // then draw list of all gamers currently in the game
            text = "\n\nCurrent Player(s):";
            foreach (Gamer gamer in networkSession.AllGamers)
            {
                text += "\n" + gamer.Gamertag;
            }
            spriteBatch.DrawString(scoreFont, text,
            new Vector2((Window.ClientBounds.Width / 2)
            - (scoreFont.MeasureString(text).X / 2),
            (Window.ClientBounds.Height / 2) + 90),
            Color.SaddleBrown);
            spriteBatch.End();
        }

        private void DrawInGameScreen(GameTime gameTime)
        {
            // Clear device
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin( );

            spriteBatch.Draw(fondoDelJuego, Vector2.Zero, null, Color.White);
            tablero.draw(spriteBatch, _currentMousePosition, _dragDropController);
            foreach (var item in _dragDropController.Items) { item.Draw(gameTime); }
            spriteBatch.End();
        }

        # region Relacionado con el juego en redes

        private void Update_GameOver(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState( );
            GamePadState gamePadSate = GamePad.GetState(PlayerIndex.One);
            // If player presses Enter or A button, restart game
            if (keyboardState.IsKeyDown(Keys.Enter) ||
            gamePadSate.Buttons.A == ButtonState.Pressed)
            {
                // Send restart game message
                packetWriter.Write((int)MessageType.RestartGame);
                networkSession.LocalGamers[0].SendData(packetWriter,
                SendDataOptions.Reliable);
                RestartGame();
            }
            // If player presses Escape or B button, rejoin lobby
            if (keyboardState.IsKeyDown(Keys.Escape) ||
            gamePadSate.Buttons.B == ButtonState.Pressed)
            {
                // Send rejoin lobby message
                packetWriter.Write((int)MessageType.RejoinLobby);
                networkSession.LocalGamers[0].SendData(packetWriter,
                SendDataOptions.Reliable);
                RejoinLobby();
            }
            // Read any incoming messages
            ProcessIncomingData(gameTime);
        }
        private void Update_InGame(GameTime gameTime)
        {
            // Update the local player
           
            // Read any incoming data
            ProcessIncomingData(gameTime);
            // Only host checks for collisions
            /*if (networkSession.IsHost)
            {*/
                //get the current state of the mouse (position, buttons, etc.)
                _currentMouse = Mouse.GetState();

                //remember the mouseposition for use in this Update and subsequent Draw
                _currentMousePosition = new Vector2(_currentMouse.X, _currentMouse.Y);


                //Verifica si llego una ficha al lado contrario para convertirla en Reina
                _dragDropController.coronarAReina();
               
            //}
        }
        protected void Update_SignIn()
        {
            // If no local gamers are signed in, show sign-in screen
            if (Gamer.SignedInGamers.Count < 1)
            {
                Guide.ShowSignIn(1, false);
            }
            else
            {
                // Local gamer signed in, move to find sessions
                currentGameState = GameState.FindSession;
            }
        }

        private void Update_FindSession()
        {
            // Find sessions of the current game
            AvailableNetworkSessionCollection sessions =
            NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);
            if (sessions.Count == 0)
            {
                // If no sessions exist, move to the CreateSession game state
                currentGameState = GameState.CreateSession;
            }
            else
            {
                // If a session does exist, join it, wire up events,
                // and move to the Start game state
                networkSession = NetworkSession.Join(sessions[0]);
                WireUpEvents();
                currentGameState = GameState.Start;
            }
        }

        private void Update_CreateSession()
        {
            // Create a new session using SystemLink with a max of 1 local player
            // and a max of 2 total players
            networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
            networkSession.AllowHostMigration = true;
            networkSession.AllowJoinInProgress = false;
            // Wire up events and move to the Start game state
            WireUpEvents();
            currentGameState = GameState.Start;
        }

        private void Update_Start(GameTime gameTime)
        {
            // Get local gamer
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];
            // Check for game start key or button press
            // only if there are two players
            if (networkSession.AllGamers.Count == 2)
            {
                // If space bar or Start button is pressed, begin the game
                if (Keyboard.GetState( ).IsKeyDown(Keys.Space) ||
                    GamePad.GetState(PlayerIndex.One).Buttons.Start ==
                    ButtonState.Pressed)
                    {   // Send message to other player that we're starting
                        packetWriter.Write((int)MessageType.StartGame);
                        localGamer.SendData(packetWriter, SendDataOptions.Reliable);
                        // Call StartGame
                        StartGame();
                        
                    }
           }
            // Process any incoming packets
            ProcessIncomingData(gameTime);
        }
        protected void StartGame()
        {
            // Set game state to InGame
            currentGameState = GameState.InGame;
            
        }
        private void RejoinLobby()
        {
            // Switch dynamite and gears sprites
            // as well as chaser versus chased
           // SwitchPlayersAndReset(false);
            currentGameState = GameState.Start;
        }
        private void RestartGame()
        {
            // Switch dynamite and gears sprites
            // as well as chaser versus chased
            //SwitchPlayersAndReset(true);
            StartGame();
        }

        protected void UpdateBoard()
        {
            Vector2 posReferencia = new Vector2(0, 0);
            //Se recibe los posicion origen y la posicion destino de la ficha que se movio
            

            if (NetGameManager.posInJugadorRemoto.Equals(posReferencia))
            {
                //Se recibe la posicion inicial
                Vector2 posicion = NetGameManager.packetReader.ReadVector2();
                NetGameManager.posInJugadorRemoto = posicion;
            }
            else if (NetGameManager.posFinJugadorRemoto.Equals(posReferencia))
            {
                //Se recibe la posicion final
                Vector2 posicion = NetGameManager.packetReader.ReadVector2();
                NetGameManager.posFinJugadorRemoto = posicion;

                //Se mueve la ficha indicada en el tablero donde dijo el jugador remoto
                _dragDropController.moverFicha(NetGameManager.posInJugadorRemoto, NetGameManager.posFinJugadorRemoto);

                //Se eliminan las posiciones guardas
                NetGameManager.posInJugadorRemoto = posReferencia;
                NetGameManager.posFinJugadorRemoto = posReferencia;
                
            }
            
           
            

            

        
        }
        protected void UpdateTurns()
        { 
            //Se recibe el cambio de turno del otro jugador
            int turnoJugadorRojo = NetGameManager.packetReader.ReadInt32();
           // Boolean turnoJugadorRojo = NetGameManager.packetReader.ReadBoolean();
            

            // Se verifica si es el turno del jugador de color rojo
            if (turnoJugadorRojo == 1)
            {
                manejadorDeTurnos.turnoJugadorRojo = true;
                manejadorDeTurnos.turnoJugadorNegro = false;
            }
            else
            {
                manejadorDeTurnos.turnoJugadorRojo = false;
                manejadorDeTurnos.turnoJugadorNegro = true;
            
            }

        
        }
      

        protected NetworkGamer GetOtherPlayer()
        {
            // Search through the list of players and find the
            // one that's remote
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                if (!gamer.IsLocal)
                {
                    return gamer;
                }
            }
            return null;
        }
        protected void ProcessIncomingData(GameTime gameTime)
        {
            // Process incoming data
            LocalNetworkGamer localGamer = networkSession.LocalGamers[0];
            NetGameManager.localGamer = networkSession.LocalGamers[0];
            // While there are packets to be read...
            while (localGamer.IsDataAvailable)
            {
                // Get the packet
                NetworkGamer sender;
                localGamer.ReceiveData(packetReader, out sender);
                // Ignore the packet if you sent it
                if (!sender.IsLocal)
                {
                    // Read messagetype from start of packet
                    // and call appropriate method
                    MessageType messageType = (MessageType)packetReader.ReadInt32();
                    switch (messageType)
                    {
                        case MessageType.EndGame:
                            EndGame();
                            break;
                        case MessageType.StartGame:
                            StartGame();
                            break;
                        case MessageType.RejoinLobby:
                            RejoinLobby();
                            break;
                        case MessageType.RestartGame:
                            RestartGame();
                            break;
                        case MessageType.UpdatePlayerPos:
                          //  UpdateRemotePlayer(gameTime);
                            break;
                        case MessageType.UpdateTurnos:
                            UpdateTurns();
                            break;
                        case MessageType.UpdateTablero:
                            UpdateBoard();
                            break;
                    }
                }
            }
        }

        protected void EndGame()
        {
            
            currentGameState = GameState.GameOver;
        }
        protected void WireUpEvents()
        {
            // Wire up events for gamers joining and leaving
            networkSession.GamerJoined += GamerJoined;
            networkSession.GamerLeft += GamerLeft;
        }
        void GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            // Gamer joined. Set the tag for the gamer to a new UserControlledSprite.
            // If the gamer is the host, create a chaser; if not, create a chased.
            if (e.Gamer.IsHost)
            {
                Colores colorJugador = Colores.Black;
                NetGameManager.colorJugadorHost = colorJugador;
                NetGameManager.colorJugadorRemoto = Colores.Red;

                // Se deciden los turnos
                manejadorDeTurnos.turnoJugadorRojo = false;
                manejadorDeTurnos.turnoJugadorNegro = true;

                // Se indica el tipo de jugador que es
                NetGameManager.tipoJugador = "host";
                e.Gamer.Tag = NetGameManager.colorJugadorHost;
            }
            else
            {
                Colores colorJugador = Colores.Black;
                NetGameManager.colorJugadorHost = colorJugador;
                NetGameManager.colorJugadorRemoto = Colores.Red;

                // Se deciden los turnos
                manejadorDeTurnos.turnoJugadorRojo = true;
                manejadorDeTurnos.turnoJugadorNegro = false;

                // Se indica el tipo de jugador que es
                NetGameManager.tipoJugador = "remoto";
                e.Gamer.Tag = NetGameManager.colorJugadorRemoto;
            }
        }

        void GamerLeft(object sender, GamerLeftEventArgs e)
        {
            // Dispose of the network session, set it to null.
            // Stop the soundtrack and go
            // back to searching for sessions.
            networkSession.Dispose( );
            networkSession = null;
           // trackInstance.Stop();
            currentGameState = GameState.FindSession;
        }

        #endregion
    }
}
