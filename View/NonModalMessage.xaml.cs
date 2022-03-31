using System;
using System.Windows;
using AdminHelper.ViewModel;


namespace AdminHelper.View
{
    public partial class NonModalMessage : Window
    {
        private readonly NonModalMessageVM nonModalMessageVM;
        private readonly Action UnblockMainWindow;

        public NonModalMessage(Action unblockMainWindow, string message, string title = "")
        {
            UnblockMainWindow = unblockMainWindow;
            InitializeComponent();
            nonModalMessageVM = new NonModalMessageVM(message, title);
            NonModalMessageText.Text = nonModalMessageVM.Message;
            NonModalMessageWindow.Title = nonModalMessageVM.Title;
        }
        public NonModalMessage(string message, string title = "")
        {
            InitializeComponent();
            nonModalMessageVM = new NonModalMessageVM(message, title);
            NonModalMessageText.Text = nonModalMessageVM.Message;
            NonModalMessageWindow.Title = nonModalMessageVM.Title;
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
