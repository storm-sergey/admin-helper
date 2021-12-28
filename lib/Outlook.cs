using Microsoft.Office.Interop.Outlook;
using OutlookApp = Microsoft.Office.Interop.Outlook.Application;

namespace AdminHelper.lib
{
    public static class Outlook
    {
        public static void ImportPST(string path, string fileName)
        {
            try
            {
                OutlookApp outlookApp = new OutlookApp();
                NameSpace nameSpace = outlookApp.GetNamespace("MAPI");
                nameSpace.AddStore(path + fileName);
            }
            catch
            {
                throw new System.Exception("Outlook archive importing is failed");
            }
        }

        public static void SendEmail(string subject, string htmlBody, string to)
        {
            try
            {
                OutlookApp outlookApp = new OutlookApp();
                MailItem newMail = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);
                newMail.To = to;
                newMail.Subject = subject;
                newMail.BodyFormat = OlBodyFormat.olFormatHTML;
                newMail.HTMLBody = htmlBody;
                newMail.Importance = OlImportance.olImportanceHigh;
                newMail.Send();
            }
            catch
            {
                throw new System.Exception("Outlook email sending is failed");
            }
        }
    }
}