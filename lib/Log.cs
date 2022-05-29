using System;
using System.IO;
using static AdminHelper.Globals;

namespace AdminHelper.lib
{
    public static class Log
    {
        public static void Record(string log)
        {
            Send(log);
            AddLocal(log);
        }

        public static void Record(Exception e)
        {
            Record(e.Message);
        }

        // TODO
        private static void Send(string log)
        {
            throw new NotImplementedException();
            if (LOG_ENDPOINT != null)
            {
                // TODO: Json.MakeLog()
                // Http.Request("POST", LOG_ENDPOINT, Json.MakeLog(log));
            }
        }

        private static void AddLocal(string log)
        {
            if (IsNotLogFileExist())
            {
                CreateLogFile();
            }

            try
            {
                File.AppendAllText(LOG_FILE, $"\n{DateTime.Now:dd-MM-yyyy HH:mm:ss}\n{log}");
            }
            catch (Exception ex)
            {
                new View.NonModalMessage($"Ошибка записи лога" +
                    $"\nИнформация для поддержки:" +
                    $"\n{ex.Message}" +
                    $"\n{ex.StackTrace}").Show();
            }
        }

        private static bool IsLogFileExist()
        {
            return File.Exists(LOG_FILE);
        }

        private static bool IsNotLogFileExist()
        {
            return !IsLogFileExist();
        }

        private static void CreateLogFile()
        {
            if (!Files.CheckDirectoryExistence(APP_DATA))
            {
                Directory.CreateDirectory(APP_DATA);
            }
            File.Create(LOG_FILE);
        }

        // TODO
        private static void SendLocalLog()
        {
            throw new NotImplementedException();
        }
    }
}
