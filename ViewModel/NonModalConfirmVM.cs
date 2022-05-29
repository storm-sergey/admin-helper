using AdminHelper.Model;

namespace AdminHelper.ViewModel
{
    public class NonModalConfirmVM : ViewModelBase
    {
        private readonly NonModalConfirmM nonModalConfirmM;

        public NonModalConfirmVM(string question, string title = "")
        {
            nonModalConfirmM = new NonModalConfirmM(question, title);
        }

        public string Title { get => nonModalConfirmM.Title; }
        public string Question { get => nonModalConfirmM.Question; }
        public bool IsAccepted
        {
            get => nonModalConfirmM.IsAccepted;
        }

        public void Accept()
        {
            nonModalConfirmM.Accept();
        }

        public void Cancel()
        {
            nonModalConfirmM.Cancel();
        }
    }
}
