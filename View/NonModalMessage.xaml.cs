using System;
using System.Threading.Tasks;
using System.Windows;
using AdminHelper.ViewModel;


namespace AdminHelper.View
{
    public partial class NonModalMessage : Window
    {
        private readonly NonModalMessageVM NonModalMessageVM;
        private readonly Action UnblockMainWindow;

        public NonModalMessage(Action unblockMainWindow, string message, string title = "")
        {
            this.UnblockMainWindow = unblockMainWindow;
            InitializeComponent();
            NonModalMessageVM = new NonModalMessageVM(message, title);
            NonModalMessageText.Text = NonModalMessageVM.Message;
            NonModalMessageWindow.Title = NonModalMessageVM.Title;
        }

        private void NonModalMessage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnblockMainWindow();
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
