using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using NConsoler;

namespace evtailNet
{
    class Program
    {
        private static StreamWriter _outputFile;

        public static ConsoleColor StandardForegroundColor { get; set; }

        public static ConsoleColor StandardBackgroundColor { get; set; }

        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
        }

        [Action]
        public static void DoWork(
            [Optional("config.json")] string configFile)
        {
            var configuration = JsonConfigurationParser.LoadConfiguration<EvTailConfigurationModel>(configFile);

            SaveConsoleColors();

            TailEventLog(configuration);

            StartCommandLoop();

            if (_outputFile != null)
            {
                _outputFile.Close();
                _outputFile.Dispose();
            }
        }

        private static void StartCommandLoop()
        {
            var quit = false;
            do
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    // clear
                    case ConsoleKey.C:
                        Console.Clear();
                        break;

                    // quit
                    case ConsoleKey.Q:
                        quit = true;
                        break;
                }
            } while (!quit);
        }

        private static void TailEventLog(EvTailConfigurationModel configuration)
        {
            if (!string.IsNullOrEmpty(configuration.outputFile))
            {
                _outputFile = new StreamWriter(configuration.outputFile, true);
            }

            var el = new EventLog(configuration.logName, Environment.MachineName);
            el.EntryWritten += (sender, eargs) =>
            {
                var entry = eargs.Entry;

                SetConsoleColors(entry.Source, entry.Message, configuration.errorStrings);
                Output(entry, configuration, _outputFile);
                ReSetConsoleColors();
            };
            el.EnableRaisingEvents = true;
        }

        private static void Output(EventLogEntry entry, EvTailConfigurationModel configuration, StreamWriter outputFile)
        {
            var msg = string.Format("{0} {1} {2}", entry.TimeGenerated, entry.Source, entry.Message);

            if (configuration.outputToConsole)
            {
                Console.Write(msg);
            }

            outputFile?.Write(msg);
            outputFile?.Flush();

            if (!entry.Message.EndsWith(Environment.NewLine))
            {
                Console.Write(Environment.NewLine);
                outputFile?.Write(Environment.NewLine);
            }
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

        private static void SetConsoleColors(string source, string message, string[] configuration)
        {
            if (IsRedCandidate(message, configuration))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static bool IsRedCandidate(string message, string[] errorStrings)
        {
            return errorStrings.Any(message.Contains);
        }
    }
}
