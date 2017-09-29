using DBTools;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository
{
    public class WorkflowQueryRepository : ISignRepository
    {
        private DB _dc { get; set; }
        public WorkflowQueryRepository(DB dc)
        {
            _dc = dc;
        }

        //列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string strSQL =
@"select distinct signM.*,signD.ChiefID_FK ChiefID_Up,emp.EmployeeName EmployeeName, typ.FormID FormID, typ.FormType FormType, dept.DepartmentName CurrentSignLevelDeptName, dept.ChiefID_FK, chief.ADAccount chiefADAccount, signD.Status
from SignForm_Main signM
left outer join SignForm_Detail signD on signM.SignDocID = signD.SignDocID_FK
left outer join Employee emp on signM.EmployeeID_FK = emp.EmployeeID
left outer join SignType typ on signM.FormID_FK = typ.FormID
left outer join Department dept on signM.CurrentSignLevelDeptID_FK = dept.DepartmentID
left outer join Employee chief on dept.ChiefID_FK = chief.EmployeeID
 {0}";

            var conditionsDic = new Conditions()
            {
                {"@queryText", String.Format("{0}%", slParms.QueryText.Trim())}
            };
            string strConditions = " where FinalStatus<>'5' and SignDocID like @queryText ";

            //if (parms.Member.IsChief)
            //{                                
            //    //組成查詢條件 SQL
            //    if (!String.IsNullOrWhiteSpace(parms.QueryText))
            //    {                    
            //        strConditions += String.Format(" or EmployeeID_FK like @queryText or emp.EmployeeName like @queryText");
            //    }
            //}
            //else 
            //{
            conditionsDic.Add("@EmployeeID_FK", slParms.Member.EmployeeID);
            strConditions += String.Format(" and EmployeeID_FK = @EmployeeID_FK");

            if (!String.IsNullOrWhiteSpace(slParms.FinalStatus))
            {
                conditionsDic.Add("@FinalStatus", slParms.FinalStatus);
                strConditions += String.Format(" and FinalStatus = @FinalStatus");
            }
            else
            {
                strConditions += " And finalStatus <> '6'";
            }
            //conditionsDic.Add("@FinalStatus", slParms.FinalStatus);
            //strConditions += String.Format(" and FinalStatus = @FinalStatus");
            //}

                var paginationParms = new PaginationParms()
                {
                    QueryString = String.Format(strSQL, strConditions),
                    QueryConditions = conditionsDic,
                    PageIndex = pParms.PageIndex,
                    PageSize = pParms.PageSize
                };
                string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

            var allowColumns = new List<string> 
            { 
                "SignDocID", "FormType", "EmployeeID_FK", "EmployeeName", "SendDate", "CurrentSignLevelDeptID_FK", "CurrentSignLevelDeptName","FinalStatus", "Status", "CreateDate"
            };
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }


    }
}