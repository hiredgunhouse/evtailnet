namespace evtailNet
{
    internal class EvTailConfigurationModel
    {
        // TODO make this an array to monitor multiple logs (but this will require syncronisation)
        public string logName;
        public string[] errorStrings;
        public string outputFile;
        public bool outputToConsole;
    }
}