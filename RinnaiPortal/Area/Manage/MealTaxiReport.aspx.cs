using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Manage;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;

namespace RinnaiPortal.Area.Manage
{
    public partial class MealTaxiReport : ManageDataList
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "MealTaxiReport"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                PageTitle.Value = "加班餐車資料 > 報表";

                // 取得 QueryString
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var manageListParms = WebUtils.ParseQueryString<ManageListParms>(Page.Request);
                manageListParms.GridView = ReportGridView;
                manageListParms.TotalRowsCount = totalRowsCount;
                manageListParms.PaginationBar = paginationBar;
                manageListParms.MealSummary = MealSummary;
                manageListParms.TaxiSummary = TaxiSummary;
                manageListParms.NoDataTip = noDataTip;

                //建構頁面
                ConstructPage(manageListParms, paggerParms, RepositoryFactory.CreateMealTaxiRepo());

                pageSizeSelect.Text = paggerParms.PageSize.ToString();
                StartDateTime.Text = manageListParms.StartDateTime.FormatDatetimeNullable();
                EndDateTime.Text = manageListParms.EndDateTime.FormatDatetimeNullable();

                MealSummaryTitle.InnerText = String.Concat(manageListParms.StartDateTime.FormatDatetimeNullable(), " ~ ", manageListParms.EndDateTime.FormatDatetimeNullable(), " 訂餐統計");
                TaxiSummaryTitle.InnerText = String.Concat(manageListParms.StartDateTime.FormatDatetimeNullable(), " ~ ", manageListParms.EndDateTime.FormatDatetimeNullable(), " 計程車統計");
            }
            else
            {
                //the second into page, IsPostBack == true
                //set ParentClassMember queryText = queryTextBox.Text
                //queryText = queryTextBox.Text;
            }
        }

        /// <summary>
        /// 建構頁面處理常式
        /// </summary>
        /// <param name="mlParm"></param>
        /// <param name="pParms"></param>
        /// <param name="repo"></param>
        public void ConstructPage(ManageListParms mlParm, PaggerParms pParms, MealTaxiRepository repo)
        {
            //分頁資訊(含資料) 資料型別DataTable
            var pagination = repo.GetPagination(mlParm, pParms);

            #region 0007 加班餐車資料報表依照加班單位代碼小排到大 by 小遇

            //重新排序
            if (pagination.Data != null)
            {
                IEnumerable<DataRow> dataRows = pagination.Data.Rows.Cast<DataRow>().OrderBy(row => row["supportdeptid_fk"]);
                pagination.Data = dataRows.CopyToDataTable();
            }

            #endregion 0007 加班餐車資料報表依照加班單位代碼小排到大 by 小遇

            if (pagination == null) { return; }
            if (0 == pagination.TotalItems) { mlParm.NoDataTip.Visible = true; mlParm.NoDataTip.Text = "查無資料"; }

            //設定 gridView Source 將分頁資訊裡的資料繫結到GridView
            ViewUtils.SetGridView(mlParm.GridView, pagination.Data);
            mlParm.TotalRowsCount.Text = pagination.TotalItems.ToString();

            //Pagination Bar Generator
            string paginationHtml = WebUtils.GetPagerNumericString(pagination, Request);
            mlParm.PaginationBar.InnerHtml = paginationHtml;

            //設定 Meal gridView Source
            ViewUtils.SetGridView(mlParm.MealSummary, repo.GetMealSummary(mlParm));

            //設定 Taxi gridView Source
            ViewUtils.SetGridView(mlParm.TaxiSummary, repo.GetTaxiSummary(mlParm));
        }
    }
}