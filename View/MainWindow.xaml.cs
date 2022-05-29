using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using AdminHelper.ViewModel;
using BusyIndicator;
using System.Drawing;
using System.Windows.Controls;
using System.Collections.Generic;
using AdminHelper.lib;
using MenuItem = System.Windows.Controls.MenuItem;
using System.Windows.Data;
using AdminHelper.Model;
using System.ComponentModel;

namespace AdminHelper.View
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM mainWindowVM;
        private NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenu trayMenu;


        public MainWindow()
        {
            if (IsAlreadyRunningApp())
            {
                Process.GetCurrentProcess().Kill();
            }

            mainWindowVM = new MainWindowVM();
            DataContext = mainWindowVM;

            InitializeComponent();
            Show();
            MakeTicketMenu();
            MakePrinterMenu();
            MakeTray();

            ComboBox_UserDealership.SelectionChanged += UpdatePrinterMenu;
            Hide();
        }

        private bool IsAlreadyRunningApp()
        {
            string assembly = System.Reflection.Assembly.GetEntryAssembly().Location;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assembly);
            return (Process.GetProcessesByName(fileName).Length > 1);
        }

        private MenuItem AddMenuItem(MenuItem parent, string item)
        {
            MenuItem menuItem = new MenuItem { Header = item };
            parent.Items.Add(menuItem);
            return menuItem;
        }

        #region Ticket Theme Menu
        private void MakeTicketMenu()
        {
            MenuItem rootMenuItem = TicketSelect_MenuItem_Main;
            HashSet<JsonTreeNode>.Enumerator nodes = mainWindowVM.TicketTemplatesJson.GetChildrenEnumerator();
            ComboBox_TicketTheme.SelectionChanged += UpdateTextBox;

            MakeTicketMenu(
                nodes,
                rootMenuItem,
                MenuItem_TicketThemeMenu_Click,
                MenuItem_TicketThemeMenu_DoubleClick
            );
        }

        public void MakeTicketMenu(
            HashSet<JsonTreeNode>.Enumerator nodes,
            MenuItem parentMenu,
            Action<object, RoutedEventArgs> click,
            Action<object, RoutedEventArgs> doubleClick = null)
        {
            while (nodes.MoveNext())
            {
                if (nodes.Current.IsALeaf())
                {
                    return;
                }
                MenuItem item = AddMenuItem(parentMenu, nodes.Current.GetData());
                item.Tag = Json.GetFullPath(nodes.Current);
                item.Click += new RoutedEventHandler(click);
                if (doubleClick != null)
                {
                    item.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(doubleClick);
                }

                if (nodes.Current.GetChildCount() > 0
                && !nodes.Current.AreChildrenLeaves())
                {
                    HashSet<JsonTreeNode>.Enumerator children = nodes.Current.GetChildrenEnumerator();
                    MakeTicketMenu(children, item, click, doubleClick);
                }
            }
        }

        private void UpdateTextBox(object sender, SelectionChangedEventArgs args)
        {
            if (ComboBox_TicketTheme.SelectedValue == null)
            {
                TextBox_TicketClaim.Text = "";
                return;
            }

            mainWindowVM.SelectedTicketTheme = ComboBox_TicketTheme.SelectedValue.ToString();
            mainWindowVM.TicketClaim = mainWindowVM.GetTicketClaimByTicketThemeIndex(ComboBox_TicketTheme.SelectedIndex);
            ComboBox_TicketTheme_Update();
        }

        public void ComboBox_TicketTheme_Update()
        {
            BindingExpression i = ComboBox_TicketTheme.GetBindingExpression(System.Windows.Controls.ComboBox.SelectedIndexProperty);
            i.UpdateSource();
        }

        public void MenuItem_TicketThemeMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = e.OriginalSource as MenuItem;
                string s = item.Tag.ToString();

                if (item != null)
                {
                    // select item in ComboBox
                    ComboBox_TicketTheme.SelectedIndex = mainWindowVM.GetTicketThemeIndex(s);
                    // put template in TextBox
                    mainWindowVM.SelectedTicketThemeIndex = mainWindowVM.GetTicketThemeIndex(s);
                }
                ComboBox_TicketTheme_Update();
            }
            catch (Exception ex)
            {
                AppException.Handle(ex);
            }
        }

        public void MenuItem_TicketThemeMenu_DoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem) sender;
                string s = item.Tag.ToString();

                if (item != null)
                {
                    // select item in ComboBox
                    ComboBox_TicketTheme.SelectedIndex = mainWindowVM.GetTicketThemeIndex(s);
                    // put template in TextBox
                    mainWindowVM.SelectedTicketThemeIndex = mainWindowVM.GetTicketThemeIndex(s);
                }
                ComboBox_TicketTheme_Update();
            }
            catch (Exception) { }
        }
        #endregion

        #region Printer Map Menu
        private void MakePrinterMenu()
        {
            
            MenuItem rootMenuItem = PrintersSelect_MenuItem_Main;
            HashSet<JsonTreeNode>.Enumerator nodes = mainWindowVM.PrintersMapJson.GetChildrenEnumerator();
            while (nodes.MoveNext())
            {
                if (nodes.Current.GetData().ToUpper() == mainWindowVM.UserDealershipName.ToUpper())
                {
                    MakePrinterMapMenu(
                    nodes.Current.GetChildrenEnumerator(),
                    rootMenuItem,
                    MenuItem_PrinterRoomMenu_Click);
                    return;
                }
            }
            PrintersSelect_MenuItem_Main.IsEnabled = false;
        }

        public void MakePrinterMapMenu(
            HashSet<JsonTreeNode>.Enumerator nodes,
            MenuItem parentMenu,
            Action<object, RoutedEventArgs> click)
        {
            while (nodes.MoveNext())
            {
                if (nodes.Current.IsALeaf())
                {
                    return;
                }

                MenuItem item = AddMenuItem(parentMenu, nodes.Current.GetData());

                if (nodes.Current.GetChildCount() > 0)
                {
                    if (nodes.Current.AreChildrenLeaves())
                    {
                        string[] printers = Json.CollectSiblingChildrenData(nodes.Current);
                        item.Tag = String.Join(" ", printers);
                        item.Click += new RoutedEventHandler(click);
                    }
                    else
                    {
                        HashSet<JsonTreeNode>.Enumerator children = nodes.Current.GetChildrenEnumerator();
                        MakePrinterMapMenu(children, item, click);
                    }
                }
            }
        }

        public void MenuItem_PrinterRoomMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.OriginalSource as MenuItem;
            string[] printers = item.Tag.ToString().Split(' ');
            LoadingAndMessaging<string[]>(_MenuItem_PrinterRoomMenu_Click, printers);
        }

        public async Task<string> _MenuItem_PrinterRoomMenu_Click(string[] printers)
        {
            try
            {
                foreach (string printer in printers)
                {
                    mainWindowVM.NewPrinterNumber = printer;
                    await mainWindowVM.ConnectPrinter(mainWindowVM.PrinterLink);
                }
                return $"Принтеры с номерами:" +
                    $"\n{String.Join(",", printers)}" +
                    $"\nподключены!";
            }
            catch (Exception ex)
            {
                return AppException.Handle(ex);
            }
        }

        public void UpdatePrinterMenu(object sender, RoutedEventArgs e)
        {
            try
            {
                mainWindowVM.NewPrinterNumber = "";
                PrinterNumber_TextBox.Text = "";
                MenuItem rootMenuItem = PrintersSelect_MenuItem_Main;
                rootMenuItem.Items.Clear();
                MakePrinterMenu();
                if (rootMenuItem.HasItems)
                {
                    rootMenuItem.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                AppException.Handle("Printers map menu updating is failed", ex);
            }
        }
        #endregion

        #region Tray
        /// <summary>
        /// Tray
        /// </summary>
        private void MakeTray()
        {
            trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.ResourceManager.GetObject("ICON") as Icon,
                Visible = true,
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = "Помощник пользователя",
            };

            trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(Notifyicon_DoublecClick);
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.BalloonTipTitle = "Помощник пользователя";
            trayIcon.ShowBalloonTip(500, "Помощник пользователя", "Автоматизация Service Desk \n it Рольф", ToolTipIcon.Info);

            trayMenu = new System.Windows.Forms.ContextMenu();

            trayMenu.MenuItems.Add("Имя и IP", (object sender, EventArgs e) => Button_ShowComputerNameAndIP_Click(sender, null));
            trayMenu.MenuItems.Add("ARMS fix", (object sender, EventArgs e) => Button_Fix_ARMS_Click(sender, null));
            trayMenu.MenuItems.Add("Диск Б", (object sender, EventArgs e) => Button_BDrive_Click(sender, null));
            trayMenu.MenuItems.Add("Скопировать рабочий стол", (object sender, EventArgs e) => Button_CopyDesktop_Click(sender, null));
            trayMenu.MenuItems.Add("Почистить временные файлы", (object sender, EventArgs e) => Button_ClearTemps_Click(sender, null));
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Быстрая помощь", (object sender, EventArgs e) => OpenFromTray(FastFix));
            trayMenu.MenuItems.Add("Заявка", (object sender, EventArgs e) => OpenFromTray(MakeATicket));
            trayMenu.MenuItems.Add("Принтеры", (object sender, EventArgs e) => OpenFromTray(Printers));
            trayMenu.MenuItems.Add("Инструкции", (object sender, EventArgs e) => OpenFromTray(Instructions));
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Выход", (object sender, EventArgs e) => System.Windows.Application.Current.Shutdown());

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void OpenFromTray(TabItem tab)
        {
            Show();
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            mainWindow_TabControl.SelectedItem = tab;
        }

        private void Notifyicon_DoublecClick(object sender, EventArgs e)
        {
            OpenFromTray(FastFix);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                trayIcon.ShowBalloonTip(500, "Помощник пользователя", "Приложение свёрнуто в трэй", ToolTipIcon.Info);
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            trayIcon.ShowBalloonTip(500, "Помощник пользователя", "Приложение свёрнуто в трэй\nВыход здесь", ToolTipIcon.Info);
            //base.OnClosing(e);
        }
        #endregion

        #region Blocking Loading
        private void ShowLoading(string loadingMessage = "Выполнение...")
        {
            BusyIndicator.BusyContent = loadingMessage;
            BusyIndicator.IsBusy = true;
        }

        private void ShowRandomLoading(string loadingMessage = "Выполнение...")
        {
            Array values = Enum.GetValues(typeof(IndicatorType));
            Random random = new Random();
            BusyIndicator.IndicatorType = (IndicatorType) values.GetValue(random.Next(values.Length));
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

        private async void LoadingAndMessaging(Func<Task<string>> btn_hundler_VM)
        {
            ShowRandomLoading();
            string message = await btn_hundler_VM();
            HideLoading();
            BlockWindow();
            new NonModalMessage(UnblockWindow, message) { Owner = this }.Show();
        }

        private async void LoadingAndMessaging<T>(Func<T, Task<string>> btn_hundler_VM, T arg)
        {
            ShowRandomLoading();
            string message = await btn_hundler_VM(arg);
            HideLoading();
            BlockWindow();
            new NonModalMessage(UnblockWindow, message) { Owner = this }.Show();
        }

        private void AcceptedLoadingAndMessaging(Func<string> get_question, Func<Task<string>> btn_hundler)
        {
            BlockWindow();
            new NonModalConfirm(
                UnblockWindow,
                () => LoadingAndMessaging(btn_hundler),
                get_question())
            { Owner = this }.Show();
        }
        #endregion

        #region Buttons
        private void Button_CopyDesktop_Click(object sender, RoutedEventArgs e)
        {
            BlockWindow();
            new CopyWorkspace(UnblockWindow) { Owner = this }.Show();
        }

        private void Button_MakeATicket_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.MakeATicket);
            ComboBox_TicketTheme.SelectedIndex = -1;
        }

        private void Button_ConnectPrinter_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging<string>(mainWindowVM.ConnectPrinter, PrinterLink.Text);
        }

        private void Button_RedirectToConnectPrinter_Click(object sender, RoutedEventArgs e)
        {
            mainWindow_TabControl.SelectedItem = Printers;
        }

        private void Button_BDrive_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.BDrive);
        }

        private void Button_SDContacts_Click(object sender, RoutedEventArgs e)
        {
            BlockWindow();
            string message = mainWindowVM.ServiceDeskContacts;
            new NonModalMessage(
                UnblockWindow,
                message,
                "Контакты Сервис Деск")
            {
                Owner = this
            }.Show();
        }

        private void Button_Fix_ARMS_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.FixARMS);
        }

        // Ticket 1740008
        private void Button_ACRolf_DisplayOfGoodsMovementFix_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.Fix_ACRolf_DisplayOfGoodsMovement);
        }
        private void Button_DelChromeCache_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.RemoveChromeCache);
        }

        private void Button_InstallPuntoSwitcher_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.InstallPuntoSwitcher);
        }

        private void Button_ShowComputerNameAndIP_Click(object sender, RoutedEventArgs e)
        {
            BlockWindow();
            string message = mainWindowVM.ComputerNameAndIp;
            new NonModalMessage(
                UnblockWindow,
                message,
                "Имя компьютера и IP-адрес")
            { Owner = this }.Show();
        }

        private void Button_UniplanSquaresFix_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.UniplanSquaresFix);
        }

        private void Button_ClearTemps_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.ClearTemps);
        }

        private void Button_Install7zip19_Click(object sender, RoutedEventArgs e)
        {
            LoadingAndMessaging(mainWindowVM.Install7zip19);
        }

        private void Button_SetAsDefaultPrinter_Click(object sender, RoutedEventArgs e)
        {
            // get clicked printer name
            System.Windows.Controls.Button button = e.OriginalSource as System.Windows.Controls.Button;
            LocalPrinter context = (LocalPrinter) button.DataContext;
            string printerName = context.PrinterName;

            LoadingAndMessaging<string>(mainWindowVM.SetDefaultPrinter, printerName);
        }

        private async void Button_RefreshPrintersList_Click(object sender, RoutedEventArgs e)
        {
            await mainWindowVM.RefreshPrintersList();
        }

        // TODO
        private void Button_DirectlyConnectPrinter_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

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