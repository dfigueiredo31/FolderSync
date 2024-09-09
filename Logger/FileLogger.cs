using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class FileLogger : Logger
    {
        public string LogPath { get; }
        public LogFileExt LogFileExt { get; }
        public string LogFileName { get; }

        public FileLogger(string logDir, LogFileExt fileExtension)
        {
            LogFileExt = fileExtension;
            LogFileName = $"log_{DateTime.Now.ToString("dd_MM_yyyy")}.{LogFileExt}";
            LogPath = Path.Combine(logDir, LogFileName);

            if (!Path.Exists(logDir))
            {
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                di.CreateSubdirectory("logs");
                LogPath = Path.Combine(di.FullName, "logs", LogFileName);
            }
        }

        public override void Log(LogLevel logLevel, string title, string[] messages)
        {
            DateTime timestamp = DateTime.Now;

            try
            {
                lock (lockOb) ;
                using (StreamWriter sw = new StreamWriter(LogPath, true))
                {
                    sw.WriteLine($"[{timestamp}] [{logLevel}] [{title}]\n");
                    foreach (string message in messages)
                    {
                        sw.WriteLine(message);
                    }
                    sw.WriteLine();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
