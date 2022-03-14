using System.Threading.Tasks;

namespace AdminHelper.Model
{
    public class NonModalMessageM
    {
        public string Title;
        public string Message;
        public Task<string> MessageTask;

        public NonModalMessageM(string message, string title)
        {
            Message = message;
            Title = title;
        }
    }
}
