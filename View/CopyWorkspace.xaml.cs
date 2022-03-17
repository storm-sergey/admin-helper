using System;
using System.Threading.Tasks;
using System.Windows;
using AdminHelper.ViewModel;

namespace AdminHelper.View
{
    public partial class CopyWorkspace : Window
    {
        private readonly CopyWorkspaceVM copyWorkspaceVM;
        private readonly Action UnblockMainWindow;

        public CopyWorkspace(Action unblockMainWindow)
        {
            UnblockMainWindow = unblockMainWindow;
            InitializeComponent();
            copyWorkspaceVM = new CopyWorkspaceVM();
            GroupBoxHeaderLabelButton.Content = $"Пользователь: {copyWorkspaceVM.UserName}";
            DataContext = copyWorkspaceVM;
        }

        private void CopyWorkspace_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnblockMainWindow();
        }

        private void Button_Click_CopyDesktop(object sender, RoutedEventArgs e)
        {
            // await copyWorkspaceVM.CopyUserWorkspace();
            Task.Run(() => copyWorkspaceVM.CopyUserWorkspace());
            Close();
        }

        private void GroupBoxHeaderLabelButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: user changing
        }


    }
}
