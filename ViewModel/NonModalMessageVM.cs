using System.Threading.Tasks;
using AdminHelper.Model;

namespace AdminHelper.ViewModel
{
    public class NonModalMessageVM : ViewModelBase
    {
        private readonly NonModalMessageM NonModalMessageM;

        public NonModalMessageVM(string message, string title = "")
        {
            NonModalMessageM = new NonModalMessageM(message, title);
        }


        public string Title { get => NonModalMessageM.Title; }
        public string Message { get => NonModalMessageM.Message; }
    }
}
