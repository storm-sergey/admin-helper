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
        private readonly string[] paths = {
            "Desktop",
            "Documents",
            "Favorites",
            "Downloads",
            "Pictures",
        };

        public CopyWorkspaceM()
        {
            Domain = DOMAIN;
            UserName = UserCredentials.UserName;
            failed = new List<string>();
        }

        public async Task CopyUserWorkspace()
        {
            List<Task> tasks = new List<Task>();
            foreach (string path in paths)
            {
                tasks.Add(Task.Run(() => {
                    try
                    {
                        Files.CopyUserDirectory(path, UserName, ComputerNameFrom, ComputerNameTo);
                    }
                    catch (Exception)
                    {
                        failed.Add(path);
                    }
                }));
            }
            await Task.WhenAll(tasks);
        }
    }
}
