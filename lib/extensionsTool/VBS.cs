using System;

namespace AdminHelper.lib
{
    public static class VBS
    {
        public static void RunVBS(String script)
        {
            TempFiles.MakeTempHook(script, ".vbs");
        }
    }
}