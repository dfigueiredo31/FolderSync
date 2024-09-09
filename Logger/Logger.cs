namespace Logger
{
    public abstract class Logger
    {
        protected readonly object lockOb = new object();

        /// <summary>
        /// Abstract log method.
        /// Each log class implements it's own logging behaviour
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public abstract void Log(LogLevel logLevel, string title, string[] message);

        /// <summary>
        /// Create a Debug level log
        /// </summary>
        /// <param name="title"></param>
        /// <param name="messages"></param>
        public void Debug(string title, params string[] messages)
        {
            Log(LogLevel.Debug, title, messages);
        }

        /// <summary>
        /// Create an Info level log
        /// </summary>
        /// <param name="title"></param>
        /// <param name="messages"></param>
        public void Info(string title, params string[] messages)
        {
            Log(LogLevel.Info, title, messages);
        }

        /// <summary>
        /// Create a Warning level log
        /// </summary>
        /// <param name="title"></param>
        /// <param name="messages"></param>
        public void Warning(string title, params string[] messages) 
        { 
            Log(LogLevel.Warning, title, messages);
        }

        /// <summary>
        /// Create an Error level log
        /// </summary>
        /// <param name="title"></param>
        /// <param name="messages"></param>
        public void Error(string title, params string[] messages)
        {
            Log(LogLevel.Error, title, messages);
        }
    }
}
