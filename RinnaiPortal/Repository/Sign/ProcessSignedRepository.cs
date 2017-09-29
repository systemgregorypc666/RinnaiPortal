using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository
{
	public class ProcessSignedRepository : ISignRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
        public ProcessSignedRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		//列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string strSQL =
            @"SELECT distinct  
	               CASE 
		             WHEN signdocid_fk <> '' THEN signdocid_fk 
		             ELSE detailsigndocid_fk 
	               END AS SignDocID_FK, 
                   smain.FormID_FK,
	               formtype, 
	               smain.EmployeeID_FK EmployeeID_FK, 	   
	               emp.employeename employeename,
	               departmentname, 
	               smain.FinalStatus FinalStatus
            FROM   signform_log slog 
	               LEFT OUTER JOIN SignForm_Main smain
					            ON slog.detailsigndocid_fk=smain.SignDocID
	               LEFT OUTER JOIN signtype type 
					            ON smain.formid_fk = type.formid
	               LEFT OUTER JOIN employee emp 
					            ON smain.EmployeeID_FK = emp.employeeid	   
	               LEFT OUTER JOIN department dept 
					            ON emp.DepartmentID_FK = dept.departmentid
                   LEFT OUTER join Employee chief 
					            ON slog.ChiefID_FK = chief.EmployeeID 	    	 
            {0}";

            string strConditions = "where (slog.Status='3' or slog.Status='4') and chief.ADAccount=@ADAccount  ";
            //and chief.ADAccount=@ADAccount 
            var conditionsDic = new Conditions()
			{
				{ "@ADAccount", slParms.Member.ADAccount}
			};
            //組成查詢條件 SQL
            if (!String.IsNullOrWhiteSpace(slParms.QueryText))
            {
                conditionsDic.Add("@queryText", String.Format("{0}%", slParms.QueryText.Trim()));
                strConditions += String.Format(" and smain.EmployeeID_FK like @queryText or emp.EmployeeName like @queryText or CASE WHEN signdocid_fk <> '' THEN signdocid_fk ELSE detailsigndocid_fk END like @queryText and  (slog.Status='3' or slog.Status='4') ");
            }

            var paginationParms = new PaginationParms()
            {
                QueryString = String.Format(strSQL, strConditions),
                QueryConditions = conditionsDic,
                PageIndex = pParms.PageIndex,
                PageSize = pParms.PageSize
            };

            //string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);
            string orderExpression = "SignDocID_FK";

            var allowColumns = new List<string>
			{
				"SignDocID_FK", "formtype", "EmployeeID_FK", "EmployeeName", "departmentname", "FinalStatus"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }



	}
}