using System.Collections.ObjectModel;
using System.Windows;
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

        // TODO: Show with non-modal popup
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
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

        // TODO: show messages strings should be from model return
        public void MakeATicket()
        {
            TicketClaim = "";
            mainWindowM.MakeATicket();
            ShowMessage("Заявка отправлена");
        }

        public void ConnectPrinter()
        {
            mainWindowM.ConnectPrinter();
            ShowMessage("Принтер подключен");
        }

        public void BDrive()
        {
            mainWindowM.BDrive();
            ShowMessage("Диск B подключен");
        }

        public void FixARMS()
        {
            mainWindowM.FixARMS();
            ShowMessage("Перезапустите Internet Explorer");
        }

        public void RemoveChromeCache()
        {
            mainWindowM.RemoveChromeCache();
            ShowMessage("Кэш Google Chrome очищен");
        }

        public void InstallPuntoSwitcher()
        {
            mainWindowM.InstallPuntoSwitcher();
            ShowMessage("Punto Switcher установлен");
        }
    }
}
