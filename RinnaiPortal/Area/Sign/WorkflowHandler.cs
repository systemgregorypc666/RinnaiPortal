using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace RinnaiPortal.Area.Sign
{
    public abstract class WorkflowHandler
    {
        protected ProcessWorkflowRepository ProcesswfRepo { get; set; }
        public WorkflowHandler Supervisor { get; set; }

        //預設建構子，讓後續衍伸類初始化instance時，不用指定讀取哪一個父類別建構子
        public WorkflowHandler()
        {
            ProcesswfRepo = RepositoryFactory.CreateProcessWorkflowRepo();
        }

        public WorkflowHandler(WorkflowHandler nextHandler)
            : this()
        {
            Supervisor = nextHandler;
        }

        public abstract void SignOff(ProcessWorkflowDetailViewModel workflowModel);

        /*
            Main 狀態:
         * 1 草稿
         * 2 待簽核
         * 3 核准
         * 4 駁回
         * 5 取消
         * 6 結案
         * 7 歸檔
            Detial 狀態:
         * 1
         * 2 待簽核
         * 3 核准
         * 4 駁回
         * 5 取消
         * 6
         * 7
        */

        /// <summary>
        /// 簽核處理函式
        /// </summary>
        /// <param name="workflowModel"></param>
        /// <param name="sqlDML"></param>
        /// <param name="orgSupervisorID"></param>
        public void ProcessIdentify(ProcessWorkflowViewModel workflowModel, List<MultiConditions> sqlDML, string orgSupervisorID)
        {
            var sqlList = new List<MultiConditions>();
            //var detail = workflowModel.WorkflowDetailList.Single(row => orgSupervisorID.Equals(row.ChiefID_FK));

            #region 0020 會有多筆資料 不該用Single

            var detail = workflowModel.WorkflowDetailList.FirstOrDefault(row => orgSupervisorID.Equals(row.ChiefID_FK));

            #endregion 0020 會有多筆資料 不該用Single

            if (detail.Status == 4)
            {
                //駁回
                workflowModel.FinalStatus = detail.Status;
                var rejectDML = ProcesswfRepo.GetRejectSQLDML(workflowModel, orgSupervisorID);

                //加入DML
                sqlList.AddRange(sqlDML);
                sqlList.AddRange(rejectDML);
            }
            else if (workflowModel.WorkflowDetailList.All(row => row.Status == 3))
            {
                //會簽只差本次主管同意，同意後寫入DB並上呈簽核
                var nextDML = ProcesswfRepo.GetNextSQLDML(workflowModel, orgSupervisorID);

                //加入DML
                sqlList.Add(sqlDML.Last());
                sqlList.AddRange(nextDML);
            }
            else
            {
                //不符合以上情形，簽核結果直接寫入DB
                sqlList.AddRange(sqlDML);
            }

            ProcesswfRepo.ExecuteSQL(sqlList);
        }
    }
}