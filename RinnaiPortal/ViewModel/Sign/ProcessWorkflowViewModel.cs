using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.ViewModel
{
	public class ProcessWorkflowViewModel
	{
		public ProcessWorkflowViewModel()
		{
			ChiefDeptIDs = new List<string>();
            ChiefIDs = new List<string>();
			RinnaiForms = new List<RinnaiForms>();
		}

		public string SignDocID { get; set; }
		public int FormID_FK { get; set; }
		public string FormSeries { get; set; }
		public string RuleID_FK { get; set; } // SignID_FK
		public string EmployeeID_FK { get; set; }
		public DateTime? SendDate { get; set; }
		public string CurrentSignLevelDeptID_FK { get; set; }
		public string CurrentSignLevelDeptName { get; set; }
		public int FinalStatus { get; set; }
		public int Remainder { get; set; }
		public string Creator { get; set; }
		public DateTime CreateDate { get; set; }
		public string Modifier { get; set; }
		public DateTime? ModifyDate { get; set; }

		public List<ProcessWorkflowDetailViewModel> WorkflowDetailList { get; set; }
		public List<RinnaiForms> RinnaiForms { get; set; }
		public List<string> ChiefDeptIDs { get; set; }

        public List<string> ChiefIDs { get; set; }
	}

	public class ProcessWorkflowDetailViewModel
	{
		public int SN { get; set; }
		public string SignDocID_FK { get; set; }
		public string ChiefID_FK { get; set; }
		public string Remark { get; set; }
		public int Status { get; set; }
		public string Creator { get; set; }
		public DateTime CreateDate { get; set; }
		public string Modifier { get; set; }
		public DateTime? ModifyDate { get; set; }
	}
}