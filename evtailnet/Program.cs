using System;
using System.Diagnostics;
using System.Linq;

namespace evtailNet
{
    class Program
    {
        public static ConsoleColor StandardForegroundColor { get; set; }

        public static ConsoleColor StandardBackgroundColor { get; set; }

        static void Main(string[] args)
        {
            SaveConsoleColors();

            var logName = args.Length > 1
                ? args[1]
                : "Application";

            TailEventLog(logName);

            StartCommandLoop();
        }

        private static void StartCommandLoop()
        {
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

        private static void TailEventLog(string logName)
        {
            var el = new EventLog(logName, Environment.MachineName);
            el.EntryWritten += (sender, eargs) =>
            {
                var entry = eargs.Entry;

                SetConsoleColors(entry.Source, entry.Message);
                Console.Write("{0} {1} {2}", entry.TimeGenerated, entry.Source, entry.Message);
                if (!entry.Message.EndsWith(Environment.NewLine))
                    Console.Write(Environment.NewLine);
                ReSetConsoleColors();
            };
            el.EnableRaisingEvents = true;
        }

        private static void ReSetConsoleColors()
        {
            Console.BackgroundColor = Program.StandardBackgroundColor;
            Console.ForegroundColor = Program.StandardForegroundColor;
        }

        private static void SaveConsoleColors()
        {
            Program.StandardBackgroundColor = Console.BackgroundColor;
            Program.StandardForegroundColor = Console.ForegroundColor;
        }

        private static void SetConsoleColors(string source, string message)
        {
            if (IsRedCandidate(message))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static bool IsRedCandidate(string message)
        {
            // TODO move this to config file
            var patters = new[]
            {
                "[Error]",
                "[Fatal]",
                "ERROR",
                "exception",
                "Exception"
            };

            return patters.Any(message.Contains);
        }
    }
}
