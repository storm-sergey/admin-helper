using System;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Printing;
using static AdminHelper.Globals;
using System.Collections.ObjectModel;

namespace AdminHelper.lib
{
    public class Printer
    {
        public static void ConnectPrinter(string printerName)
        {
            try
            {
                printerName = printerName.Trim();
                if (IsPrinterInstalled(printerName))
                {
                    throw new Exception("Printer is already connected");
                }
                using (ManagementClass win32_Printer = new ManagementClass("Win32_Printer"))
                {
                    using (ManagementBaseObject inputParam = win32_Printer.GetMethodParameters("AddPrinterConnection"))
                    {
                        inputParam.SetPropertyValue("Name", $@"\\{PRINT_SERVER}\{printerName}");
                        ManagementBaseObject result = win32_Printer.InvokeMethod("AddPrinterConnection", inputParam, null);
                    }
                }
            }
            catch
            {
                throw new Exception("Printer connecting is failed");
            }
        }

        public static bool IsPrinterInstalled(string printerName)
        {
            foreach (string installedPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (installedPrinterName == $@"\\{PRINT_SERVER}\{printerName}")
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

        public async static Task FastGetListOfPrinters(string prefix, ObservableCollection<string> aPutPlace)
        {
            string request = $"SELECT Name FROM Win32_Share WHERE Name LIKE \"%{prefix}%\"";
            string[] keys = { "Name", };
            await PutWMIWin32Responce(PRINT_SERVER, request, keys, aPutPlace);
        }

        public async static Task GetListOfPrinters(string prefix, string[] keys, ObservableCollection<string> aPutPlace)
        {
            string request = $"SELECT * FROM Win32_Printer WHERE Name LIKE \"%{prefix}%\"";
            await PutWMIWin32Responce(PRINT_SERVER, request, keys, aPutPlace);
        }

        /// <summary>
        /// Make query
        /// WPF Only method
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
        /// WPF Only method
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
                        foreach (string key in keys)
                        {
                            objProperties += $"{obj[key]} ";
                        }
                        aPutPlace.Add(objProperties);
                    });
                }
            });
        }
    }
}