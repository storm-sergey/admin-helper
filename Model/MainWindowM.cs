using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdminHelper.lib;
using static AdminHelper.Globals;
using AdminHelper.View;
using System.Windows.Controls;

namespace AdminHelper.Model
{
    public class MainWindowM
    {
        private readonly String ARMSFix;
        private readonly string BDriveScript;
        private readonly string ClearTempsScript;
        private readonly string SubnetsDealerships;
        public readonly string D_Program_Files;
        public readonly string ChromeCachePath;
        public readonly string AppDataLocalTempPath;
        public ObservableCollection<string> Dealerships;
        public ObservableCollection<string> Printers;
        public double GridOpacity;
        public bool GridIsEnabled;
        public string TicketClaim;
        public int UserDealership;
        public string SelectedPrinter;
        public string PrinterLink;
        

        public MainWindowM()
        {
            GridIsEnabled = true;
            GridOpacity = 1;
            BDriveScript = Properties.Resources.B_Drive_bat;
            ARMSFix = Properties.Resources.ARMS_Fix_reg;
            ClearTempsScript = Properties.Resources.Clear_temps_cmd;
            D_Program_Files = @"D:\Program Files";
            ChromeCachePath = @"D:\Users\SAStorm\AppData\Local\Google\Chrome\User Data\Default\Cache";
            AppDataLocalTempPath = @"D:\Users\SAStorm\AppData\Local\Temp";
            // TODO: test without internter connection
            // Dealership
            SubnetsDealerships = Properties.Resources.Subnets_19_03_2022;
            Dealerships = GetDealerships();
            UserDealership = GetDillershipIndex();
            // Printers
            Printers = new ObservableCollection<string>();
            SelectedPrinter = "";
            Task.Run(() => GetPrinterNamesDescriptions(Printers));
        }

        public async Task<string> FixARMS()
        {
            try
            {
                await Task.Run(() => Regedit.RunRegedit(ARMSFix));
                return "Перезапустите Internet Explorer";
            }
            catch
            {
                throw new Exception("Fixing ARMS is failed");
            }
        }

        public async Task<string> BDrive()
        {
            try
            {
                await Task.Run(() => CMD.Process(BDriveScript));
                return "Диск B подключен";
            }
            catch
            {
                throw new Exception("Disk B connection is failed");
            }
        }

        public async Task<string> RemoveChromeCache()
        {
            try
            {
                await Task.Run(() => Files.DeleteFilesInDirectory(ChromeCachePath));
                return "Кэш Google Chrome очищен";
            }
            catch
            {
                throw new Exception("Chrome cache deleting is failed");
            }
        }

        public async Task<string> ClearTemps()
        {
            try
            {
                await Task.Run(() => _ClearTemps());
                return "Временные файлы Windows удалены\n" +
                       "Рекомендуется перезагрузить компьютер";
            }
            catch
            {
                throw new Exception("Temp cleaning is failed");
            }
        }

        private void _ClearTemps()
        {
            CMD.Process(ClearTempsScript);
        }

        public async Task<string> UniplanSquaresFix()
        {
            try
            {
                await Task.Run(() => _UniplanSquaresFix());
                return "Необходимо обновить страницу UNIPLAN";
            }
            catch
            {
                throw new Exception("UNIPLAN fixing is failed");
            }
        }

        private void _UniplanSquaresFix()
        {
            string path = @"Software\Microsoft\Internet Explorer\BrowserEmulation";
            string key = "IntranetCompatibilityMode";
            RegistryKey myKey = Registry.CurrentUser.OpenSubKey(path, true);
            try
            {
                if (myKey != null)
                {
                    myKey.SetValue(key, 0, RegistryValueKind.DWord);
                    myKey.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                myKey.Close();
            }
        }

        // TODO: Ticket reason would be here
        public async Task<string> MakeATicket()
        {
            string mockTicketReason = "Заявка пользователя";
            await Task.Run(() => _MakeATicket(mockTicketReason, TicketClaim));
            return "Заявка отправлена";
        }

        private void _MakeATicket(string ticketReason = "", string ticketClaim = "")
        {
            try
            {
                string subject = $"ДЦ#{UserDealership}: {ticketReason}";
                string htmlBody =
                    $"Login: {UserCredentials.UserName}<br>" +
                    $"Computer: {UserCredentials.MachineName}<br>" +
                    $"IP: {Subnet.GetLocalhostIP()}<br>" +
                    (ticketReason != "" ? $"<br><div><b>{ticketReason}</b></div>" : "") +
                    (ticketClaim != "" ? $"<div>{ticketClaim}</div>" : "");
                Outlook.SendEmail(subject, htmlBody, SERVICE_DESK_EMAIL);
            }
            catch
            {
                throw new Exception("Ticket making is failed");
            }
        }

        // Subnet string format: "10.10.192.0/18 Location\n"
        public ObservableCollection<string> GetDealerships()
        {
            try
            {
                ObservableCollection<string> dealerships = new ObservableCollection<string>();
                HashSet<string> singlesDealerships = new HashSet<string>();
                foreach (string subnetDC in SubnetsDealerships.Split('\n'))
                {
                    int dealershipTitleStart = subnetDC.IndexOf(" ") + 1;
                    if (subnetDC[dealershipTitleStart] != '*')
                    {
                        string DC = subnetDC.Substring(dealershipTitleStart, subnetDC.Length - 1 - dealershipTitleStart).ToUpperInvariant();

                        singlesDealerships.Add(DC);
                    }
                }
                foreach (string dealership in singlesDealerships)
                {
                    dealerships.Add(dealership);
                }
                return dealerships;
            }
            catch
            {
                throw new Exception("Getting dealerships is falied");
            }

        }

        public int GetDillershipIndex()
        {

            foreach (string subnetDC in SubnetsDealerships.Split('\n'))
            {
                int subnetCidrLength = subnetDC.IndexOf(" ");
                string subnetCidr = subnetDC.Substring(0, subnetCidrLength);
                if (Subnet.IsRankedBySubnet(Subnet.GetLocalhostIP(), subnetCidr))
                {
                    // from CIDR to \n
                    string title = subnetDC.Substring(subnetCidrLength + 1, subnetDC.Length - subnetCidrLength - 2);
                    int index = Dealerships.IndexOf(title.ToUpper());
                    return index;
                }
            }
            return -1;
        }

        public async Task GetPrinterNamesDescriptions(ObservableCollection<string> printers)
        {
            try
            {
                string prefix = UserCredentials.LocationPrefix;
                await lib.Printers.FastGetListOfPrinters(prefix, printers);

                // TODO: Fix memory leak with GetListOfPrinters
                //string[] keys = {
                //    "Name",
                //    "Location",
                //    "Comment",
                //};
                //await Printer.GetListOfPrinters(prefix, keys, printers);
            }
            catch
            {
                throw new Exception("Getting printers is falied");
            }
        }

        public async Task<string> ConnectPrinter()
        {
            return await Task.Run(() => _ConnectPrinter());
        }

        private string _ConnectPrinter()
        {
            if (SelectedPrinter != "")
            {
                string[] printerProperies = SelectedPrinter.Split(' ');
                string printerName = printerProperies[0];
                if (!lib.Printers.IsPrinterInstalled(printerName))
                {
                    lib.Printers.ConnectPrinter(SelectedPrinter);
                }
                return "Принтер подключен";
            }
            return "";
        }

        public async Task<string> InstallPuntoSwitcher()
        {
            return await Task.Run(() => _InstallPuntoSwitcher());
        }

        private string _InstallPuntoSwitcher()
        {
            try
            {
                string source = @"\\1\IT_Distrib\Punto Switcher";
                string shortcut = "Punto Switcher.lnk";
                if (!Files.CheckDirectoryExists(source))
                {
                    throw new Exception("Punto Switcher недоступен для скачивания, "
                        + "обратитесь к системным администраторам");
                }
                string destination = $@"{D_Program_Files}\Punto Switcher";
                Files.CopyDirectory(source, destination);
                
                if (Files.CheckDirectoryExists($@"{destination}"))
                {
                    bool overwriteFile = true;
                    Files.CopyFile(shortcut, destination, UserCredentials.LocalDesktop, overwriteFile);
                    Files.RunFile(destination, shortcut);
                    return "Punto Switcher установлен";
                }
                else
                {
                    // TODO: exception remote report
                    new Exception("Не удалось установить Punto Switcher на Ваш компьютер. Обратитесь к системным администраторам.");
                    return "Ошибка установки PuntoSwitcher";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetComputerNameAndIp()
        {
            if (Subnet.IsNetworkAvailable())
            {
                return "\n\n" +
                       $"имя:  {Subnet.GetLocalhostName()}\n".ToLower() +
                       "                                            \n" +
                       $"ip:  {Subnet.GetLocalhostIP()}\n";
            }
            else
            {
                return "Отсутствует интернет подключение";
            }
        }
    }
}
