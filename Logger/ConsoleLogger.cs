using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public  class ConsoleLogger : Logger
    {
        public override void Log(LogLevel logLevel, string title, string[] messages)
        {
            DateTime timestamp = DateTime.Now;

            switch (logLevel)
            {
                case LogLevel.Debug:
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine($"[{timestamp}] [{logLevel}] [{title}]\n");
            Console.ForegroundColor = ConsoleColor.White;
            foreach(string message in messages) 
            { 
                Console.WriteLine(message);
            }
            Console.WriteLine();
        }
    }
}
