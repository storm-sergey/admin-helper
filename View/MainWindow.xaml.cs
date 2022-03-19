using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using AdminHelper.ViewModel;
using BusyIndicator;
using System.Drawing;

namespace AdminHelper.View
 {
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM mainWindowVM;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        
        /// <summary>
        /// Tray
        /// </summary>
        private void SetIconToMainApplication()
        {
            trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.ResourceManager.GetObject("ICON") as Icon,
                Visible = true,
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = "Помощник пользователя",

        };
            trayIcon.MouseDoubleClick += new MouseEventHandler(this._notifyicon_DoublecClick);
            //trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            //trayIcon.BalloonTipTitle = "Помощник пользователя";
            trayIcon.ShowBalloonTip(500, "Быстрая помощь", "Автоматизация Service Desk \n it Рольф", ToolTipIcon.Info);
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Помощь");
            trayMenu.MenuItems[0].MenuItems.Add("Быстрая помощь", contx);
            trayMenu.MenuItems[0].MenuItems.Add("Заявка", contx);
            trayMenu.MenuItems[0].MenuItems.Add("Принтеры", contx);
            trayMenu.MenuItems[0].MenuItems.Add("Инструкции", contx);
            trayMenu.MenuItems.Add("Закрыть", (object sender, EventArgs e) => Close());
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void _notifyicon_DoublecClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void contx(object sender, EventArgs e)
        {
            // Mock
        }

        public MainWindow()
        {
            InitializeComponent();
            mainWindowVM = new MainWindowVM();
            DataContext = mainWindowVM;
            SetIconToMainApplication();
            //Visibility = Visibility.Hidden;
        }

        public void MainMenu_Closing()
        {

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

        private void TabablzControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // MOCK
        }

        private void Button_MakeATicket_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.MakeATicket);
        }

        private void Button_ConnectPrinter_Click(object sender, RoutedEventArgs e)
        {

            WithLoadingAndMessaging(mainWindowVM.ConnectPrinter);
        }

        private void PrinterDescriptions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.ConnectPrinter);
        }

        private void Button_RedirectToConnectPrinter_Click(object sender, RoutedEventArgs e)
        {
            mainWindow_TabControl.SelectedItem = Printers;
        }

        private void Button_BDrive_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.BDrive);
        }

        private void Button_Fix_ARMS_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.FixARMS);
        }

        private void Button_DelChromeCache_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.RemoveChromeCache);
        }

        private void Button_InstallPuntoSwitcher_Click(object sender, RoutedEventArgs e)
        {

            WithLoadingAndMessaging(mainWindowVM.InstallPuntoSwitcher);
            
        }

        private void Button_ShowComputerNameAndIP_Click(object sender, RoutedEventArgs e)
        {
            BlockWindow();
            string message = mainWindowVM.ComputerNameAndIp;
            new NonModalMessage(UnblockWindow, message, "Имя компьютера и IP-адрес").Show();
        }

        private void Button_UniplanSquaresFix_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.UniplanSquaresFix);
        }


        private void Button_ClearTemps_Click(object sender, RoutedEventArgs e)
        {
            WithLoadingAndMessaging(mainWindowVM.ClearTemps);
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
