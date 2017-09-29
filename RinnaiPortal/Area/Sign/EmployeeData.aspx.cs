using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using System;
using ValidationAPI;

namespace RinnaiPortal.Area.Sign
{
    public partial class EmployeeData : System.Web.UI.Page
    {
        private EmployeeViewModel model = null;
        private EmployeeRepository _employeeRepo = null;
        private RootRepository _rootRepo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //驗證身分權限
            if (!Authentication.HasResource(User.Identity.Name, "EmployeeData"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            //表單第一次載入
            if (!IsPostBack)
            {
                _employeeRepo = RepositoryFactory.CreateEmployeeRepo();
                _rootRepo = RepositoryFactory.CreateRootRepo();
                //從QueryString取得 員工編號
                string empID = String.IsNullOrEmpty(Request["EmployeeID"]) ? String.Empty : Request["EmployeeID"].ToString();

                //將部門資料 與下拉式選單綁定
                ViewUtils.SetOptions(DepartmentID_FK, _rootRepo.GetDepartment());
                //將成本部門資料 與下拉式選單綁定
                ViewUtils.SetOptions(CostDepartmentID, _rootRepo.GetCostDepartment());
                //代理人資料 與下拉式選單綁定
                ViewUtils.SetOptions(AgentName, _rootRepo.GetEmployee());
                //權限類別  與下拉式選單綁定
                ViewUtils.SetOptions(AccessType, _rootRepo.GetGroup());
                //國籍類別  與下拉式選單綁定
                ViewUtils.SetOptions(NationalType, _employeeRepo.GenarationNationalType());
                //性別類別  與下拉式選單綁定
                ViewUtils.SetOptions(SexType, _employeeRepo.GenarationSexType());

                PageTitle.Value = "員工基本資料 > 新增";

                if (!String.IsNullOrWhiteSpace(empID))
                {
                    //將 viewModel 的值綁定到 頁面上
                    WebUtils.PageDataBind(_employeeRepo.GetEmployeeDataByID(empID), this.Page);

                    EmployeeID.ReadOnly = true;
                    DepartmentName.Value = DepartmentID_FK.Text;
                    CostDepartmentName.Value = CostDepartmentID.Text;
                    PageTitle.Value = "員工基本資料 > 編輯";
                }
            }
        }

        public void SaveBtn_Click(object sender, EventArgs e)
        {
            _employeeRepo = RepositoryFactory.CreateEmployeeRepo();
            //取得頁面資料
            model = WebUtils.ViewModelMapping<EmployeeViewModel>(this.Page);

            ///驗證model定義的attribute
            var validator = new Validator();
            var validResult = validator.ValidateModel(model);
            if (!validResult.IsValid)
            {
                Response.Write(validResult.ErrorMessage.ToString().ToAlertFormat());
                return;
            }

            //btn處理 將buttonCss調換 (顯示/隱藏)
            ViewUtils.ButtonOff(SaveBtn, CoverBtn);

            //存檔
            var responseMessage = "";
            var successRdUrl = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(Request["EmployeeID"]))
                {
                    _employeeRepo.CreateData(model);
                    successRdUrl = @"EmployeeDataList.aspx?orderField=CreateDate&descending=True";
                    responseMessage = "新增成功!";
                }
                else
                {
                    _employeeRepo.EditData(model);
                    successRdUrl = @"EmployeeDataList.aspx?orderField=ModifyDate&descending=True";
                    responseMessage = "編輯成功!";
                }

                //btn處理
                ViewUtils.ButtonOn(SaveBtn, CoverBtn);
                responseMessage = responseMessage.ToAlertAndRedirect(successRdUrl);
            }
            catch (Exception ex)
            {
                responseMessage = String.Concat("存檔失敗!\r\n錯誤訊息: ", ex.Message).ToAlertFormat();
                ViewUtils.ShowRefreshBtn(CoverBtn, RefreshBtn);
            }
            finally
            {
                Response.Write(responseMessage);
            }
        }

        protected void DepartmentID_FK_SelectedIndexChanged(object sender, EventArgs e)
        {
            DepartmentName.Value = DepartmentID_FK.Text;
        }

        protected void CostDepartmentID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CostDepartmentName.Value = CostDepartmentID.Text;
        }

        protected void AgentName_SelectedIndexChanged(object sender, EventArgs e)
        {
            AgentID.Text = AgentName.SelectedValue.ToString();
        }

        protected void NationalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NationalType.Text = NationalType.SelectedValue.ToString();
        }

        protected void SexType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SexType.Text = SexType.SelectedValue.ToString();
        }

        protected void Disabled_CheckedChanged(object sender, EventArgs e)
        {
            DisabledDate.Text = Disabled.Checked ? DateTime.Now.FormatDatetime("yyyy-MM-dd HH:mm:00") : String.Empty;
        }
    }
}