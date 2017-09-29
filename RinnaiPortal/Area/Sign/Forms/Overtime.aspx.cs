using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign.Forms
{
	public partial class Overtime : System.Web.UI.Page
	{
		private OvertimeRepository _overtimeRepo = null;
        private RootRepository _rootRepo = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "Overtime"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				//進入新增頁面
				_overtimeRepo = RepositoryFactory.CreateOvertimeRepo();
				_rootRepo = RepositoryFactory.CreateRootRepo();

				//將部門資料 與下拉式選單綁定
				ViewUtils.SetOptions(DefaultDeptName, _rootRepo.GetDepartment());
				//將支援部門資料 與下拉式選單綁定
                ViewUtils.SetOptions(DefaultSupportDeptName, _rootRepo.GetDepartment());
                //20161117 修改抓成本部門
                //ViewUtils.SetOptions(DefaultSupportDeptName, _rootRepo.GetCostDepartment());
				//將訂餐資料 與下拉式選單綁定
				ViewUtils.SetOptions(DefaultMealOrderValue, _overtimeRepo.CreateMealOrderType());
				//將報酬資料 與下拉式選單綁定
				ViewUtils.SetOptions(DefaultPayTypeValue, _overtimeRepo.CreatePayType());

				var employeeData = _rootRepo.QueryForEmployeeByADAccount(User.Identity.Name);
				ApplyID_FK.Value = employeeData != null ? employeeData["EmployeeID"].ToString() : String.Empty;
				ApplyName.Text = employeeData != null ? employeeData["EmployeeName"].ToString() : String.Empty;
                
				var deptData = _rootRepo.QueryForDepartmentByDeptID(employeeData != null ? employeeData["departmentID_FK"].ToString() : String.Empty);
				ApplyDeptID_FK.Value = deptData != null ? deptData["DepartmentID"].ToString() : String.Empty;
				ApplyDeptName.Text = deptData != null ? deptData["DepartmentName"].ToString() : String.Empty;
				DefaultDeptName.SelectedValue = !String.IsNullOrWhiteSpace(ApplyDeptID_FK.Value) ? ApplyDeptID_FK.Value : String.Empty;
                //20161117 支援單位預設值抓成本部門(已取消此做法)
				DefaultSupportDeptName.SelectedValue = !String.IsNullOrWhiteSpace(ApplyDeptID_FK.Value) ? ApplyDeptID_FK.Value : String.Empty;
                //DefaultSupportDeptName.SelectedValue = employeeData != null ? employeeData["CostDepartmentID"].ToString() : String.Empty;
                if (employeeData["CostDepartmentID"].ToString().Substring(0, 2) == "39")
                {
                    DefaultMealOrderValue.SelectedValue = "none";
                }
                else
                {
                    DefaultMealOrderValue.SelectedValue = "Carnivore";
                }
				DefaultPayTypeValue.SelectedValue = "overtimePay";
				DefaultStartDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd 17:20:00");
				DefaultEndDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd 19:20:00");
				PageTitle.Value = "表單新增作業 > 加班單";
				FormSeries.Value = "Overtime";
                DefaultSupportDeptName_SelectedIndexChanged(sender,e);
				string signDocID = String.IsNullOrEmpty(Request["SignDocID_FK"]) ? String.Empty : Request["SignDocID_FK"].ToString();
				if (!String.IsNullOrWhiteSpace(signDocID))
				{
					PageTitle.Value = "表單編輯作業 > 加班單";
					SignDocID.Value = signDocID;
				}

			}
		}

        protected void DefaultSupportDeptName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //20161121 add尋找支援部門實際對應的成本部門
            _rootRepo = RepositoryFactory.CreateRootRepo();
            var DeptDataLevel = _rootRepo.QueryForDepartmentByDeptID(DefaultSupportDeptName.Text);
            string Level = DeptDataLevel != null ? DeptDataLevel["DepartmentLevel"].ToString() : String.Empty;
            string TrueSupportDept = "";
            if (DeptDataLevel["Virtual"].ToString() == "N")
            {
                TrueSupportDept = DeptDataLevel["DepartmentID"].ToString();
            }
            else
            {
                string UpDept = "";
                for (int i = Int32.Parse(Level) - 1; i >= 1; i--)
                {
                    if (UpDept=="")
                    {
                        var DeptData = _rootRepo.QueryForDepartmentByDeptID(DeptDataLevel["UpperDepartmentID"].ToString());
                        if (DeptData["Virtual"].ToString() == "N")
                        {
                            TrueSupportDept = DeptData["DepartmentID"].ToString();
                            break;
                        }
                        else
                        {
                            UpDept = DeptData["UpperDepartmentID"].ToString();
                        }
                       
                    }
                    else
                    {
                        var DeptData = _rootRepo.QueryForDepartmentByDeptID(UpDept);
                        if (DeptData["Virtual"].ToString() == "N")
                        {
                            TrueSupportDept = DeptData["DepartmentID"].ToString();
                            break;
                        }
                        else
                        {
                            UpDept = DeptData["UpperDepartmentID"].ToString();
                        }
                    }                   
                }//迴圈END
            }

            TrueSupportDeptName.Value = TrueSupportDept;
        }

	}
}