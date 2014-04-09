using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Damas.Model
{
    // Variable de estado del juego
    enum GameState { Start, InGame, GameOver, ayuda };

    // Variable para referirse a los colores
    public enum Colores { White= 0, Black = 1, Red = 2 }

    // Variable para referirse al estado de las casillas
   public struct estatusCasillas
    {
       public bool NohayUnaFicha;
       public Colores colorDeLaFicha;
    
    }
    
}
