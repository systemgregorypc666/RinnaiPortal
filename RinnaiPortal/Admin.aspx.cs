using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace RinnaiPortal
{
    /// <summary>
    /// 0018
    /// </summary>
    public partial class Admiiin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string portalConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString;

            Dictionary<string, string> connStringParts = portalConnectionString.Split(';')
                .Select(t => t.Split(new char[] { '=' }, 2))
                .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase);
            var getDatabaseName = connStringParts["Initial Catalog"];

            if (getDatabaseName == "RinnaiPortal")
                hostType.Text = "測試區";
            else
                hostType.Text = "正式區";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            GetEmpData();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
        }

        private EmployeeViewModel GetEmpDataByEmpID(string empID)
        {
            EmployeeRepository employeeRepository = RepositoryFactory.CreateEmployeeRepo();
            EmployeeViewModel empInfo = employeeRepository.GetEmployeeDataByID(empID);
            return empInfo;
        }

        public void GetEmpData()
        {
            try
            {
                string emp1Id = empID1ID.Text;
                string emp2Id = empID2ID.Text;
                //string testOrRelease = Request.Form["testOrRelease"].ToString();
                List<string> empIdList = new List<string>() { emp1Id, emp2Id };
                List<EmployeeViewModel> empDataList = new List<EmployeeViewModel>();
                //EmployeeRepository employeeRepository = RepositoryFactory.CreateEmployeeRepo();

                foreach (var empId in empIdList)
                {
                    //EmployeeViewModel empInfo = employeeRepository.GetEmployeeDataByID(empId);
                    EmployeeViewModel empInfo = GetEmpDataByEmpID(empId);
                    if (empInfo == null)
                        continue;
                    empDataList.Add(empInfo);
                }
                if (empDataList.Count == 0)
                    throw new Exception("未找到相關員工資訊。");

                string resultMsg = string.Empty;
                var emp1Data = empDataList.Where(o => o.EmployeeID == emp1Id).FirstOrDefault();
                var emp2Data = empDataList.Where(o => o.EmployeeID == emp2Id).FirstOrDefault();

                if (emp1Data == null)
                {
                    resultMsg = "emp1 查無相關資訊";
                }
                else
                {
                    empID1EmpId.Text = emp1Data.EmployeeID;
                    empID1ADAccount.Text = emp1Data.ADAccount;
                    empID1Name.Text = emp1Data.EmployeeName;
                    empID1Status.Text = emp1Data.NationalType != "TEST" ? "未設定" : "處理中";
                }
                if (emp2Data == null)
                {
                    if (string.IsNullOrEmpty(resultMsg))
                        resultMsg = "emp2 查無相關資訊";
                    else
                        resultMsg += " ， emp2 查無相關資訊";
                }
                else
                {
                    empID2EmpId.Text = emp2Data.EmployeeID;
                    empID2ADAccount.Text = emp2Data.ADAccount;
                    empID2Name.Text = emp2Data.EmployeeName;
                    empID2Status.Text = emp2Data.NationalType != "TEST" ? "未設定" : "處理中";
                }

                if (!string.IsNullOrEmpty(resultMsg))
                {
                    comment.Value = resultMsg;
                    return;
                }
                else
                    comment.Value = "查詢完成";
            }
            catch (Exception ex)
            {
                comment.Value = ex.Message;
            }
        }

        //up
        protected void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                ClientScriptManager cs = Page.ClientScript;
                bool isSetting = Convert.ToBoolean(Request["isSetting"]);
                string emp1Id = Request["empID1EmpId"].ToString();
                string emp2Id = Request["empID2EmpId"].ToString();

                EmployeeViewModel firstModel = this.GetEmpDataByEmpID(emp1Id);
                EmployeeViewModel updateModel = this.GetEmpDataByEmpID(emp2Id);

                if (firstModel == null || updateModel == null)
                {
                    cs.RegisterClientScriptBlock(this.GetType(), "PopupScript", "var isSuccess = false;var isSetting = " + isSetting.ToString().ToLower() + ";", true);
                    return;
                }
                string ad1 = firstModel.ADAccount;
                string ad2 = updateModel.ADAccount;
                firstModel.ADAccount = ad2;
                updateModel.ADAccount = ad1;

                var dc = ConnectionFactory.GetPortalDC();
                string NationalType = string.Empty;

                for (int i = 0; i < 2; i++)
                {
                    EmployeeViewModel tmp = new EmployeeViewModel();
                    if (i == 0)
                    {
                        tmp = firstModel;
                        if (isSetting)
                            NationalType = "TEST";
                        else
                        {
                            NationalType = (firstModel.EmployeeName.Length >= 2 && firstModel.EmployeeName.Length <= 3) ? "TAIWAN" :
                                             firstModel.EmployeeName.Length >= 3 && firstModel.EmployeeName.Length <= 5 ? "JAPAN" : "TAIWAN";
                        }
                    }
                    else
                    {
                        tmp = updateModel;
                        NationalType = (updateModel.EmployeeName.Length >= 2 && updateModel.EmployeeName.Length <= 3) ? "TAIWAN" :
                            updateModel.EmployeeName.Length >= 3 && updateModel.EmployeeName.Length <= 5 ? "JAPAN" : "TAIWAN";
                    }

                    var mainpulationConditions = new Conditions()
                        {
                            {"@EmployeeID", tmp.EmployeeID },
                            {"@NationalType", NationalType},
                            {"@ADAccount", tmp.ADAccount},
                        };

                    string strSQL = @"
                                        update Employee
                                        set
                                        ADAccount = @ADAccount,
                                        NationalType = @NationalType
                                        where EmployeeID = @EmployeeID
                                        ";

                    dc.ExecuteAndCheck(strSQL, mainpulationConditions);
                }

                firstModel = this.GetEmpDataByEmpID(emp1Id);
                updateModel = this.GetEmpDataByEmpID(emp2Id);

                empID1EmpId.Text = firstModel.EmployeeID;
                empID1ADAccount.Text = firstModel.ADAccount;
                empID1Name.Text = firstModel.EmployeeName;
                empID1Status.Text = firstModel.NationalType != "TEST" ? "未設定" : "處理中";

                empID2EmpId.Text = updateModel.EmployeeID;
                empID2ADAccount.Text = updateModel.ADAccount;
                empID2Name.Text = updateModel.EmployeeName;
                empID2Status.Text = updateModel.NationalType != "TEST" ? "未設定" : "處理中";

                cs.RegisterClientScriptBlock(this.GetType(), "PopupScript", "var isSuccess = true; var isSetting = " + isSetting.ToString().ToLower() + ";", true);
            }
            catch (Exception ex)
            {
                comment.Value = ex.Message;
            }
        }

        //down
        protected void Button4_Click(object sender, EventArgs e)
        {
        }
    }
}