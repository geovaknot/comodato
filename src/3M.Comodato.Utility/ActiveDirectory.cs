using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace _3M.Comodato.Utility
{
    public class ActiveDirectory
    {
        public class ADUser
        {
            //#region constants

            ///// <summary>
            ///// Property name of sAM account name.
            ///// </summary>
            //public const string SamAccountNameProperty = "sAMAccountName";

            ///// <summary>
            ///// Property name of canonical name.
            ///// </summary>
            //public const string CanonicalNameProperty = "CN";

            //#endregion

            //#region Properties

            ///// <summary>
            ///// Gets or sets the canonical name of the user.
            ///// </summary>
            //public string CN { get; set; }

            ///// <summary>
            ///// Gets or sets the sAM account name
            ///// </summary>
            //public string SamAcountName { get; set; }

            //#endregion

            ///// <summary>
            ///// Gets all users of a given domain.
            ///// </summary>
            ///// <param name="domain">Domain to query. Should be given in the form ldap://domain.com/ </param>
            ///// <returns>A list of users.</returns>
            //public static List<ADUser> GetUsers(string domain)
            //{
            //    List<ADUser> users = new List<ADUser>();

            //    using (DirectoryEntry searchRoot = new DirectoryEntry(domain))
            //    using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot))
            //    {
            //        // Set the filter
            //        directorySearcher.Filter = "(&(objectCategory=person)(objectClass=user))";

            //        // Set the properties to load.
            //        directorySearcher.PropertiesToLoad.Add(CanonicalNameProperty);
            //        directorySearcher.PropertiesToLoad.Add(SamAccountNameProperty);

            //        using (SearchResultCollection searchResultCollection = directorySearcher.FindAll())
            //        {
            //            foreach (SearchResult searchResult in searchResultCollection)
            //            {
            //                // Create new ADUser instance
            //                var user = new ADUser();

            //                // Set CN if available.
            //                if (searchResult.Properties[CanonicalNameProperty].Count > 0)
            //                    user.CN = searchResult.Properties[CanonicalNameProperty][0].ToString();

            //                // Set sAMAccountName if available
            //                if (searchResult.Properties[SamAccountNameProperty].Count > 0)
            //                    user.SamAcountName = searchResult.Properties[SamAccountNameProperty][0].ToString();

            //                // Add user to users list.
            //                users.Add(user);
            //            }
            //        }
            //    }

            //    // Return all found users.
            //    return users;
            //}

            private string ADService = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADService); 
            private string ADDomain = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADDomain); 
            private string aDUser = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADUser); 
            private string ADPassword = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADPassword); 

            public List<ADUsersEntity> ObterListaTodosUsuariosAD()
            {
                List<ADUsersEntity> lstReturn = new List<ADUsersEntity>();

                DirectoryEntry directoryEntry = null;

                if (string.IsNullOrEmpty(aDUser))
                    directoryEntry = new DirectoryEntry(ADService);
                else
                    directoryEntry = new DirectoryEntry(ADService, string.Concat(ADDomain, "\\", aDUser), ADPassword);

                DirectorySearcher searcher = new DirectorySearcher(directoryEntry);
                searcher.Filter = "(&(objectClass=user)(sn=*))";
                searcher.PropertiesToLoad.Add("displayName");
                searcher.PropertiesToLoad.Add("SAMAccountName");
                searcher.PropertiesToLoad.Add("mail");

                searcher.PageSize = 500;

                foreach (SearchResult searchResult in searcher.FindAll())
                {
                    ADUsersEntity adUsersEntity = new ADUsersEntity();

                    foreach (object displayName in searchResult.Properties["displayName"])
                    {
                        adUsersEntity.cdsNome = displayName.ToString();
                    }

                    foreach (object SAMAccountName in searchResult.Properties["SAMAccountName"])
                    {
                        adUsersEntity.cdsLogin = SAMAccountName.ToString().ToLower();
                    }

                    foreach (object displayName in searchResult.Properties["mail"])
                    {
                        adUsersEntity.cdsEmail = displayName.ToString();
                    }
                }

                return lstReturn;
            }

            public List<ADUsersEntity> ObterListaTodosUsuariosADPorLogin()
            {
                List<ADUsersEntity> lstReturn = new List<ADUsersEntity>();
                
                lstReturn = ObterListaTodosUsuariosAD().OrderBy(o => o.cdsLogin).ToList();

                return lstReturn;
            }

            public List<ADUsersEntity> ObterListaTodosUsuariosADPorNome()
            {
                List<ADUsersEntity> lstReturn = new List<ADUsersEntity>();

                lstReturn = ObterListaTodosUsuariosAD().OrderBy(o => o.cdsNome).ToList();

                return lstReturn;
            }
        }

        public class ADUsersEntity
        {
            public String cdsLogin { get; set; }
            public String cdsNome { get; set; }
            public String cdsEmail { get; set; }
        }

    }
}
