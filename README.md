# FolderSync

## What it does
Syncronizes the content between two folders, source and replica. The program periodically checks for changes and updates the replica folder.
Logs the folder contents and performed file operations (copy and delete).

## Usage
Run the FolderSync executable via cmd. The executable takes 4 parameters: source dir, replica dir, log dir and sync interval. Usage as described below:

```
      USAGE : FOLDERSYNC source replica log interval
     source : source directory, from which files will be copied from
    replica : destination directory, where files will be copied to
        log : directory for logfile storage
   interval : sync interval in seconds
    EXAMPLE : FOlDERSYNC C:\source C:\replica C:\logs 60
```
