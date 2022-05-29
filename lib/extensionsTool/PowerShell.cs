using System;

namespace AdminHelper.lib
{
    public static class PowerShell
    {
        public static void RunPowerShell(String script)
        {
            TempFiles.MakeTempHook(script, ".ps1", "powershell.exe");
        }
    }
}