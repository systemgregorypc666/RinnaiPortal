using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RinnaiPortal.ViewModel;

namespace RinnaiPortal.Area.Sign.Handlers
{
    //課長類
    public class SectionChief : WorkflowHandler
    {
        //同 public SectionChief(WorkflowHandler nextHandler): base()
        public SectionChief(WorkflowHandler nextHandler)
        {
            Supervisor = nextHandler;
        }

        /// <summary>
        /// 簽核前置檢查作業及簽核
        /// </summary>
        /// <param name="model"></param>
        public override void SignOff(ProcessWorkflowDetailViewModel model)
        {
            if (model == null)
                throw new Exception("查無擔當簽核明細資料!");

            var sqlDML = ProcesswfRepo.GetSignOffDML(model);
            var workflowModel = ProcesswfRepo.GetWorkflowData(model.SignDocID_FK);

            var newDetailModel = new List<ProcessWorkflowDetailViewModel>();
            //替換明細資料為頁面傳入資料
            workflowModel.WorkflowDetailList.ForEach(row =>
            {
                if (row.ChiefID_FK == model.ChiefID_FK)
                {
                    newDetailModel.Add(model);
                    return;
                }
                newDetailModel.Add(row);
            });
            workflowModel.WorkflowDetailList = newDetailModel;

            //寫入簽核檔
            ProcessIdentify(workflowModel, sqlDML, model.ChiefID_FK);
        }
    }
}