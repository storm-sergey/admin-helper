using System.Security.Principal;

namespace AdminHelper.lib
{
    public static class UserCredentials
    {
        public static string UserName { get => WindowsIdentity.GetCurrent().Name.Split('\\')[1]; }
        public static string RuningAs { get => System.Environment.UserName; }
        public static string MachineName { get => System.Environment.MachineName; }
        public static string ComputerName { get => MachineName; }
        public static string LocationPrefix { get => MachineName.Split('-')[0]; }
        public static string AppDataLocal { get => $@"D:\Users\{UserName}\AppData\Local"; }
        public static string LocalDesktop
        { 
            get => Files.CheckDirectoryExistence($@"D:\Users\{UserName}\Desktop")
                ? $@"D:\Users\{UserName}\Desktop"
                : $@"D:\Users\{UserName}\Рабочий стол";
        }
        // TODO
        //NetworkCredential myCredentials = new NetworkCredential(String.Format("{0}\\{1}", DOMAIN, userName), password);
    }
}
