using System;
using System.Threading.Tasks;

namespace AdminHelper.lib
{
    public static class AppException
    {
        public static string Handle(Exception inner)
        {
            try
            {
                Log.Record($"inner:" +
                    $"\n{inner.Message}" +
                    $"\n{inner.StackTrace}"
                );
                return inner.Message;
            }
            catch (Exception ex)
            {
                return $"AppException error:" +
                    $"\ninner:" +
                    $"\n{inner.Message}" +
                    $"\n{inner.StackTrace}" +
                    $"\nex:" +
                    $"\n{ex.Message}" +
                    $"\n{ex.StackTrace}";
            }
        }

        public static string Handle(string message)
        {
            try
            {
                Task.Run(() => Log.Record($"message:\n{message}"));
                return message;
            }
            catch (Exception ex)
            {
                return $"AppException error:" +
                    $"\nmessage:" +
                    $"\n{message}" +
                    $"\nex:" +
                    $"\n{ex.Message}" +
                    $"\n{ex.StackTrace}";
            }
        }

        public static string Handle(string message, Exception inner)
        {
            try
            {
                Task.Run(() =>
                {
                    Log.Record($"message:" +
                        $"\n{message}" +
                        $"\ninner:" +
                        $"\n{inner.Message}" +
                        $"\n{inner.StackTrace}");
                });
                return $"{message}\n{inner.Message}";
            }
            catch (Exception ex)
            {
                return $"AppException error:" +
                    $"\nmessage:" +
                    $"\n{message}" +
                    $"\ninner:" +
                    $"\n{inner.Message}" +
                    $"\n{inner.StackTrace}" +
                    $"\nex:" +
                    $"\n{ex.Message}" +
                    $"\n{ex.StackTrace}";
            }
        }
    }
}
