using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;

namespace USB_CTRL_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //var UD = new FTDI();
            ConsoleKeyInfo keyinfo;
            Console.WriteLine("Type something");
            keyinfo = Console.ReadKey();
            if (keyinfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Escape Pressed.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Key :" + keyinfo.Key.ToString());
                Console.ReadLine();
            }
        }
    }
}
