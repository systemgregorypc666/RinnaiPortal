using RinnaiPortal.Repository;
using RinnaiPortal.Repository.Manage;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Repository.SmartMan;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RinnaiPortal.FactoryMethod
{
    public static class RepositoryFactory
    {
        public static Dictionary<string, string> PotalConn = connPaser("LocalConnectionStringName");
        public static Dictionary<string, string> SmartManConn = connPaser("SmartManConnectionStringName");
        public static Dictionary<string, string> TrainConn = connPaser("TrainingConnectionStringName");

        // Connectionstring Paser
        private static Dictionary<string, string> connPaser(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) { throw new Exception("查無對應連線字串設定!"); }
            var connectionData = System.Configuration.ConfigurationManager.ConnectionStrings[name];
            if (connectionData == null) { throw new ArgumentException("連線字串名稱有誤,請重新確認!"); }

            var result = new Dictionary<string, string>();

            #region 0021 因Roger遮罩改變，無法ping到志元主機，故改程式判斷

            if (name == "SmartManConnectionStringName")
                result.Add("DataSource", "iteip");
            else
                result.Add("DataSource", Regex.Match(connectionData.ConnectionString, @"(?i)Data\s+Source=(?<DataSource>.*?);").Groups["DataSource"].Value);

            #endregion 0021 因Roger遮罩改變，無法ping到志元主機，故改程式判斷

            result.Add("Catelog", Regex.Match(connectionData.ConnectionString, @"(?i)catalog=(?<catelog>.*?);").Groups["catelog"].Value);

            return result;
        }

        // Global Repo
        public static RootRepository CreateRootRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new RootRepository(dc);
        }

        public static RinnaiPortalRepository CreateRinnaiPortalRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new RinnaiPortalRepository(dc);
        }

        public static MemberRepository CreateMemberRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new MemberRepository(dc, CreateRootRepo());
        }

        //Manage Repo
        // Specify Repo
        public static MealTaxiRepository CreateMealTaxiRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new MealTaxiRepository(dc);
        }

        public static OvertimeReportRepository CreateOvertimeReportRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new OvertimeReportRepository(dc);
        }

        public static OvertimeSettingRepository CreateOvertimeSettingRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new OvertimeSettingRepository(dc);
        }

        //SmartMan
        // Specify Repo
        public static SmartManRepository CreateSmartManRepo()
        {
            var dc = ConnectionFactory.GetSmartManDC();
            return new SmartManRepository(dc);
        }

        public static AutoInsertHandler CreateAutoInsert()
        {
            var dc = ConnectionFactory.GetSmartManDC();
            return new AutoInsertHandler(dc);
        }

        public static AutoInsertHandler CreateAutoInsert(string SignDocID)
        {
            var dc = ConnectionFactory.GetSmartManDC();
            return new AutoInsertHandler(dc, SignDocID);
        }

        //Sign
        // Forms Repo
        public static ForgotPunchRepository CreateForgotPunchRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new ForgotPunchRepository(dc, CreateRootRepo(), CreateProcessWorkflowRepo());
        }

        public static OvertimeRepository CreateOvertimeRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new OvertimeRepository(dc, CreateRootRepo(), CreateProcessWorkflowRepo());
        }

        public static TrainRepository CreateTrainRepo()
        {
            var dc = ConnectionFactory.GetTrainingDC();
            return new TrainRepository(dc);
        }

        public static TrainDetailRepository CreateTrainDetailRepo()
        {
            var dc = ConnectionFactory.GetTrainingDC();
            return new TrainDetailRepository(dc, CreateRootRepo(), CreateProcessWorkflowRepo());
        }

        // Specify Repo
        public static AgentRepository CreateAgentRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new AgentRepository(dc, CreateRootRepo());
        }

        public static DepartmentRepository CreateDepartmentRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new DepartmentRepository(dc, CreateRootRepo());
        }

        public static BbsRepository CreateBbsRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new BbsRepository(dc, CreateRootRepo());
        }

        public static BbsListRepository CreateBbsListRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new BbsListRepository(dc, CreateRootRepo());
        }

        public static MonthlyRepository CreateMonthlyRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new MonthlyRepository(dc, CreateRootRepo());
        }

        public static DailyRepository CreateDailyRepo()
        {
            var dc = ConnectionFactory.GetSmartManDC();
            return new DailyRepository(dc, CreateRootRepo());
        }

        public static RecreateRepository CreateRecreateRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new RecreateRepository(dc, CreateRootRepo());
        }

        public static EmployeeRepository CreateEmployeeRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new EmployeeRepository(dc, CreateRootRepo());
        }

        public static GroupRepository CreateGroupRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new GroupRepository(dc, CreateRootRepo());
        }

        public static ProcedureRepository CreateProcedureRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new ProcedureRepository(dc);
        }

        public static ProcessWorkflowRepository CreateProcessWorkflowRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new ProcessWorkflowRepository(dc, CreateRootRepo());
        }

        public static TypeRepository CreateTypeRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new TypeRepository(dc, CreateRootRepo());
        }

        public static WorkflowDetailRepository CreateWorkflowDetailRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new WorkflowDetailRepository(dc, CreateRootRepo());
        }

        //20161209 新增(主管簽核歷程查詢用)
        public static ProcessSignedRepository CreateSignQueryRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new ProcessSignedRepository(dc, CreateRootRepo());
        }

        public static WorkflowQueryRepository CreateWorkflowQueryRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new WorkflowQueryRepository(dc);
        }

        public static SalaryLimitRepository CreateSalaryLimitRepo()
        {
            var dc = ConnectionFactory.GetPortalDC();
            return new SalaryLimitRepository(dc, CreateRootRepo());
        }
    }
}