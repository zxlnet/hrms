using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using ActiveDs;
using BdsSoft.DirectoryServices.Linq;
using GotWell.Model.Authorization;

namespace GotWell.Utility
{
    [DirectorySchema("user", typeof(IADsUser))]
    class User
    {
        public User()
        {
            Name = string.Empty;
            SamAccountName = string.Empty;
            DisplayName = string.Empty;
        }

        public string Name { get; set; }

        public string SamAccountName { get; set; }

        public string DisplayName { get; set; }
    }


    public class UtilDomain
    {
        public static string ADServerPath = string.Empty;

        private static DirectoryEntry root;

        public UtilDomain()
        {
            if (root == null)
            {
                root = new DirectoryEntry(ADServerPath);
            }
        }

        public List<SysUserMdl> GetAllUsers()
        {
            var users = new DirectorySource<User>(root, SearchScope.Subtree);

            var res1 = (from usr in users
                        select new SysUserMdl() { emid = usr.SamAccountName, urnm = usr.DisplayName, isck = false, urid = usr.SamAccountName });

            return res1.ToList<SysUserMdl>();
        }

        public List<SysUserMdl> GetAllUsersBut(List<string> butUsers)
        {
            var users = new DirectorySource<User>(root, SearchScope.Subtree);

            var res1 = (from usr in users
                        select new SysUserMdl() { emid = usr.SamAccountName, urnm = usr.DisplayName, isck = false, urid = usr.SamAccountName });
            return res1.ToList<SysUserMdl>().FindAll(delegate(SysUserMdl su) { return !butUsers.Contains(su.urid); });
        }

        public List<SysUserMdl> GetUsersBy(string userName)
        {
            var users = new DirectorySource<User>(root, SearchScope.Subtree);
            string queryParams = string.Empty;
            if (userName.Equals(string.Empty))
            {
                queryParams = userName + "*";
            }
            else
            {
                queryParams = "*" + userName + "*";
            }

            var res1 = (from usr in users
                        where usr.SamAccountName == queryParams || usr.DisplayName == queryParams
                        select new SysUserMdl() { emid = usr.SamAccountName, urnm = usr.DisplayName, isck = false, urid= usr.SamAccountName });

            return res1.ToList<SysUserMdl>();
        }

        public List<SysUserMdl> GetUsersBy(string userName, List<string> butUsers)
        {
            var users = new DirectorySource<User>(root, SearchScope.Subtree);
            string queryParams = string.Empty;
            if (userName.Equals(string.Empty))
            {
                queryParams = userName + "*";
            }
            else
            {
                queryParams = "*" + userName + "*";
            }

            var res1 = (from usr in users
                        where usr.SamAccountName == queryParams || usr.DisplayName == queryParams
                        select new SysUserMdl() { urid = usr.SamAccountName, urnm = usr.DisplayName, isck = false, emid = usr.SamAccountName });

            if (butUsers == null)
                return res1.ToList<SysUserMdl>();

            return res1.ToList<SysUserMdl>().FindAll(delegate(SysUserMdl su) { return !butUsers.Contains(su.urid); });
        }
    }
}
