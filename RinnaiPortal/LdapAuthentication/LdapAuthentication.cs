using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Web;

namespace RinnaiPortal.LdapAuthentication
{
    public class LdapAuthentication
    {
        private string _path;
        private string _filterAttribute;

        public LdapAuthentication(string path)
        {
            _path = path;
        }

        /// <summary>
        /// 傳入從使用者收集的認證
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);
            //若帳號密碼無效entry之所屬成員皆都不會建立實體。get則會拋出exception
            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user. " + ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 取得使用者所屬的安全性與通訊群組清單
        /// 此方法會傳回由管道 (|) 分隔的群組清單。請注意，LdapAuthentication.GetGroups 方法會管理並截斷字串。這樣可縮短儲存在驗證 Cookie 中的字串長度。若未截斷字串，每個群組的格式看起來像下面這樣：
        /// CN=...,...,DC=domain,DC=com
        /// LdapAuthentication.GetGroups 方法可以傳回超長字串。若此字串的長度超過 Cookie 長度，可能無法建立驗證 Cookie。若此字串可能超過 Cookie 長度，您可能想將群組資訊儲存在 ASP.NET Cache 物件或資料庫中。此外，您也可以加密群組資訊並將此資訊儲存在隱藏的表單欄位中。
        /// </summary>
        /// <returns></returns>
        public string GetGroups()
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return groupNames.ToString();
        }
    }
}