using FolderOperations;
using Logger;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace FolderSync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //prerun
            //log init + console log
            //validate args
            LoggerManager logger = new LoggerManager(new ConsoleLogger());

            //num of args
            if(args.Length != 4) 
            {
                logger.Error("Invalid args", "FOLDER SYNC\n"
                            , "      USAGE : FOLDERSYNC source replica log interval\n"
                            , "     source : source directory, from which files will be copied from"
                            , "    replica : destination directory, where files will be copied to"
                            , "        log : directory for logfile storage"
                            , "   interval : sync interval in seconds\n"
                            , "    EXAMPLE : FOlDERSYNC C:\\source C:\\replica C:\\logs 60");
                return;
            }

            //whether the specified folders exist
            string folderCheck = "";
            bool invalidFolders = false;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        folderCheck += "     source : ";
                        break;
                    case 1:
                        folderCheck += "        replica : ";
                        break;
                    case 2:
                        folderCheck += "            log : ";
                        break;
                }
                if (!Path.Exists(args[i]))
                {
                    invalidFolders = true;
                    folderCheck += "directory is invalid or doesnt exist\n";
                }
                else
                {
                    folderCheck += args[i] + "\n";
                }
            }

            if (invalidFolders)
            {
                logger.Error("One or more invalid folders", "      USAGE : FOLDERSYNC source replica log interval\n", folderCheck);
                return;
            }

            string sourceDirectory = args[0];
            string replicaDirectory = args[1];
            string logDirectory = args[2];

            //whether the given time value is valid
            if (!int.TryParse(args[3], out int parsedTime) || parsedTime <= 0)
            {
                logger.Error("Invalid sync interval", "      USAGE : FOLDERSYNC source replica log interval\n", $"   interval : {args[3]}");
                return;
            }

            int syncInterval = parsedTime;

            //program init
            //file log init
            //timer init
            FileLogger fileLog = new FileLogger(logDirectory, LogFileExt.txt);
            logger.AddLogger(fileLog);

            DirectoryInfo source = new DirectoryInfo(sourceDirectory);
            DirectoryInfo replica = new DirectoryInfo(replicaDirectory);

            Timer timer = new Timer(syncInterval * 1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();


            //run
            logger.Info("FolderSync runnig"
                       , $"  source : {sourceDirectory}"
                       , $" replica : {replicaDirectory}"
                       , $"     log : {fileLog.LogPath}"
                       , $"interval : {syncInterval} seconds");

            void timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                logger.Info("Source folder", FolderManager.GetFolderContents(source));
                logger.Info("Replica folder", FolderManager.GetFolderContents(replica));

                string executed = FolderManager.SyncFolders(source, replica);

                if(!string.IsNullOrEmpty(executed)) {

                    logger.Warning("Executed operations", executed);
                }
                else
                {
                    logger.Info("No operations", "No files copied or deleted since last execution");
                }
;
            }
            Console.ReadLine();
        }
    }
}
