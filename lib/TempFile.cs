using System;
using System.Diagnostics;
using System.IO;

namespace AdminHelper.lib
{
    public static class TempFile
    {
        private static string MakeTempFile(string format)
        {
            string tempFile = $"{Path.GetTempPath()}{Path.GetRandomFileName()}.{format}";
            File.Create(tempFile).Close();
            return tempFile;
        }

        private static void FillTempFile(string tempFile, String script)
        {
            if (File.Exists(tempFile))
            {
                using (StreamWriter outputFile = new StreamWriter(tempFile))
                {
                    foreach (string line in script.Split('\n'))
                    {
                        outputFile.Write(line);
                    }
                }
            }
            else
            {
                throw new Exception("FillTempFile() is failed because tempFile isn't existence");
            }
        }

        private static void RemoveTempFile(string tempFile)
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }

        private static void RunTempFile(string tempFile, string processName = null)
        {
            Process tempFileProcess;
            if (String.IsNullOrEmpty(processName))
            {
                tempFileProcess = Process.Start($"{tempFile}");
            }
            else
            {
                tempFileProcess = Process.Start($"{processName}", $"/s {tempFile}");
            }       
            tempFileProcess.WaitForExit();
        }

        public static void MakeTempHook(String script, string format, string processName = null)
        {
            string tempFile = "";
            try
            {
                tempFile = MakeTempFile(format);
                FillTempFile(tempFile, script);
                RunTempFile(tempFile, processName);
                RemoveTempFile(tempFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                RemoveTempFile(tempFile);
            }
        }
    }
}
