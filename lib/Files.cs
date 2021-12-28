using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace AdminHelper.lib
{
    public static class Files
    {
        public static void CopyUserDirectory(string path, string user, string source, string destination)
        {
            try
            {
                FileSystem.CopyDirectory(
                $@"\\{source}\d$\Users\{user}\{path}",
                $@"\\{destination}\d$\Users\{user}\{path}",
                UIOption.AllDialogs,
                UICancelOption.ThrowException);
            }
            catch
            {
                throw new Exception("Copying user directory is failed");
            }
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
    }
}