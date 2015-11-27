using System;
using System.Diagnostics;

namespace evtailNet
{
    class Program
    {

        static void Main(string[] args)
        {
            Program.StandardBackgroundColor = Console.BackgroundColor;
            Program.StandardForegroundColor = Console.ForegroundColor;

            var logName = args.Length > 1
                ? args[1]
                : "Application";

            var el = new EventLog(logName, Environment.MachineName);
            el.EntryWritten += (sender, eargs) =>
            {
                var entry = eargs.Entry;

                SetConsoleColors(entry.Source, entry.Message);
                Console.Write("{0} {1} {2}", entry.TimeGenerated, entry.Source, entry.Message);
                if (!entry.Message.EndsWith(Environment.NewLine))
                    Console.Write(Environment.NewLine);
            };
            el.EnableRaisingEvents = true;

            //Thread.Sleep(TimeSpan.FromHours(24));
            var quit = false;
            do
            {
                var key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case 'c':
                        Console.Clear();
                        break;
                    case 'q':
                        quit = true;
                        break;
                }
            } while (!quit);
        }

        public static ConsoleColor StandardBackgroundColor { get; set; }
        public static ConsoleColor StandardForegroundColor { get; set; }

        private static void SetConsoleColors(string source, string message)
        {
            if (message.Contains("ERROR"))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (message.Contains("exception") || message.Contains("Exception"))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.BackgroundColor = Program.StandardBackgroundColor;
            Console.ForegroundColor = Program.StandardForegroundColor;
        }
    }
}
