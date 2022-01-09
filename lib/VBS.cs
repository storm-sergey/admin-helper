using System;

namespace AdminHelper.lib
{
    public static class VBS
    {
        public static void RunVBS(String script)
        {
            TempFile.MakeTempHook(script, ".vbs");
        }
    }
}