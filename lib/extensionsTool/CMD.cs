using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdminHelper.lib
{
    public static class CMD
    {
        private const bool REDIRECT_CMD_STANDARD_INPUT = true;
        private const bool REDIRECT_CMD_STANDARD_OUTPUT = false;
        private const bool REDIRECT_CMD_STANDARD_ERROR = false;

        // TODO: check required start info and cut unnecessary
        private static void SetProcessStartInfo(Process cmd)
        {
            cmd.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = REDIRECT_CMD_STANDARD_INPUT,
                RedirectStandardOutput = REDIRECT_CMD_STANDARD_OUTPUT,
                RedirectStandardError = REDIRECT_CMD_STANDARD_ERROR,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
        }

        private static List<string> Parser(String script)
        {
            return new List<string>(script.Split('\n'));
        }

        private static void PutScript(Process cmd, String script)
        {
            foreach (string line in Parser(script))
            {
                cmd.StandardInput.WriteLine(line);
            }
        }

        // TODO: How to process livecycle
        private static void Close(Process cmd)
        {
            try
            {
                try
                {
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                }
                catch
                {
                    cmd.Close();
                }
            }
            catch
            {
                cmd.Kill();
            }
        }

        public static string Process(String script)
        {
            try
            {
                string output = null;
                using (Process cmd = new Process())
                {
                    SetProcessStartInfo(cmd);
                    cmd.Start();
                    PutScript(cmd, script);
                    Close(cmd);
                }
                return output;
            }
            catch
            {
                throw new Exception("CMD process is failed");
            }
        }
    }
}
