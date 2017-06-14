using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            //This initializes the title of the software
            Console.Title = "Lending Club Software v1.0.0 AutoInvestor";

            //this changes the color of the text.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.ReadKey();
        }
    }
}
