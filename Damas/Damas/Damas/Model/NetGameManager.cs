using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework;

namespace Damas.Model
{
  public static  class NetGameManager
    {
      static public Colores colorJugadorHost;
      static public Colores colorJugadorRemoto;

      static public string tipoJugador;
      // Se utiliza para preparar los datos que se van a enviar
      static public PacketWriter packetWriter;
      // Se utiliza para preparar los datos que se van a recibir
      static public PacketReader packetReader;
      //Se utiliza para poder enviar datos al jugador remoto
      static public LocalNetworkGamer localGamer;

      static public Vector2 posInJugadorRemoto;
      static public Vector2 posFinJugadorRemoto;

     public static void InitPacketWriter()
      {
         packetWriter = new PacketWriter();


      }
     public static void InitPacketReader()
     {
         packetReader = new PacketReader();
     
     }

    }

   
    
}
