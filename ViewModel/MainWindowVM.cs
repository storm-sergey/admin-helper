using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Threading;
using System.Windows.Controls;
using AdminHelper.Model;
using static AdminHelper.Globals;
using System.Threading.Tasks;

namespace AdminHelper.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        private readonly MainWindowM mainWindowM;

        public MainWindowVM()
        {
            mainWindowM = new MainWindowM();
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

        public string TicketClaim
        {
            get => mainWindowM.TicketClaim;
            set
            {
                mainWindowM.TicketClaim = value;
                OnPropertyChange("TicketClaim");
            }
        }

        public ObservableCollection<string> Dealerships
        {
            get => mainWindowM.Dealerships;
        }

        public ObservableCollection<string> Printers
        {
            get => mainWindowM.Printers;
        }

        public int UserDealership
        {
            get => mainWindowM.UserDealership;
            set
            {
                mainWindowM.UserDealership = value;
                OnPropertyChange("UserDealership");
            }
        }

        public string SelectedPrinter
        {
            get => mainWindowM.SelectedPrinter;
            set
            {
                mainWindowM.SelectedPrinter = value;
                mainWindowM.PrinterLink = $@"\\{PRINT_SERVER}\{value.Split(' ')[0]}";
                OnPropertyChange("SelectedPrinter");
                OnPropertyChange("PrinterLink");
            }
        }

        public string PrinterLink
        {
            get => mainWindowM.PrinterLink;
        }

        public string ComputerNameAndIp
        {
            get => mainWindowM.GetComputerNameAndIp();
        }

        public async Task<string> MakeATicket()
        {
            string makingATicketResult = await mainWindowM.MakeATicket();
            TicketClaim = "";
            return makingATicketResult;
        }

        public Task<string> ConnectPrinter()
        {
            return mainWindowM.ConnectPrinter();
        }

        public Task<string> BDrive()
        {
            return mainWindowM.BDrive();
        }

        public Task<string> FixARMS()
        {
            return mainWindowM.FixARMS();
        }

        public Task<string> RemoveChromeCache()
        {
            return mainWindowM.RemoveChromeCache();
        }

        public Task<string> InstallPuntoSwitcher()
        {
            return mainWindowM.InstallPuntoSwitcher();
        }
    }
}
