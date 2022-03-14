using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using AdminHelper.ViewModel;
using BusyIndicator;

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

        private void ShowLoading(string loadingMessage = "Выполнение...")
        {
            BusyIndicator.BusyContent = loadingMessage;
            BusyIndicator.IsBusy = true;
        }

        private void ShowRandomLoading(string loadingMessage = "Выполнение...")
        {
            Array values = Enum.GetValues(typeof(IndicatorType));
            Random random = new Random();
            BusyIndicator.IndicatorType = (IndicatorType)values.GetValue(random.Next(values.Length));
            ShowLoading(loadingMessage);
        }

        private void HideLoading()
        {
            BusyIndicator.IsBusy = false;
        }

        private void BlockWindow()
        {
            IsEnabled = false;
            Opacity = 0.4;
        }

        private void UnblockWindow()
        {
            IsEnabled = true;
            Opacity = 1;
        }

        private async void WithLoadingAndMessaging(Func<Task<string>> btn_hundler_VM)
        {
            ShowRandomLoading();
            string message = await btn_hundler_VM();
            HideLoading();
            BlockWindow();
            new NonModalMessage(UnblockWindow, message).Show();
        }

        private void Button_CopyDesktop_Click(object sender, RoutedEventArgs e)
        {
            BlockWindow();
            new CopyWorkspace(UnblockWindow).Show();
        }

        private void Button_Test(object sender, RoutedEventArgs e)
        {

        }

        private void TabablzControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // MOCK
        }

        private void Button_Make_A_Ticket(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.MakeATicket);
        }

        private void Button_Connect_Printer(object sender, RoutedEventArgs e)
        {

            WithLoadingAndMessaging(mainWindowVM.ConnectPrinter);
        }

        private void PrinterDescriptions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.ConnectPrinter);
        }

        private void Button_Redirect_To_Connect_Printer(object sender, RoutedEventArgs e)
        {
            mainWindow_TabControl.SelectedItem = Printers;
        }

        private void Button_B_Drive_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.BDrive);
        }

        private void Button_Fix_ARMS_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.FixARMS);
        }

        private void Button_Del_Chrome_Cache_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.RemoveChromeCache);
        }

        private void Button_Install_Punto_Switcher(object sender, RoutedEventArgs e)
        {

            WithLoadingAndMessaging(mainWindowVM.InstallPuntoSwitcher);
            
        }

        private void Button_Show_Computer_Name_And_IP(object sender, RoutedEventArgs e)
        {

        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private async void HandleLinkClick(object sender, RoutedEventArgs e)
        {
            ShowRandomLoading("Открытие ссылки в браузере");
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
            await Task.Run(() => System.Threading.Thread.Sleep(2000));
            HideLoading();
        }
    }
}
