using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdminHelper.lib;
using static AdminHelper.Globals;


namespace AdminHelper.Model
{
    public class MainWindowM
    {
        private readonly String ARMSFix;
        private readonly string BDriveScript;
        private readonly string SubnetsDealerships;
        public readonly string D_Program_Files;
        public readonly string ChromeCachePath;
        public readonly string AppDataLocalTempPath;
        public ObservableCollection<string> Dealerships;
        public ObservableCollection<string> Printers;
        public string TicketClaim;
        public string UserDealership;
        public string SelectedPrinter;
        public string PrinterLink;

        public MainWindowM()
        {
            BDriveScript = Properties.Resources.B_Drive_bat;
            ARMSFix = Properties.Resources.ARMS_Fix_reg;
            D_Program_Files = @"D:\Program Files";
            ChromeCachePath = @"D:\Users\SAStorm\AppData\Local\Google\Chrome\User Data\Default\Cache";
            AppDataLocalTempPath = @"D:\Users\SAStorm\AppData\Local\Temp";
            // TODO: test without internter connection
            // Dealership
            SubnetsDealerships = Properties.Resources.Subnets_12_12_2021;
            Dealerships = GetDealerships();
            UserDealership = GetDillershipTitle();
            // Printers
            Printers = new ObservableCollection<string>();
            SelectedPrinter = "";
            Task.Run(() => GetPrinterNamesDescriptions(Printers));
        }

        public void FixARMS()
        {
            try
            {
                Regedit.RunRegedit(ARMSFix);
            }
            catch
            {
                throw new Exception("Fixing ARMS is failed");
            }
        }

        public void BDrive()
        {
            try
            {
                CMD.Process(BDriveScript);
            }
            catch
            {
                throw new Exception("Disk B connection is failed");
            }
        }

        public void RemoveChromeCache()
        {
            try
            {
                Files.DeleteFilesInDirectory(ChromeCachePath);
            }
            catch
            {
                throw new Exception("Chrome cache deleting is failed");
            }
        }

        public void MakeATicket()
        {
            string mockTicketReason = "Sent_by_AdminHelper";
            _MakeATicket(mockTicketReason, TicketClaim);
        }

        private void _MakeATicket(string ticketReason = "", string ticketClaim = "")
        {
            try
            {
                string subject = $"Администраторам_#{GetDillershipTitle()}: {ticketReason}";
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

        public string GetDillershipTitle()
        {
            try
            {
                foreach (string subnetDC in SubnetsDealerships.Split('\n'))
                {
                    int subnetCidrLength = subnetDC.IndexOf(" ");
                    string subnetCidr = subnetDC.Substring(0, subnetCidrLength);
                    if (Subnet.IsRankedBySubnet(Subnet.GetLocalhostIP(), subnetCidr))
                    {
                        // from CIDR to \n
                        return subnetDC.Substring(subnetCidrLength + 1, subnetDC.Length - subnetCidrLength - 2);
                    }
                }
                return "ДЦ не определён";
            }
            catch
            {
                return "ДЦ не определён*";
            }
        }

        public async Task GetPrinterNamesDescriptions(ObservableCollection<string> printers)
        {
            try
            {
                string prefix = UserCredentials.LocationPrefix;
                await Printer.FastGetListOfPrinters(prefix, printers);
                string[] keys = {
                    "Name",
                    "Location",
                    "Comment",
                };
                // TODO: Fix memory leak
                //await Printer.GetListOfPrinters(prefix, keys, printers);
            }
            catch
            {
                throw new Exception("Getting printers is falied");
            }
        }

        public void ConnectPrinter()
        {
            if (SelectedPrinter != "")
            {
                string[] printerProperies = SelectedPrinter.Split(' ');
                string printerName = printerProperies[0];
                if (!Printer.IsPrinterInstalled(printerName))
                {
                    Printer.ConnectPrinter(SelectedPrinter);
                }
            }
        }

        public void InstallPuntoSwitcher()
        {
            try
            {
                string source = @"\\1\Distr\Punto Switcher";
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
                }
                else
                {
                    new Exception("Не удалось установить Punto Switcher на Ваш компьютер. Обратитесь к системным администраторам.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
