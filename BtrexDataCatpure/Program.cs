using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BtrexDataCapture.Interface;
using BtrexDataCapture.Data;

namespace BtrexDataCapture
{
    class Program
    {
        private static BtrexWebSocket websocket1 = new BtrexWebSocket();
        private static BtrexWebSocket websocket2 = new BtrexWebSocket();
        private static BtrexWebSocket websocket3 = new BtrexWebSocket();

        public static IReadOnlyList<string> SubSpecificDeltas1 = new List<string>()
        {
            "BTC-OMG"
        };

        public static IReadOnlyList<string> SubSpecificDeltas2 = new List<string>()
        {
            "BTC-XLM"
        };

        public static IReadOnlyList<string> SubSpecificDeltas3 = new List<string>()
        {
            "BTC-NEO"
        };


        static void Main(string[] args)
        {
            Console.BufferHeight = 9999;
            
            PrintTitle();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.BackgroundColor = ConsoleColor.Black;
                        
            RunAsync().Wait(); 
                        
        }


        static async Task RunAsync()
        {

            BtrexData.NewData();


            Console.Write("Connecting websocket1...");
            await websocket1.Connect();
            Console.WriteLine("DONE");
            Console.Write("Connecting websocket2...");
            await websocket2.Connect();
            Console.WriteLine("DONE");
            Console.Write("Connecting websocket3...");
            await websocket3.Connect();
            Console.WriteLine("DONE");
            await websocket1.SubscribeMarketsList(SubSpecificDeltas1.ToList());
            await websocket2.SubscribeMarketsList(SubSpecificDeltas2.ToList());
            await websocket3.SubscribeMarketsList(SubSpecificDeltas3.ToList());


            //START DATA THREAD
            await BtrexData.StartDataUpdates();


         

            Console.WriteLine("\r\n\r\n-PRESS ENTER 3 TIMES TO EXIT-\r\n\r\n");
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Environment.Exit(0);
        }



        private static void PrintTitle()
        {
            Console.SetWindowSize(120, 40);
            Console.WriteLine("\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(
@"-----------------------------------------------------------------------------------------------------------------------
  oooooooooo.      .                                  ooooooooooooo                          .o8                       
  `888'   `Y8b   .o8                                  8'   888   `8                         ""888                       
   888     888 .o888oo oooo d8b  .ooooo.  oooo    ooo      888      oooo d8b  .oooo.    .oooo888   .ooooo.  oooo d8b   
   888oooo888'   888   `888""""8P d88' `88b  `88b..8P'       888      `888""""8P `P  )88b  d88' `888  d88' `88b `888""""8P   
   888    `88b   888    888     888ooo888    Y888'         888       888      .oP""888  888   888  888ooo888  888       
   888    .88P   888 .  888     888    .o  .o8""'88b        888       888     d8(  888  888   888  888    .o  888       
  o888bood8P'    ""888"" d888b    `Y8bod8P' o88'   888o     o888o     d888b    `Y888""""8o `Y8bod88P"" `Y8bod8P' d888b      
          .          ,-_/         .                 .                                                                  
          |-. . .    '  | . . ,-. | , ,-. ,-,-. ,-. |- . ,-.                                                           
          | | | |       | | | | | |<  | | | | | ,-| |  | |                                                             
          ^-' `-|       | `-^ ' ' ' ` `-' ' ' ' `-^ `' ' `-'                                                           
               /|    /` |                                                                                              
              `-'    `--'                                                                                              
-----------------------------------------------------------------------------------------------------------------------
");

        }



    }

   


}
