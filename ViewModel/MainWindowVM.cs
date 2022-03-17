using System.Collections.ObjectModel;
using AdminHelper.Model;
using System.Threading.Tasks;
using System.Windows.Media;
using System;
using AdminHelper.lib;
using AdminHelper.View;

namespace AdminHelper.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        private readonly MainWindowM mainWindowM;

        public MainWindowVM()
        {
            mainWindowM = MainWindowM.Singleton;
        }

        public SolidColorBrush IconsColor
        {
            get => mainWindowM.IconsColor;
        }

        public bool GridIsEnabled
        {
            get => mainWindowM.GridIsEnabled;
            set
            {
                mainWindowM.GridIsEnabled = value;
                OnPropertyChange("GridIsEnabled");
            }
        }

        public double GridOpacity
        {
            get => mainWindowM.GridOpacity;
            set
            {
                mainWindowM.GridOpacity = value;
                OnPropertyChange("GridOpacity");

            }
        }

        public bool IsTicketClaimFilled
        {
            get => mainWindowM.IsTicketClaimFilled;
            set
            {
                mainWindowM.IsTicketClaimFilled = value;
                OnPropertyChange("IsTicketClaimFilled");
            }
        }

        public string TicketClaim
        {
            get => mainWindowM.TicketClaim;
            set
            {
                if (String.IsNullOrEmpty(value)
                || String.IsNullOrWhiteSpace(value))
                {
                    IsTicketClaimFilled = false;
                }
                else
                {
                    IsTicketClaimFilled = true;
                }

                mainWindowM.TicketClaim = value;
                OnPropertyChange("TicketClaim");
            }
        }

        public JsonTreeNode TicketTemplatesJson
        {
            get => mainWindowM.TicketTemplatesJson;
        }

        public JsonTreeNode PrintersMapJson
        {
            get => mainWindowM.PrintersMapJson;
        }

        public ObservableCollection<string> TicketThemesList
        {
            get => mainWindowM.TicketThemesList;
        }

        public int SelectedTicketThemeIndex
        {
            get => mainWindowM.SelectedTicketThemeIndex;
            set
            {
                mainWindowM.SelectedTicketThemeIndex = value;
                if (value > -1
                && value < mainWindowM.TicketThemesList.Count)
                {
                    TicketClaim = mainWindowM.GetTicketClaimByTheme(mainWindowM.TicketThemesList[value]);
                }
            }
        }

        public string SelectedTicketTheme
        {
            get => mainWindowM.SelectedTicketTheme;
            set
            {
                if (Json.IsNotNullEmptySpace(value))
                {
                    mainWindowM.SelectedTicketTheme = value;
                }
            }
        }

        public int GetTicketThemeIndex(string value)
        {
            return mainWindowM.GetTicketThemeIndex(value);
        }

        public string GetTicketClaimByTicketThemeIndex(int ticketIndex)
        {
            string ticketTheme = mainWindowM.TicketThemesList[ticketIndex];
            return GetTicketClaimByTicketTheme(ticketTheme);
        }

        public string GetTicketClaimByTicketTheme(string tickeTheme)
        {
            return mainWindowM.GetTicketClaimByTheme(tickeTheme);
        }

        public ObservableCollection<string> Dealerships
        {
            get => mainWindowM.Dealerships;
        }

        public int UserDealership
        {
            get => mainWindowM.TicketDialershipIndex;
            set
            {
                mainWindowM.UpdateDialershipIndex(value);
                OnPropertyChange("UserDealership");
            }
        }

        //public void ComboBox_UserDealership_SelectionChanged()
        //{
        //    mainWindow.UpdatePrinterMenu();
        //}

        public string UserDealershipName
        {
            get => mainWindowM.Dealerships[mainWindowM.TicketDialershipIndex];
        }

        public ObservableCollection<string> PrintserverPrinters
        {
            get => mainWindowM.PrintserverPrinters;
        }
        public ObservableCollection<LocalPrinter> LocalPrinters
        {
            get => mainWindowM.LocalPrinters;
        }

        public string NewPrinterNumber
        {
            get => mainWindowM.NewPrinterNumber;
            set
            {
                if (mainWindowM.ValidateNewPrinterNumber(value))
                {
                    mainWindowM.NewPrinterNumber = value;
                    string findedPrinter = mainWindowM.FindPrinterByNumber(value);
                    mainWindowM.PrinterLink = mainWindowM.IsNewPrinterFound ? $@"\\{mainWindowM.PrintServer}\{findedPrinter}" : findedPrinter;
                    IsNewPrinterFound = mainWindowM.IsNewPrinterFound;
                    OnPropertyChange("NewPrinterNumber");
                    OnPropertyChange("PrinterLink");
                }
            }
        }

        public string PrinterLink
        {
            get => mainWindowM.PrinterLink;
        }

        public bool IsNewPrinterFound
        {
            get => mainWindowM.IsNewPrinterFound;
            set
            {
                if (value)
                {
                    mainWindowM.PrinterLink_Valid = System.Windows.Visibility.Visible;
                    mainWindowM.PrinterLink_Unvalid = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    mainWindowM.PrinterLink_Valid = System.Windows.Visibility.Collapsed;
                    mainWindowM.PrinterLink_Unvalid = System.Windows.Visibility.Visible;
                }
                OnPropertyChange("IsNewPrinterFound");
                OnPropertyChange("PrinterLink_Valid");
                OnPropertyChange("PrinterLink_Unvalid");
            }
        }

        public System.Windows.Visibility PrinterLink_Valid
        {
            get => mainWindowM.PrinterLink_Valid;
        }

        public System.Windows.Visibility PrinterLink_Unvalid
        {
            get => mainWindowM.PrinterLink_Unvalid;
        }

        public string ComputerNameAndIp
        {
            get => mainWindowM.GetComputerNameAndIp();
        }

        public string ServiceDeskContacts
        {
            get => mainWindowM.GetServiceDeskContacts();
        }

        public async Task<string> MakeATicket()
        {
            string makingATicketResult = await mainWindowM.MakeATicket();
            TicketClaim = "";
            return makingATicketResult;
        }

        public async Task<string> RefreshPrintersList()
        {
            string mock = await mainWindowM.RefreshLocalPrinters();
            OnPropertyChange("LocalPrinters");
            return mock;
        }

        public async Task<string> ConnectPrinter(string printer)
        {
            string message = await mainWindowM.ConnectPrinter(printer);
            OnPropertyChange("LocalPrinters");
            return message;
        }

        public async Task<string> SetDefaultPrinter(string printerName)
        {
            string message = await mainWindowM.SetDefaultPrinter(printerName);
            OnPropertyChange("LocalPrinters");
            return message;
        }

        public Task<string> BDrive()
        {
            return mainWindowM.BDrive();
        }

        public Task<string> FixARMS()
        {
            return mainWindowM.FixARMS();
        }
        public Task<string> Fix_ACRolf_DisplayOfGoodsMovement()
        {
            return mainWindowM.Fix_ACRolf_DisplayOfGoodsMovement();
        }

        public Task<string> RemoveChromeCache()
        {
            return mainWindowM.RemoveChromeCache();
        }

        public Task<string> InstallPuntoSwitcher()
        {
            return mainWindowM.InstallPuntoSwitcher();
        }

        public Task<string> ClearTemps()
        {
            return mainWindowM.ClearTemps();
        }

        public Task<string> UniplanSquaresFix()
        {
            return mainWindowM.UniplanSquaresFix();
        }

        public Task<string> Install7zip19()
        {
            return mainWindowM.Install7zip19();
        }
    }
}
