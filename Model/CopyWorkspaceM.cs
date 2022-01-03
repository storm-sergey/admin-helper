using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminHelper.lib;
using static AdminHelper.Globals;

namespace AdminHelper.Model
{
    public class CopyWorkspaceM
    {
        public string UserName;
        public string Domain;
        public string ComputerNameFrom;
        public string ComputerNameTo;
        public List<string> failed;
        private readonly string[][] userWorkspace;

        public CopyWorkspaceM()
        {
            Domain = DOMAIN;
            UserName = UserCredentials.RuningAs;
            failed = new List<string>();
            userWorkspace = new string[5][];
            userWorkspace[0] = new string[2] { "Desktop", "Рабочий стол" };
            userWorkspace[1] = new string[2] { "Documents", "Документы" };
            userWorkspace[2] = new string[2] { "Favorites", "Избранное" };
            userWorkspace[3] = new string[2] { "Downloads", "Загрузки" };
            userWorkspace[4] = new string[2] { "Pictures", "Изображение" };
        }

        public async Task CopyUserWorkspace()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                foreach (string[] userFolder in userWorkspace)
                {
                    foreach (string folderNameVar in userFolder)
                    {
                        if (Files.CheckUserDirectoryExists(folderNameVar, UserName, ComputerNameFrom))
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                Files.CopyUserDirectory(folderNameVar, UserName, ComputerNameFrom, ComputerNameTo);
                            }));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
