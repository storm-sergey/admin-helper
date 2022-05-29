using System;

namespace AdminHelper.lib
{
    public static class Regedit
    {
        public static void RunRegedit(String script)
        {
            TempFiles.MakeTempHook(script, ".reg", "regedit.exe");
        }
    }
}