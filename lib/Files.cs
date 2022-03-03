using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace AdminHelper.lib
{
    public static class Files
    {
        private static string CheckBackslashBetween(string path, string name)
        {
            if (path.EndsWith("\\"))
            {
                return path + name;
            }
            else
            {
                return $@"{path}\{name}";
            }
        }

        public static void CopyDirectory(
            string source,
            string destination,
            bool dialogs = true,
            bool errors = true)
        {
            FileSystem.CopyDirectory(
                $@"{source}",
                $@"{destination}",
                dialogs ? UIOption.AllDialogs : UIOption.OnlyErrorDialogs,
                errors ? UICancelOption.ThrowException : UICancelOption.DoNothing);
        }

        public static void CopyUserDirectory(
            string userDirectory, 
            string user, 
            string source, 
            string destination,
            bool dialogs = true,
            bool errors = true)
        {
            // TODO: exception
                FileSystem.CopyDirectory(
                $@"\\{source}\d$\Users\{user}\{userDirectory}",
                $@"\\{destination}\d$\Users\{user}\{userDirectory}",
                dialogs ? UIOption.AllDialogs : UIOption.OnlyErrorDialogs,
                errors ? UICancelOption.ThrowException : UICancelOption.DoNothing);
        }

        public static void CopyFile(
            string fileName,
            string source,
            string destination,
            bool overwrite)
        {
            FileSystem.CopyFile(
                CheckBackslashBetween(source, fileName),
                CheckBackslashBetween(destination, fileName),
                overwrite);
        }

        public static void CopyFile(
            string fileName,
            string source,
            string destination,
            bool dialogs = true,
            bool errors = true)
        {
            FileSystem.CopyFile(
                CheckBackslashBetween(source, fileName),
                CheckBackslashBetween(destination, fileName),
                dialogs ? UIOption.AllDialogs : UIOption.OnlyErrorDialogs,
                errors ? UICancelOption.ThrowException : UICancelOption.DoNothing);
        }

        public static void DeleteFilesInDirectory(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                        continue;
                    }

                }
                DirectoryInfo[] subDirectories = di.GetDirectories();
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    try
                    {
                        subDirectory.Delete(true);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {
                throw new Exception("File deletign is failed");
            }
        }

        public static bool CheckDirectoryExistence(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            return dir.Exists;
        }

        public static bool CheckUserDirectoryExistence(string userDirectory, string user, string computerName)
        {
            return CheckDirectoryExistence($@"\\{computerName}\d$\Users\{user}\{userDirectory}");
        }

        public static bool CheckUserExistence(string computerName, string user)
        {
            return CheckDirectoryExistence($@"\\{computerName}\d$\Users\{user}");
        }

        public static void RunFile(string file)
        {
            System.Diagnostics.Process.Start(file);
        }

        public static void RunFile(string path, string fileName)
        {
            RunFile(CheckBackslashBetween(path, fileName));
        }
    }
}