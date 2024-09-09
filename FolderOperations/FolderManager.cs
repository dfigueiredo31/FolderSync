using Logger;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FolderOperations
{
    public static class FolderManager
    {
        /// <summary>
        /// Syncs files and folders between the specified folders - source and replica
        /// Any file or folder in source will be copied to the replica folder, with the same subdirectory structure
        /// Any file modified externally in the replica folder will be replaced by the original file from the source folder
        /// Any file deleted from the source folder will also be deleted from the replica folder
        /// </summary>
        /// <param name="source">The source folder, from which the files will be copied from</param>
        /// <param name="replica">The replica folder, to which the files will be copied to</param>
        public static string SyncFolders(DirectoryInfo source, DirectoryInfo replica)
        {
            StringBuilder operations = new StringBuilder();
            bool filesChanged = false;
            operations.AppendFormat("{0,-60}{1,-50}{2,-10}\n", "Name", "Path", "Operation");

            SyncFoldersAux(source, replica, operations, ref filesChanged);

            return filesChanged ? operations.ToString() : "";
        }

        /// <summary>
        /// Helper method for SyncFolders
        /// Known bugs: does not handle deleting subdirectories.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="replica"></param>
        /// <param name="operations"></param>
        /// <param name="filesChanged"></param>
        private static void SyncFoldersAux(DirectoryInfo source, DirectoryInfo replica, StringBuilder operations, ref bool filesChanged)
        {
            if (Directory.Exists(replica.FullName) == false)
            {
                Directory.CreateDirectory(replica.FullName);
            }

            source.Refresh();
            replica.Refresh();

            Dictionary<string, FileInfo> sourceFiles = source.GetFiles().ToDictionary(x => x.Name);
            Dictionary<string, FileInfo> replicaFiles = replica.GetFiles().ToDictionary(x => x.Name);

            //copy files from source to replica
            foreach (KeyValuePair<string, FileInfo> sf in sourceFiles)
            {
                if (!replicaFiles.TryGetValue(sf.Key, out FileInfo rf) || !FilesAreEqual(sf.Value, rf))
                {
                    filesChanged = true;
                    sf.Value.CopyTo(Path.Combine(replica.FullName, sf.Value.Name), true);
                    operations.AppendFormat("{0,-60}{1,-50}{2,-10}\n", sf.Value.Name, replica.FullName, "Copy");
                }
            }

            //remove any files that are in replica but not in source
            foreach (KeyValuePair<string, FileInfo> rf in replicaFiles)
            {
                if (!sourceFiles.TryGetValue(rf.Key, out FileInfo sf))
                {
                    filesChanged = true;
                    rf.Value.Delete();
                    operations.AppendFormat("{0,-60}{1,-50}{2,-10}\n", rf.Value.Name, replica.FullName, "DELETE");
                }
            }

            //handle subdirectories
            foreach (DirectoryInfo subDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = replica.CreateSubdirectory(subDir.Name);
                SyncFoldersAux(subDir, nextTargetSubDir, operations, ref filesChanged);
            }
        }

        /// <summary>
        /// Returns a string representation of the folder contents
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetFolderContents(DirectoryInfo folder)
        {
            folder.Refresh();

            StringBuilder contents = new StringBuilder();
            contents.AppendFormat("{0,-60}{1,-25}{2,-25}{3,-10}\n", "Name", "Created", "Last modified", "Size (kb)"); 

            AddDirectoryContents(folder, contents, 0);
            return contents.ToString();
        }

        /// <summary>
        /// Helper method for GetFolderContents 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="contents"></param>
        private static void AddDirectoryContents(DirectoryInfo directory, StringBuilder contents, int indent)
        {
            string folderIndent = new string('.', indent);
            string folderFormat = "{0}" + "{1,-" + (60 - indent) + "}{2,-25}{3,-25}{4,-10}\n";
            contents.AppendFormat(folderFormat, folderIndent, "\\"+directory.Name+"\\", directory.CreationTime, directory.LastWriteTime, "");

            string fileIndent = new string('.', indent + 1);
            string fileFormat = "{0}" + "{1,-" + (59-indent) + "}{2,-25}{3,-25}{4,-10}\n";
            foreach (var file in directory.GetFiles())
            {
                contents.AppendFormat(fileFormat, fileIndent, file.Name, file.CreationTime, file.LastWriteTime, file.Length/1000);
            }

            // handle subdirectories
            foreach (var subDir in directory.GetDirectories())
            {
                AddDirectoryContents(subDir, contents, indent + 1);
            }
        }

        /// <summary>
        /// Checks whether two files are equivalent or not, based on name, last modified date, size, and computed hash
        /// </summary>
        /// <param name="firstFile"></param>
        /// <param name="secondFile"></param>
        /// <returns>Bool value representing whether the two given files are equivalent or not</returns>
        public static bool FilesAreEqual(FileInfo firstFile, FileInfo secondFile)
        {
            if (firstFile.Name != secondFile.Name)
            {
                return false;
            }

            if (firstFile.LastWriteTime != secondFile.LastWriteTime)
            {
                return false;
            }

            if (firstFile.Length != secondFile.Length) 
            {
                return false;
            }

            byte[] firstFileaHash;
            byte[] secondFileHash;

            using (FileStream fs = firstFile.OpenRead())
            {
                firstFileaHash = MD5.Create().ComputeHash(fs);
            }

            using (FileStream fs = secondFile.OpenRead())
            {
                secondFileHash = MD5.Create().ComputeHash(fs);
            }

            for (int i = 0; i < firstFileaHash.Length; i++)
            {
                if (firstFileaHash[i] != secondFileHash[i])
                    return false;
            }

            return true;
        }
    }
}
