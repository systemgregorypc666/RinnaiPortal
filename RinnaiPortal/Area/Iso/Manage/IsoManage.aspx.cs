using RinnaiPortal.Entities;
using RinnaiPortal.Models.IsoModels;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;

namespace RinnaiPortal.Area.Iso.Manage
{
    public partial class IsoManage : System.Web.UI.Page
    {
        private IsoRepository Repository = new IsoRepository();
        public List<IsoMain> m_bindData = new List<IsoMain>();
        public FilterCondition Filter = new FilterCondition();

        private IsoManageListViewModel m_viewModel = new IsoManageListViewModel();
        public IsoManageListViewModel IsoViewModel { get { return m_viewModel; } set { m_viewModel = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "IsoManage"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            IsoViewModel.Data = Repository.GetAllIsoAppListForManage();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var status = Request["isoStatus"];
            var level = Convert.ToInt16(Request["docLevel"]);
            IsoViewModel.Data.Clear();
            IsoViewModel.Data.AddRange(Repository.FilterManageList(qry.Text, status, level));
            Filter.Qry = qry.Text;
            Filter.Status = status;
            Filter.Level = level;
        }
    }

    public class FilterCondition
    {
        /// <summary>
        /// 關鍵字 文件號碼或申請人
        /// </summary>
        public string Qry { get; set; }
        /// <summary>
        /// 申請狀態
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 文件階級
        /// </summary>
        public int Level { get; set; }
    }
}