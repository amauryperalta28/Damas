using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Damas.Model
{
    enum GameState { Start, InGame, GameOver, ayuda };
    public enum Colores { White= 0, Black = 1, Red = 2 }

   public struct estatusCasillas
    {
       public bool NohayUnaFicha;
        public Colores colorDeLaFicha;
    
    }
    
}
