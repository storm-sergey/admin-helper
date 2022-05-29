using System.Threading.Tasks;
using AdminHelper.Model;

namespace AdminHelper.ViewModel
{
    public class CopyWorkspaceVM : ViewModelBase
    {
        private readonly CopyWorkspaceM copyDesktopM;

        public CopyWorkspaceVM()
        {
            copyDesktopM = new CopyWorkspaceM();
        }

        public string Domain { get => copyDesktopM.Domain; }
        public string UserName
        {
            get => copyDesktopM.UserName;
            set
            {
                copyDesktopM.UserName = value;
                OnPropertyChange("UserName");
            }
        }

        public string ComputerNameFrom
        {
            get => copyDesktopM.ComputerNameFrom;
            set
            {
                copyDesktopM.ComputerNameFrom = value;
                OnPropertyChange("ComputerNameFrom");
            }
        }
        public string ComputerNameTo
        {
            get => copyDesktopM.ComputerNameTo;
            set
            {
                copyDesktopM.ComputerNameTo = value;
                OnPropertyChange("ComputerNameTo");
            }
        }
        public async Task CopyUserWorkspace()
        {
            await copyDesktopM.CopyUserWorkspace();
        }
        
    }
}
