using System;
using System.Diagnostics;
using System.IO;

namespace AdminHelper.lib
{
    public static class Regedit
    {
        private static string MakeTempReg()
        {
            try
            {
                string tempReg = $"{Path.GetTempPath()}{Path.GetRandomFileName()}.reg";
                File.Create(tempReg).Close();
                return tempReg;
            }
            catch
            {
                throw new Exception("Making temp .reg is failed");
            }
            
        }

        private static void FillTempReg(string tempReg, String script)
        {
            if (File.Exists(tempReg))
            {
                using (StreamWriter outputFile = new StreamWriter(tempReg))
                {
                    foreach (string line in script.Split('\n'))
                    {
                        outputFile.WriteLine(line);
                    }
                }
            } else
            {
                throw new Exception("FillTempReg is failed");
            }
        }

        private static void RemoveReg(string tempReg)
        {
            if (File.Exists(tempReg))
            {
                File.Delete(tempReg);
            }
        }

        private static void RegEdit(string tempReg)
        {
            try
            {
                Process regeditProcess = Process.Start("regedit.exe", $"/s {tempReg}");
                regeditProcess.WaitForExit();
            }
            catch
            {
                throw new Exception("regedit calling is failed");
            }
        }

        public static void EditBy(String script)
        {
            string tempReg = MakeTempReg();
            FillTempReg(tempReg, script);
            RegEdit(tempReg);
            RemoveReg(tempReg);
        }
    }
}
