using System.Threading.Tasks;
using AdminHelper.Model;

namespace AdminHelper.ViewModel
{
    public class NonModalMessageVM : ViewModelBase
    {
        private readonly NonModalMessageM NonModalMessageM;

        public NonModalMessageVM(string title, string message)
        {
            NonModalMessageM = new NonModalMessageM(title, message);
        }

        public string Title { get => NonModalMessageM.Title; }
        public string Message { get => NonModalMessageM.Message; }
    }
}
