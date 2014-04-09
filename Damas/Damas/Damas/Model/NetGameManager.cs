using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Net;

namespace Damas.Model
{
  public static  class NetGameManager
    {
      static public Colores colorJugadorLocal;
      static public Colores colorJugadorRemoto;
      static public PacketWriter packetWriter;
      static public PacketReader packetReader;
      static public LocalNetworkGamer localGamer;

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
