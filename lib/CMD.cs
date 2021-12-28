using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdminHelper.lib
{
    public static class CMD
    {
        private const bool REDIRECT_CMD_STANDARD_INPUT = true;
        private const bool REDIRECT_CMD_STANDARD_OUTPUT = true;
        private const bool REDIRECT_CMD_STANDARD_ERROR = true;

        // TODO: check required start info and cut unnecessary
        private static void SetProcessStartInfo(Process cmd)
        {
            cmd.StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
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

        private static List<string> CheckOutput(Process cmd)
        {
            List<string> output = null;
            if (REDIRECT_CMD_STANDARD_OUTPUT)
            {
                output = new List<string>();
                while (cmd.StandardOutput.Peek() > -1)
                {
                    output.Add(cmd.StandardOutput.ReadLine());
                }
                while (cmd.StandardError.Peek() > -1)
                {
                    output.Add(cmd.StandardError.ReadLine());
                }
            }
            return output;
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
                    output = CheckOutput(cmd).ToString();
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
