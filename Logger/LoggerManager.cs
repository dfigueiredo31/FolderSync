namespace Logger
{
    public class LoggerManager : Logger
    {
        public List<Logger> Loggers { get; }

        public LoggerManager(params Logger[] loggers) 
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Loggers = loggers.ToList();
        }

        public void AddLogger(Logger logger)
        {
            Loggers.Add(logger);
        }

        public override void Log(LogLevel logLevel, string title, string[] messages)
        {
            foreach(Logger logger in Loggers)
            {
                    logger.Log(logLevel, title, messages);
            }
        }

    }
}
