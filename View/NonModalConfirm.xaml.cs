using System;
using System.Windows;
using AdminHelper.ViewModel;


namespace AdminHelper.View
{
    public partial class NonModalConfirm : Window
    {
        private readonly NonModalConfirmVM nonModalConfirmVM;
        private readonly Action UnblockMainWindow;
        private readonly Action AcceptHandler;

        public NonModalConfirm(Action unblockMainWindow, Action acceptHandler, string question)
        {
            UnblockMainWindow = unblockMainWindow;
            AcceptHandler = acceptHandler;
            InitializeComponent();
            nonModalConfirmVM = new NonModalConfirmVM(question);
            NonModalConfirmText.Text = nonModalConfirmVM.Question;
            NonModalConfirmWindow.Title = nonModalConfirmVM.Title;
        }

        private void NonModalMessage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnblockMainWindow();
            if (nonModalConfirmVM.IsAccepted)
            {
                AcceptHandler();
            }
        }

        private void Button_Click_Accept(object sender, RoutedEventArgs e)
        {
            nonModalConfirmVM.Accept();
            Close();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            nonModalConfirmVM.Cancel();
            Close();
        }
    }
}
