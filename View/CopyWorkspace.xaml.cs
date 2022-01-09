using System.Windows;
using AdminHelper.ViewModel;


namespace AdminHelper.View
{
    public partial class CopyWorkspace : Window
    {
        private readonly CopyWorkspaceVM copyWorkspaceVM;

        public CopyWorkspace()
        {
            InitializeComponent();
            copyWorkspaceVM = new CopyWorkspaceVM();
            GroupBoxHeaderLabelButton.Content = $"Пользователь: {copyWorkspaceVM.UserName}";
            DataContext = copyWorkspaceVM;
        }

        private async void Button_Click_CopyDesktop(object sender, RoutedEventArgs e)
        {
            await copyWorkspaceVM.CopyUserWorkspace();
            Close();
        }

        private void GroupBoxHeaderLabelButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: user changing
        }
    }
}
