using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using System.Web.Routing;
using System.Web.Http;

namespace RinnaiPortal
{
    public class Global : HttpApplication
    {
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        void Application_Start(object sender, EventArgs e)
        {
                RouteTable.Routes.MapHttpRoute(
                name: "CustomAPi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            RouteTable.Routes.MapHttpRoute(
                  name: "DefaultApi",
                  routeTemplate: "api/{controller}/{id}",
                  defaults: new { id = RouteParameter.Optional }
              );

        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Context.Request.Cookies[cookieName];

            if (null == authCookie)
            {
                //There is no authentication cookie.
                return;
            }
            FormsAuthenticationTicket authTicket = null;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception ex)
            {
                //Write the exception to the Event Log.
                _log.Error(String.Concat("Authenticate Exception Message: \r\n", ex.Message, "\r\n"));
                return;
            }

            if (null == authTicket)
            {
                //Cookie failed to decrypt.
                return;
            }
            //When the ticket was created, the UserData property was assigned a
            //pipe-delimited string of group names.
            string[] groups = authTicket.UserData.Split(new char[] { '|' });
            //Create an Identity.
            GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");
            //This principal flows throughout the request.
            GenericPrincipal principal = new GenericPrincipal(id, groups);
            Context.User = principal;
        }
    }
}

