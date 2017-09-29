using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Manage;
using RinnaiPortal.Tools;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Manage
{
    public class ManageDataList : System.Web.UI.Page
    {
        public virtual void ConstructPage(ManageListParms mlParm, PaggerParms pParms, IManageRepository repo)
        {
            var pagination = repo.GetPagination(mlParm, pParms);

            if (pagination == null) { return; }
            if (0 == pagination.TotalItems) { mlParm.NoDataTip.Visible = true; mlParm.NoDataTip.Text = "查無資料"; }

            //設定 gridView Source
            ViewUtils.SetGridView(mlParm.GridView, pagination.Data);
            mlParm.TotalRowsCount.Text = pagination.TotalItems.ToString();

            //Pagination Bar Generator
            string paginationHtml = WebUtils.GetPagerNumericString(pagination, Request);
            mlParm.PaginationBar.InnerHtml = paginationHtml;

        }

        public virtual void QueryBtn_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            string query = String.Empty;
            var condition = new Dictionary<string, string>()
            {
                {"StartDateTime",((TextBox)btn.Parent.FindControl("StartDateTime")).Text},
                {"EndDateTime", ((TextBox)btn.Parent.FindControl("EndDateTime")).Text}
            };
            query = WebUtils.ConstructQuerytString(condition, Request);
            //查詢後 pageIndex 必為 1
            query = Regex.Replace(query, @"pageIndex=\d+", "pageIndex=1");
            string url = Request.Url.LocalPath;
            Response.Redirect(String.Concat(url, query));
        }

        protected void PageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pageSize = sender as DropDownList;
            if (pageSize == null) { Response.Write("頁面上並沒有提供換頁元件!".ToAlertFormat()); return; }

            string queryString = WebUtils.ConstructQueryString("pageSize", pageSize.SelectedValue, Request);
            //變更 pageSize 後 pageIndex 必為 1
            queryString = Regex.Replace(queryString, @"pageIndex=\d+", "pageIndex=1");
            string url = Request.Url.LocalPath;
            Response.Redirect(String.Concat(url, queryString));
        }

    }
}