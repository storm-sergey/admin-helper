using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using AdminHelper.lib;
using static AdminHelper.Globals;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;

namespace AdminHelper.Model
{
    public sealed class MainWindowM
    {
        static MainWindowM() {}
        // app
        private readonly string ServiceDeskContacts;
        private readonly String ARMSFix;
        private readonly string BDriveScript;
        private readonly string ClearTempsScript;
        public readonly string D_Program_Files;
        public readonly string ChromeCachePath;
        public readonly string AppDataLocalTempPath;
        public readonly byte[] SubnetsDealershipsLocal;
        private readonly JsonTreeNode SubnetsDealershipsJson;
        public string UserDealership;
        public ObservableCollection<string> Dealerships;
        public double GridOpacity;
        public bool GridIsEnabled;
        public SolidColorBrush IconsColor;
        // printers
        public readonly byte[] PrintServersLocal;
        public readonly byte[] PrintersMapLocal;
        public readonly JsonTreeNode PrintServersJson;
        public readonly JsonTreeNode PrintersMapJson;
        public readonly Dictionary<string, string> PrintServers;
        public Dictionary<string, string> PrintersNumberName;
        public ObservableCollection<string> PrintserverPrinters;
        public ObservableCollection<LocalPrinter> LocalPrinters;
        public readonly string PrinterNotFoundMessage;
        public readonly string PrintServerNotFoundMessage;
        public string PrintServer;
        public string PrinterLink;
        public string NewPrinterNumber;
        public bool IsNewPrinterFound;
        public System.Windows.Visibility PrinterLink_Valid;
        public System.Windows.Visibility PrinterLink_Unvalid;
        private readonly Regex NewPrinterNumberValidation;
        // tickets
        public byte[] TicketsThemesLocal;
        public JsonTreeNode TicketTemplatesJson;
        public Dictionary<string, string> FullPathWithLeafData;
        public Dictionary<string, string> EachTicketThemeFullPathWithLeavesData;
        public ObservableCollection<string> TicketThemesList;
        public int TicketDialershipIndex;
        public bool IsTicketClaimFilled;
        public string TicketClaim;
        public string SelectedTicketTheme;
        public int SelectedTicketThemeIndex;

        private MainWindowM()
        {
            GridIsEnabled = true;
            GridOpacity = 1;
            IconsColor = (SolidColorBrush)new BrushConverter().ConvertFrom(ICONS_COLOR);
            BDriveScript = Properties.Resources.B_Drive_bat;
            ARMSFix = Properties.Resources.ARMS_Fix_reg;
            ClearTempsScript = Properties.Resources.Clear_temps_cmd;
            D_Program_Files = @"D:\Program Files";
            ChromeCachePath = $@"D:\Users\{UserCredentials.UserName}\AppData\Local\Google\Chrome\User Data\Default\Cache";
            AppDataLocalTempPath = $@"D:\Users\{UserCredentials.UserName}\AppData\Local\Temp";
            ServiceDeskContacts = "\n\n"
                        + $"Телефон: {SERVICE_DESK_EMAIL}\n"
                        + "                                            \n"
                        + $"Почта: {SERVICE_DESK_PHONE_NUMBER}\n";
            // TODO: test without internter connection
            // Dealership
            SubnetsDealershipsLocal = Properties.Resources.Subnets;
            SubnetsDealershipsJson = GetRoot("Подсети", SUBNETS_ENDPOINT, SUBNETS_FILE, SubnetsDealershipsLocal).GetAwaiter().GetResult();
            Dealerships = GetDealerships();
            UserDealership = GetUserDealership();
            TicketDialershipIndex = GetTicketDillershipIndex();
            // Printers
            PrintServersLocal = Properties.Resources.Print_servers;
            PrintersMapLocal = Properties.Resources.Printers_rooms;
            PrintServersJson = GetRoot("Принтсерверы", PRINT_SERVERS_ENDPOINT, PRINT_SERVERS_FILE, PrintServersLocal).GetAwaiter().GetResult();
            PrintersMapJson = GetRoot("Принтеры", PRINTERS_ENDPOINT, PRINTERS_FILE, PrintersMapLocal).GetAwaiter().GetResult();
            PrintServers = Format_PrintServers(GetPrintServersFromJSON(PrintServersJson.GetChildrenEnumerator()));
            PrintServer = GetPrintServer(PrintServers, UserDealership);
            NewPrinterNumberValidation = new Regex(@"^[0-9]{0,3}$");
            PrinterNotFoundMessage = "Принтер не найден";
            PrintServerNotFoundMessage = "Принтсервер не задан";
            IsNewPrinterFound = false;
            PrinterLink_Valid = System.Windows.Visibility.Hidden;
            PrinterLink_Unvalid = System.Windows.Visibility.Collapsed;
            LocalPrinters = GetLocalInstalledPrinterNames();
            PrintserverPrinters = new ObservableCollection<string>();
            PrintserverPrinters.CollectionChanged += ServerPrinters_AddingNewPrinter;
            PrintersNumberName = new Dictionary<string, string>();
            Task.Run(() => GetServerPrintersNames(PrintserverPrinters, PrintServer));
            // Ticket theme
            IsTicketClaimFilled = false;
            TicketClaim = "";
            SelectedTicketTheme = "";
            SelectedTicketThemeIndex = -1;
            TicketsThemesLocal = Properties.Resources.Ticket_templates;
            TicketTemplatesJson = GetRoot("Заявки", TICKETS_ENDPOINT, TICKETS_FILE, TicketsThemesLocal).GetAwaiter().GetResult();
            EachTicketThemeFullPathWithLeavesData = Json.GetEachNodeFullPathWithLeavesData(TicketTemplatesJson);
            TicketThemesList = GetTicketThemesList(EachTicketThemeFullPathWithLeavesData);
        }

        private static readonly MainWindowM singleton = new MainWindowM();

        public static MainWindowM Singleton
        {
            get => singleton;
        }

        public void UpdateDialershipIndex(int value)
        {
            TicketDialershipIndex = value;
            UserDealership = Dealerships[value];
            UpdatePrintersPanelSettings();
        }

        private void UpdatePrintersPanelSettings()
        {
            PrintServer = GetPrintServer(PrintServers, UserDealership);
            PrintserverPrinters.Clear();
            PrintersNumberName.Clear();
            Task.Run(() => GetServerPrintersNames(PrintserverPrinters, PrintServer));
            
        }

        public string GetTicketClaimByTheme(string ticketTheme)
        {
            if (EachTicketThemeFullPathWithLeavesData.ContainsKey(ticketTheme))
            {
                return EachTicketThemeFullPathWithLeavesData[ticketTheme];
            }
            return "";
        }

        public int GetTicketThemeIndex()
        {
            return GetItemIndexInCollection(TicketThemesList, SelectedTicketTheme);
        }

        public int GetTicketThemeIndex(string selectedTicketTheme)
        {
            return GetItemIndexInCollection(TicketThemesList, selectedTicketTheme);
        }

        private ObservableCollection<string> GetTicketThemesList(Dictionary<string, string> TicketsWithTemplates)
        {
            ObservableCollection<string> themesList = new ObservableCollection<string>();
            Dictionary<string, string>.KeyCollection.Enumerator nodesEnum = TicketsWithTemplates.Keys.GetEnumerator();
            while (nodesEnum.MoveNext())
            {
                themesList.Add(nodesEnum.Current);
            }
            return themesList;
        }

        private async Task<JsonTreeNode> GetRoot(string jsonRootName, string apiEndpoint, string filePath, byte[] resource)
        {
            byte[] jsonData = await _GetJson(apiEndpoint, filePath, resource);
            return Json.Parse(jsonData, jsonRootName);
        }

        private async Task<byte[]> _GetJson(string apiEndpoint, string filePath, byte[] programResource)
        {
            byte[] recievedJson = null;
            if (Json.IsNotNullEmptySpace(apiEndpoint))
            {
                recievedJson = await Http.Request("GET", apiEndpoint);
            }
            else if (Json.IsNotNullEmptySpace(filePath) && File.Exists(filePath))
            {
                recievedJson = File.ReadAllBytes(filePath);
            }
            return Json.Validate(recievedJson) ? recievedJson : programResource;
        }

        public async Task<string> FixARMS()
        {
            try
            {
                await Task.Run(() => Regedit.RunRegedit(ARMSFix));
                return "Перезапустите Internet Explorer";
            }
            catch (Exception ex)
            {
                return AppException.Handle("Fixing ARMS is failed", ex);
            }
        }

        public async Task<string> Fix_ACRolf_DisplayOfGoodsMovement()
        {
            try
            {
                await Task.Run(() => _Fix_ACRolf_DisplayOfGoodsMovement());
                return "Необходимо перезапустить Internet Explorer";
            }
            catch (Exception ex)
            {
                return AppException.Handle("ACRolf display of goods movement fixing is failed", ex);
            }
        }

        private void _Fix_ACRolf_DisplayOfGoodsMovement()
        {
            string path = @"Software\Microsoft\Internet Explorer\Main\EnterpriseMode";
            string key = "CurrentVersion";
            int value = 5;
            using (RegistryKey myKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                if (myKey != null)
                {
                    myKey.SetValue(key, value, RegistryValueKind.DWord);
                    myKey.Close();
                }
            }
        }

        public async Task<string> Install7zip19()
        {
            string sevenZip_msi = Environment.Is64BitOperatingSystem ? "7z1900-x64.msi" : "7z1900.msi";
            string msi_path = Globals.SHARE;
            string success_message = "7zip версии 19 установлен";
            try
            {
                try
                {
                    await Task.Run(() => TempFiles.MakeTempHook(sevenZip_msi, msi_path, _Install7zip19));
                    return success_message;
                }
                catch
                {
                    await Task.Run(() => _Install7zip19($@"{msi_path}{sevenZip_msi}"));
                    return success_message;
                }
            }
            catch (Exception ex)
            {
                return AppException.Handle("7zip 19 installation is failed", ex);
            }
        }

        private void _Install7zip19(string fullpath)
        {
            using (Process installerProcess = new Process())
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    Arguments = $@"/i  {fullpath} /passive",
                    FileName = "msiexec"
                };
                installerProcess.StartInfo = processInfo;
                installerProcess.Start();
                installerProcess.WaitForExit();
            }
        }

        public async Task<string> BDrive()
        {
            try
            {
                if (Subnet.IsNetworkAvailable())
                {
                    await Task.Run(() => CMD.Process(BDriveScript));
                    return "Диск B подключен";
                }
                else
                {
                    return "Отсутствует интернет подключение";
                }
            }
            catch (Exception e)
            {
                return AppException.Handle("Disk B connection is failed", e);
            }
        }

        public async Task<string> RemoveChromeCache()
        {
            try
            {
                await Task.Run(() => Files.DeleteFilesInDirectory(ChromeCachePath));
                return "Кэш Google Chrome очищен";
            }
            catch (Exception ex)
            {
                return AppException.Handle("Chrome cache deleting is failed", ex);
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
            catch (Exception ex)
            {
                return AppException.Handle("Temp cleaning is failed", ex);
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
            catch (Exception ex)
            {
                return AppException.Handle("UNIPLAN fixing is failed", ex);
            }
        }

        private void _UniplanSquaresFix()
        {
            string path = @"Software\Microsoft\Internet Explorer\BrowserEmulation";
            string key = "IntranetCompatibilityMode";
            int value = 0;
            using (RegistryKey myKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                if (myKey != null)
                {
                    myKey.SetValue(key, value, RegistryValueKind.DWord);
                    myKey.Close();
                }
            }
        }

        // TODO: Ticket reason would be here
        public async Task<string> MakeATicket()
        {
            if (Subnet.IsNetworkAvailable())
            {
                string TicketReason = Json.IsNotNullEmptySpace(SelectedTicketTheme) ? SelectedTicketTheme : "Заявка пользователя";
                return await Task.Run(() => _MakeATicket(TicketReason, TicketClaim));
            }
            else
            {
                return "Отсутствует интернет подключение";
            }
        }

        private string _MakeATicket(string ticketReason = "", string ticketClaim = "")
        {
            try
            {
                string[] s = ticketReason.Split('-');
                string shortTicketReason = s[s.Length - 1];
                ticketClaim = ticketClaim.Replace("\n", "<br>");

                string subject = $"{ticketReason}";
                string htmlBody =
                    $"<b>#{Dealerships[TicketDialershipIndex]}</b><br><br>" +
                    $"Login: {UserCredentials.UserName}<br>" +
                    $"Computer: {UserCredentials.MachineName}<br>" +
                    $"IP: {Subnet.GetLocalhostIP()}<br>" +
                    (shortTicketReason != "" ? $"<br><div><b>{shortTicketReason}</b></div>" : "") +
                    (ticketClaim != "" ? $"<div>{ticketClaim}</div>" : "");
                Outlook.SendEmail(subject, htmlBody, SERVICE_DESK_EMAIL);
                return "Заявка отправлена";
            }
            catch (Exception ex)
            {
                return AppException.Handle("Ticket making is failed", ex);
            }
        }

        public ObservableCollection<string> GetDealerships()
        {
            try
            {
                ObservableCollection<string> dealerships = new ObservableCollection<string>();
                HashSet<string> singlesDealerships = new HashSet<string>();
                HashSet<JsonTreeNode>.Enumerator subnets = SubnetsDealershipsJson.GetChildrenEnumerator();
                while (subnets.MoveNext())
                {
                    string subnet = subnets.Current.GetData();
                    string dealership = subnets.Current.GetChildData();
                    
                    if (dealership[0] != '*')
                    {
                        singlesDealerships.Add(dealership.ToUpperInvariant());
                    }
                }

                singlesDealerships.Remove("НЕ ОПРЕДЕЛЕН");
                foreach (string dealership in singlesDealerships)
                {
                    dealerships.Add(dealership);
                }
                return dealerships;
            }
            catch (Exception ex)
            {
                AppException.Handle("Getting dealerships is falied", ex);
                return null;
            }
        }

        private string GetUserDealership()
        {
            try
            {
                HashSet<JsonTreeNode>.Enumerator subnets = SubnetsDealershipsJson.GetChildrenEnumerator();
                while (subnets.MoveNext())
                {
                    string subnetCidr = subnets.Current.GetData();
                    if (Subnet.IsRankedBySubnet(Subnet.GetLocalhostIP(), subnetCidr))
                    {
                        // from CIDR to \n
                        string dealershipTitle = subnets.Current.GetChildData();
                        return dealershipTitle.ToUpper();
                    }
                }
                AppException.Handle(new Exception("Dealership title is not define"));
                return "Не определена";
            }
            catch (Exception ex)
            {
                AppException.Handle(ex);
                return "Не определена";
            }

        }

        private int GetTicketDillershipIndex()
        {
            return GetItemIndexInCollection(Dealerships, UserDealership.ToUpper());
        }

        private int GetItemIndexInCollection(ObservableCollection<string> collection, string item)
        {
            if (!String.IsNullOrEmpty(item))
            {
                return collection.IndexOf(item);
            }
            return -1;
        }

        /// <summary>
        /// This app has an uppercase subnet naming convention
        /// </summary>
        /// <param name="DC_PrintServer">
        /// Pairs where the key is a subnet name, the value is a print server name
        /// </param>
        /// <returns>
        /// Dict with uppercase keys and lowercase values
        /// </returns>
        private Dictionary<string, string> Format_PrintServers(Dictionary<string, string> DC_PrintServer)
        {
            Dictionary<string, string> formatedDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> DC in DC_PrintServer)
            {
                formatedDict.Add(DC.Key.ToUpper(), DC.Value.ToLower());
            }
            return formatedDict;
        }

        private Dictionary<string, string> GetPrintServersFromJSON(HashSet<JsonTreeNode>.Enumerator locationsPrintServersJson)
        {
            try
            {
                Dictionary<string, string> locationsPrintServers = new Dictionary<string, string>();
                while (locationsPrintServersJson.MoveNext())
                {
                    string location = locationsPrintServersJson.Current.GetData();
                    string printServer = Json.GetOneLeafData(locationsPrintServersJson.Current);
                    locationsPrintServers.Add(location, printServer);
                }
                return locationsPrintServers;
            }
            catch (Exception ex)
            {
                AppException.Handle("Getting print servers from JSON error", ex);
                return null;
            }
        }

        public string GetPrintServer(Dictionary<string, string> printServers, string userDealerShip)
        {
            if (printServers.ContainsKey(userDealerShip))
            {
                return printServers[userDealerShip];
            }
            return null;
        }

        public bool ValidateNewPrinterNumber(string printerName)
        {
            return NewPrinterNumberValidation.IsMatch(printerName);
        }

        public string FindPrinterByNumber(string number)
        {
            if (PrintServer == null)
            {
                IsNewPrinterFound = false;
                return PrintServerNotFoundMessage;
            }

            if (!String.IsNullOrEmpty(number)
            && PrintersNumberName.ContainsKey(number))
            {
                IsNewPrinterFound = true;
                return PrintersNumberName[number];
            }
            else if (String.IsNullOrEmpty(number))
            {
                IsNewPrinterFound = false;
                return "";
            }
            else
            {
                while (number.Length < 3)
                {
                    number = number.Insert(0, "0");
                    if (PrintersNumberName.ContainsKey(number))
                    {
                        IsNewPrinterFound = true;
                        return PrintersNumberName[number];
                    }
                }
                IsNewPrinterFound = false;
                return PrinterNotFoundMessage;
            }
        }

        public ObservableCollection<LocalPrinter> GetLocalInstalledPrinterNames()
        {
            try
            {
                string defaultPrinterName = Printers.GetDefaultPrinterName();
                Regex regex_printServer = new Regex($@"^[\\]*{PrintServer}[\\]*{UserCredentials.LocationPrefix}[[:ascii:]]*", RegexOptions.IgnoreCase);
                Regex regex_localPrinters = new Regex($@"^[\\]*{UserCredentials.LocationPrefix}[[:ascii:]]*", RegexOptions.IgnoreCase);

                ObservableCollection<LocalPrinter> printers = new ObservableCollection<LocalPrinter>();
                foreach (string printerName in Printers.GetLocalInstalledPrinterNames())
                {
                    if (regex_printServer.IsMatch(printerName)
                    || regex_localPrinters.IsMatch(printerName))
                    {
                        if (Printers.IsPrinterInstalledAndValid(printerName))
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                LocalPrinter printer = new LocalPrinter()
                                {
                                    PrinterName = printerName,
                                    PrinterDescription = "",
                                    Button_SetDeafult_Visibility = System.Windows.Visibility.Visible,
                                    Expander_Visibility = System.Windows.Visibility.Visible,
                                    Button_ConnectDirectly_Visibility = System.Windows.Visibility.Visible,
                                    Button_ConnectDirectly_IsEnabled = false,
                                };
                                if (defaultPrinterName == printerName)
                                {
                                    printer.PrinterDescription = "Установлен по умолчанию";
                                    printer.Button_SetDeafult_Visibility = System.Windows.Visibility.Collapsed;
                                }
                                printers.Add(printer);
                            });
                        }
                    }
                }
                return printers;
            }
            catch (Exception ex)
            {
                AppException.Handle("Getting local printer names list is falied", ex);
                return null;
            }
        }

        private void ServerPrinters_AddingNewPrinter(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startIndexOfPrinterNumber;
                int endIndexOfPrinterNumber;
                string printerNumber;
                foreach (string printerName in e.NewItems)
                {
                    startIndexOfPrinterNumber = printerName.IndexOf('-');
                    endIndexOfPrinterNumber = printerName.IndexOf("-", startIndexOfPrinterNumber + 1);
                    if (endIndexOfPrinterNumber != -1
                    && endIndexOfPrinterNumber - startIndexOfPrinterNumber <= 3)
                    {
                        printerNumber = printerName.Substring(
                            startIndexOfPrinterNumber + 1,
                            endIndexOfPrinterNumber - 1);
                        if (!PrintersNumberName.ContainsKey(printerNumber))
                        {
                            PrintersNumberName.Add(printerNumber, printerName);
                        }
                    }
                    else
                    {
                        printerNumber = Regex.Match(printerName.Substring(startIndexOfPrinterNumber + 1), @"^\d{1,3}").ToString();
                        if (!PrintersNumberName.ContainsKey(printerNumber))
                        {
                            PrintersNumberName.Add(printerNumber, printerName);
                        }
                    }
                }
            }
        }

        public async Task GetServerPrintersNames(ObservableCollection<string> printers, string host)
        {
            try
            {
                string prefix = UserCredentials.LocationPrefix;
                await Printers.FastGetListOfPrinters(host, prefix, printers);

                // TODO: Fix memory leak with GetListOfPrinters
                //string[] keys = {
                //    "Name",
                //    "Location",
                //    "Comment",
                //};
                //await Printer.GetListOfPrinters(prefix, keys, printers);
            }
            catch (Exception ex)
            {
                AppException.Handle("Getting printers is falied", ex);
            }
        }

        public async Task<string> RefreshLocalPrinters()
        {
            return await Task.Run(() => _RefreshLocalPrinters());
        }

        private string _RefreshLocalPrinters()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                LocalPrinters.Clear();
                foreach (LocalPrinter installedPrinter in GetLocalInstalledPrinterNames())
                {
                    LocalPrinters.Add(installedPrinter);
                }
            });
            return "";
        }

        public async Task<string> ConnectPrinter(string printer)
        {
            return await Task.Run(() => _ConnectPrinter(printer));
        }

        private string _ConnectPrinter(string PrinterLink)
        {
            if (PrinterLink != "")
            {
                if (!Printers.IsPrinterInstalledAndValid(PrinterLink))
                {
                    if (Subnet.IsNetworkAvailable())
                    {
                        Printers.ConnectPrinter(PrinterLink);
                        string printerName = PrinterLink.Substring(PrinterLink.LastIndexOf('\\') + 1);
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            LocalPrinter printer = new LocalPrinter()
                            {
                                PrinterName = printerName,
                                PrinterDescription = "",
                                Button_SetDeafult_Visibility = System.Windows.Visibility.Visible,
                                Expander_Visibility = System.Windows.Visibility.Visible,
                                Button_ConnectDirectly_Visibility = System.Windows.Visibility.Visible,
                                Button_ConnectDirectly_IsEnabled = false,
                            };
                            _RefreshLocalPrinters();
                        });
                    }
                    else
                    {
                        return "Отсутствует интернет подключение";
                    }
                }
                return "Принтер подключен";
            }
            return $"Не найден принтер\n{PrinterLink}";
        }

        public async Task<string> SetDefaultPrinter(string printerName)
        {
            return await Task.Run(() => _SetDefaultPrinter(printerName));
        }

        private string _SetDefaultPrinter(string printerName)
        {
            _RefreshLocalPrinters();
            if (printerName == null)
            {
                return $"Принтер \"{printerName}\" больше не найден";
            }
            if (printerName == Printers.GetDefaultPrinterName())
            {
                return "Принтер уже по умолчанию";
            }

            Printers.SetDefaultPrinter(printerName);
            _RefreshLocalPrinters();

            return $"Принтер {printerName}\nустановлен по умолчанию";
        }

        public async Task<string> InstallPuntoSwitcher()
        {
            return await Task.Run(() => _InstallPuntoSwitcher());
        }

        private string _InstallPuntoSwitcher()
        {
            if (Subnet.IsNetworkAvailable())
            {
                try
                {
                    string source = @"\\1\IT_Distrib\Punto Switcher";
                    string shortcut = "Punto Switcher.lnk";
                    if (!Files.CheckDirectoryExistence(source))
                    {
                        throw new Exception("Punto Switcher недоступен для скачивания, "
                            + "обратитесь к системным администраторам");
                    }
                    string destination = $@"{D_Program_Files}\Punto Switcher";
                    Files.CopyDirectory(source, destination);

                    if (Files.CheckDirectoryExistence($@"{destination}"))
                    {
                        bool overwriteFile = true;
                        Files.CopyFile(shortcut, destination, UserCredentials.LocalDesktop, overwriteFile);
                        Files.RunFile(destination, shortcut);
                        return "Punto Switcher установлен";
                    }
                    else
                    {
                        throw new Exception("Ошибка установки PuntoSwitcher");
                    }
                }
                catch (Exception ex)
                {
                    return AppException.Handle("Не удалось установить Punto Switcher на Ваш компьютер.\nОбратитесь к системным администраторам.", ex);
                }
            }
            else
            {
                return "Отсутствует интернет подключение";
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

        public string GetServiceDeskContacts()
        {
            return ServiceDeskContacts;
        }
    }

    // TODO ref
    public class LocalPrinter
    {
        public string PrinterName
        {
            get; set;
        }
        public string PrinterDescription
        {
            get; set;
        }
        public System.Windows.Visibility Expander_Visibility
        {
            get; set;
        }
        public System.Windows.Visibility Button_SetDeafult_Visibility
        {
            get; set;
        }
        public System.Windows.Visibility Button_ConnectDirectly_Visibility
        {
            get; set;
        }
        public bool Button_ConnectDirectly_IsEnabled
        {
            get; set;
        }
    }
}
