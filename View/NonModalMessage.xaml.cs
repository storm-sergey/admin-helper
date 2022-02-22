using System.Windows;
using AdminHelper.ViewModel;


namespace AdminHelper.View
{
    public partial class NonModalMessage : Window
    {
        private readonly NonModalMessageVM NonModalMessageVM;

        public NonModalMessage(string message, string title = "")
        {
            InitializeComponent();
            NonModalMessageVM = new NonModalMessageVM(message, title);
            NonModalMessageText.Text = NonModalMessageVM.Message;
            NonModalMessageWindow.Title = NonModalMessageVM.Title;
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
