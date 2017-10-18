using DBTools;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
namespace RinnaiPortal
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        private RinnaiPortalRepository _protalRepo = RepositoryFactory.CreateRinnaiPortalRepo();

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Menu systemMenu = (Menu)menuLoginView.FindControl("SystemMenu");

            if (systemMenu != null && Context.User.Identity.IsAuthenticated)
            {
                //產生 member instance
                var member = new MemberViewModel(Context.User.Identity.Name);

                #region #0016 新增-資訊人員Debug時佔用帳號 將往指導到user容易理解的頁面。

                //開發人員使用中
                if (member.NationalType == "TEST")
                {
                    DB db = ConnectionFactory.GetSmartManDC();
                    //從志元取得員工名字以及ID
                    string strSQL = @"select employecd,chinesename from EMPLOYEE where EMAILADDRESS = @EMAILADDRESS ";
                    var conditionsDic = new Conditions()
			            {
				            { "@EMAILADDRESS",member.ADAccount+"@rinnai.com.tw" },
			            };

                    DataRow result = db.QueryForDataRow(strSQL, conditionsDic);
                    string smartManEmpCd = result["employecd"].ToString();
                    string smartManEmpName = result["chinesename"].ToString();

                    if (!member.EmployeeID.Equals(smartManEmpCd))
                    {
                        //進此敘述代表ADAccount已被改變，於資訊人員偵錯中
                        RinnaiPortal.Tools.Authentication.LoginList.Remove(Context.User.Identity.Name);
                        Context.GetOwinContext().Authentication.SignOut();
                        Response.Redirect("~/AdminInDebug.aspx?empId=" + smartManEmpName);
                    }
                }

                #endregion #0016 新增-資訊人員Debug時佔用帳號 將往指導到user容易理解的頁面。

                // 將member 資料加入登入列表
                if (!Authentication.LoginList.ContainsKey(member.ADAccount))
                {
                    Authentication.LoginList.Add(member.ADAccount, member.ProfilePair);
                }

                if (member.ResourceList == null) { Response.Write("您沒有任何瀏覽權限!即將為您跳轉至首頁!".ToAlertFormat()); return; }

                DataTable dataSource = _protalRepo.GetMenuData(String.Format("'{0}'", String.Join("','", member.ResourceList)));

                var menuItemList = getMenuitems(0, dataSource);
                foreach (var menuItem in menuItemList)
                {
                    systemMenu.Items.Add(menuItem);
                }
            }
            //HttpCookie MyCookie = new HttpCookie("%5FAutoWebCookie%5FEIP%5FWEB%5FEIP");
            //MyCookie.Values["ERPID"] = "george.chang";

            //Response.Cookies.Add(MyCookie);
        }

        private List<MenuItem> getMenuitems(int parrentID, DataTable dataSource)
        {
            var itemsResult = new List<MenuItem>();
            string filter = "ParentId = " + parrentID;
            DataRow[] rows = dataSource.Select(filter);

            if (rows.Length == 0)
            {
                return itemsResult;
            }

            foreach (var row in rows)
            {
                int tmpNodeID = Int32.Parse(row["NodeID"].ToString());
                string tmpMenuTitle = row["NodeTitle"].ToString();
                if (tmpMenuTitle.Equals("ISO文件明細") || tmpMenuTitle.Equals("ISO文件管理明細"))
                    continue;
                string tmpMenuUri = tmpMenuTitle == "ISO文件申請" ? row["NodeUri"].ToString() + "?uId=" + HttpContext.Current.User.Identity.Name : row["NodeUri"].ToString();
                MenuItem newMenuItem = new MenuItem(tmpMenuTitle, String.Empty, String.Empty, tmpMenuUri, String.Empty);

                //看看當前的節點，是哪些節點的老爸，把那些人找出來，加到自己的下面當成子節點
                foreach (var menuItem in getMenuitems(tmpNodeID, dataSource))
                {
                    newMenuItem.ChildItems.Add(menuItem);
                }

                //上foreach 同下面兩種變形
                //(1)
                //getMenus(tmpNodeID, dataSource).ForEach(item =>
                //{
                //    newMenuItem.ChildItems.Add(item);
                //});
                //(2)
                //getMenus(tmpNodeID, dataSource).ForEach(newMenuItem.ChildItems.Add);

                itemsResult.Add(newMenuItem);
            }

            return itemsResult;
        }

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            Authentication.LoginList.Remove(Context.User.Identity.Name);
            Context.GetOwinContext().Authentication.SignOut();
        }
    }
}