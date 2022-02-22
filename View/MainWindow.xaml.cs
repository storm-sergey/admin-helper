using System.Windows;
using AdminHelper.ViewModel;

namespace AdminHelper.View
 {
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM mainWindowVM;
               
        public MainWindow()
        {
            InitializeComponent();
            mainWindowVM = new MainWindowVM();
            DataContext = mainWindowVM;
        }

        private void Button_CopyDesktop_Click(object sender, RoutedEventArgs e)
        {
            CopyWorkspace copyingDesktop = new CopyWorkspace();
            copyingDesktop.Show();
        }

        private void TabablzControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // MOCK
        }

        private void Button_Make_A_Ticket(object sender, RoutedEventArgs e)
        {
            mainWindowVM.MakeATicket();
        }

        private void Button_Connect_Printer(object sender, RoutedEventArgs e)
        {
            mainWindowVM.ConnectPrinter();
        }

        private void PrinterDescriptions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainWindowVM.ConnectPrinter();
        }

        private void Button_Redirect_To_Connect_Printer(object sender, RoutedEventArgs e)
        {
            mainWindow_TabControl.SelectedItem = Printers;
        }

        private void Button_B_Drive_Click(object sender, RoutedEventArgs e)
        {
            mainWindowVM.BDrive();
        }

        private void Button_Fix_ARMS_Click(object sender, RoutedEventArgs e)
        {
            mainWindowVM.FixARMS();
        }

        private void Button_Del_Chrome_Cache_Click(object sender, RoutedEventArgs e)
        {
            mainWindowVM.RemoveChromeCache();
        }

        private void Button_Install_Punto_Switcher(object sender, RoutedEventArgs e)
        {
            mainWindowVM.InstallPuntoSwitcher();
        }

        private void Button_Show_Computer_Name_And_IP(object sender, RoutedEventArgs e)
        {

        }
    }
}
