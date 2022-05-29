using System;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Printing;
using static AdminHelper.Globals;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace AdminHelper.lib
{
    public class Printers
    {
        #region Printer managing
        // TODO
        /// <summary>
        /// MOCK
        /// </summary>
        /// <param name="printerName"></param>
        public static void ReconnectDirectly(string printerName)
        {
            return;
        }

        public static PrinterSettings.StringCollection GetLocalInstalledPrinterNames()
        {
            return PrinterSettings.InstalledPrinters;
        }

        public static void DeleteAllPrinters()
        {

            foreach(string printer in GetLocalInstalledPrinterNames())
            {
                try
                {
                    DeletePrinter(printer);
                }
                catch
                {
                    throw new Exception($"Printer {printer} isn't deleted");
                }
            }
        }

        public static bool DeletePrinter(string printerName)
        {
            ManagementScope oManagementScope = new ManagementScope(ManagementPath.DefaultPath);
            oManagementScope.Connect();

            SelectQuery oSelectQuery = new SelectQuery
            {
                QueryString = $@"SELECT * FROM Win32_Printer WHERE Name = '{printerName.Replace("\\", "\\\\")}'"
            };

            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(oManagementScope, oSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            if (oObjectCollection.Count != 0)
            {
                foreach (ManagementObject oItem in oObjectCollection)
                {
                    oItem.Delete();
                    return true;
                }
            }
            return false;
        }

        public static void ConnectPrinter(string printerName, string printServer = "")
        {
            try
            {
                printerName = printerName.Trim();
                if (IsPrinterInstalledAndValid(printerName, printServer))
                {
                    return;
                    //throw new Exception("Printer is already connected");
                }
                using (ManagementClass win32_Printer = new ManagementClass("Win32_Printer"))
                {
                    using (ManagementBaseObject inputParam = win32_Printer.GetMethodParameters("AddPrinterConnection"))
                    {
                        string printer = String.IsNullOrEmpty(printServer) ? printerName : $@"\\{printServer}\{printerName}";

                        inputParam.SetPropertyValue("Name", printer);
                        ManagementBaseObject result = win32_Printer.InvokeMethod("AddPrinterConnection", inputParam, null);
                    }
                }
            }
            catch
            {
                throw new Exception("Printer connecting is failed");
            }
        }

        public static bool IsPrinterInstalledAndValid(string printerName, string printServer = "")
        {
            foreach (string installedPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (installedPrinterName == printerName
                ||  installedPrinterName == $@"\\{printServer}\{printerName}")
                {
                    PrinterSettings printer = new PrinterSettings
                    {
                        PrinterName = installedPrinterName
                    };
                    if (printer.IsValid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Default printer
        public static string GetDefaultPrinterName()
        {
            foreach (string installedPrinterName in PrinterSettings.InstalledPrinters)
            {
                PrinterSettings printer = new PrinterSettings
                {
                    PrinterName = installedPrinterName
                };
                if (printer.IsDefaultPrinter)
                {
                    return installedPrinterName;
                }
            }
            return "";
        }

        public static void SetDefaultPrinter(string printerName, string printServer = null)
        {
            string printer = printServer != null ? $@"\\{printServer}\{printerName}" : printerName;
            if (IsPrinterInstalledAndValid(printer))
            {
                bool result;
                try
                {
                    result = _SetDefaultPrinter(printer);
                }
                catch
                {
                    result = _SetDefaultPrinter2(printer);
                }
                bool check = GetDefaultPrinterName() == printer;
                if (!result
                ||  !check)
                {
                    throw new Exception($"Принтер {printer} не установлен по умлочанию");
                }
            }
        }

        private static bool _SetDefaultPrinter(string printerName)
        {
            ManagementScope oManagementScope = new ManagementScope(ManagementPath.DefaultPath);
            oManagementScope.Connect();

            SelectQuery oSelectQuery = new SelectQuery
            {
                QueryString = $@"SELECT * FROM Win32_Printer WHERE Name = '{printerName.Replace("\\", "\\\\")}'"
            };

            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(oManagementScope, oSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            if (oObjectCollection.Count != 0)
            {
                foreach (ManagementObject oItem in oObjectCollection)
                {
                    oItem.InvokeMethod("SetDefaultPrinter", new object[] { printerName });
                    return true;
                }
            }
            return false;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool _SetDefaultPrinter2(string Name);
        #endregion

        #region Printer list from print server
        public async static Task FastGetListOfPrinters(string printServer, string prefix, ObservableCollection<string> aPutPlace)
        {
            string request = $"SELECT Name FROM Win32_Share WHERE Name LIKE \"%{prefix}%\"";
            string[] keys = { "Name", };
            await PutWMIWin32Responce(printServer, request, keys, aPutPlace);
        }

        public async static Task GetListOfPrinters(string printServer, string prefix, ObservableCollection<string> aPutPlace, string[] keys = null)
        {
            string request = $"SELECT * FROM Win32_Printer WHERE Name LIKE \"%{prefix}%\"";
            await PutWMIWin32Responce(printServer, request, keys, aPutPlace);
        }

       
        /// <summary>
        /// Make query
        /// </summary>
        /// <param name="hostname"> "dp-print-02" for example </param>
        /// <param name="request"> WMI request </param>
        /// <param name="keys"> for example: {"Name", "Location", "Comment", "etc."} </param>
        /// <param name="aPutPlace"> ObservableCollection of strings"
        private async static Task PutWMIWin32Responce(
            string hostname,
            string request,
            string[] keys,
            ObservableCollection<string> aPutPlace)
        {
            try
            {
                ManagementScope scope = new ManagementScope($@"\\{hostname}\root\CIMV2");
                SelectQuery query = new SelectQuery(request);
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    await PutInUIThread(keys, searcher, aPutPlace);
                };
                
            }
            catch
            {
                throw new Exception("WMI request is failed");
            }
        }

        /// <summary>
        /// Getting by WMI of Object properties
        /// </summary>
        private async static Task PutInUIThread(
            string[] keys,
            ManagementObjectSearcher searcher,
            ObservableCollection<string> aPutPlace)
        {
            await Task.Run(() =>
            {
                ManagementObjectCollection objCollection = searcher.Get();
                
                string objProperties;
                foreach (ManagementObject obj in objCollection)
                {
                    objProperties = "";
                    
                    // put the delegate on UI Dispatcher
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        //string objProperties = "";
                        if (keys != null)
                        {
                            foreach (string key in keys)
                            {
                                objProperties += $"{obj[key]} ";
                            }
                            aPutPlace.Add(objProperties);
                        }
                    });
                }
            });
        }
    }
    #endregion
}