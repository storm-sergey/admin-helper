using System;
using System.Threading.Tasks;

namespace AdminHelper.Model
{
    public class NonModalConfirmM
    {
        private bool AcceptanceHasBeenMade;
        private bool isAccepted;
        public string Title;
        public string Question;
        
        public NonModalConfirmM(string question, string title)
        {
            Question = question;
            Title = title;
            AcceptanceHasBeenMade = false;
            isAccepted = false;
        }

        public bool IsAccepted
        {
            get
            {
                if (AcceptanceHasBeenMade) return isAccepted;
                else throw new Exception("NonModalConfirmM has no been made accept but an acceptance is required");
            }
            set { isAccepted = value; }
        }

        public void Accept()
        {
            AcceptanceHasBeenMade = true;
            IsAccepted = true;
        }

        public void Cancel()
        {
            AcceptanceHasBeenMade = true;
            IsAccepted = false;
        }
    }
}
