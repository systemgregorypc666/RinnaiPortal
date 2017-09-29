using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace RinnaiPortal
{
    public class DefaultDataList : System.Web.UI.Page
    {
        public virtual void ConstructPage(SignListParms slParms, PaggerParms pParms, ISignRepository repo)
        {
            var pagination = repo.GetPagination(slParms, pParms);

            if (pagination == null) {  return; }
            if (0 == pagination.TotalItems) { slParms.NoDataTip.Visible = true; slParms.NoDataTip.Text = "查無資料"; }

            //設定 gridView Source
            ViewUtils.SetGridView(slParms.GridView, pagination.Data);
            slParms.TotalRowsCount.Text = pagination.TotalItems.ToString();

            //Pagination Bar Generator
            string paginationHtml = WebUtils.GetPagerNumericString(pagination, Request);
            slParms.PaginationBar.InnerHtml = paginationHtml;

        }

        public virtual void ConstructPage_Daily(SignListParms slParms, PaggerParms pParms, ISignRepository repo)
        {
            var pagination_Daily = repo.GetPagination(slParms, pParms);

            if (pagination_Daily == null) { return; }
            if (0 == pagination_Daily.TotalItems) { slParms.NoDataTip.Visible = true; slParms.NoDataTip.Text = "查無資料"; }

            //設定 gridView Source
            ViewUtils.SetGridView(slParms.GridView, pagination_Daily.Data);
            slParms.TotalRowsCount.Text = pagination_Daily.TotalItems.ToString();

            //Pagination Bar Generator
            string paginationHtml_Daily = WebUtils.GetDailyPagerNumericString(pagination_Daily, Request);
            slParms.PaginationBar_Daily.InnerHtml = paginationHtml_Daily;

        }
        public virtual void QueryBtn_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            string queryString = WebUtils.ConstructQueryString("queryText", ((TextBox)btn.Parent.FindControl("queryTextBox")).Text, Request);
            //查詢後 pageIndex 必為 1
            queryString = Regex.Replace(queryString, @"pageIndex=\d+", "pageIndex=1");
            string url = Request.Url.LocalPath;
            Response.Redirect(String.Concat(url, queryString));
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