using System.Collections.ObjectModel;
using AdminHelper.Model;
using static AdminHelper.Globals;

namespace AdminHelper.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        private readonly MainWindowM mainWindowM;

        public MainWindowVM()
        {
            mainWindowM = new MainWindowM();
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

        public string UserDealership
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

        public void MakeATicket()
        {
            TicketClaim = "";
            mainWindowM.MakeATicket();
        }

        public void ConnectPrinter()
        {
            mainWindowM.ConnectPrinter();
        }

        public void BDrive()
        {
            mainWindowM.BDrive();
        }

        public void FixARMS()
        {
            mainWindowM.FixARMS();
        }

        public void RemoveChromeCache()
        {
            mainWindowM.RemoveChromeCache();
        }

        public void InstallPuntoSwitcher()
        {
            mainWindowM.InstallPuntoSwitcher();
        }
    }
}
