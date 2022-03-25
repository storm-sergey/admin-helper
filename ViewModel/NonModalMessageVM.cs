using AdminHelper.Model;

namespace AdminHelper.ViewModel
{
    public class NonModalMessageVM : ViewModelBase
    {
        private readonly NonModalMessageM nonModalMessageM;

        public NonModalMessageVM(string message, string title = "")
        {
            nonModalMessageM = new NonModalMessageM(message, title);
        }

        public string Title { get => nonModalMessageM.Title; }
        public string Message { get => nonModalMessageM.Message; }
    }
}
