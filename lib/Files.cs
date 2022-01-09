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

        public static void CopyDirectory(string source, string destination)
        {
            FileSystem.CopyDirectory(
                $@"{source}",
                $@"{destination}",
                UIOption.AllDialogs,
                UICancelOption.ThrowException);
        }

        public static void CopyUserDirectory(string userDirectory, string user, string source, string destination)
        {
                FileSystem.CopyDirectory(
                $@"{source}\d$\Users\{user}\{userDirectory}",
                $@"{destination}\d$\Users\{user}\{userDirectory}",
                UIOption.AllDialogs,
                UICancelOption.ThrowException);
        }

        public static void CopyFile(string fileName, string source, string destination)
        {
            FileSystem.CopyFile(
                CheckBackslashBetween(source, fileName),
                CheckBackslashBetween(destination, fileName),
                UIOption.OnlyErrorDialogs,
                UICancelOption.ThrowException);
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

        public static bool CheckDirectoryExists(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            return dir.Exists;
        }

        public static bool CheckUserDirectoryExists(string userDirectory, string user, string computerName)
        {
            return CheckDirectoryExists($@"\\{computerName}\d$\Users\{user}\{userDirectory}");
        }

        public static bool CheckUserExists(string computerName, string user)
        {
            return CheckDirectoryExists($@"\\{computerName}\d$\Users\{user}");
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