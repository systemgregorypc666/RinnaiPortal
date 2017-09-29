using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Manage
{
	public class ManageListParms : ListParms
	{
		public DateTime? StartDateTime { get; set; }
		public DateTime? EndDateTime { get; set; }

		public GridView MealSummary { get; set; }
		public GridView TaxiSummary { get; set; }

		public string FinalStatus { get; set; }
		public string SignDocID { get; set; }

		public ManageListParms()
		{
            StartDateTime = DateTime.Parse(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            EndDateTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
			MealSummary = new GridView();
			TaxiSummary = new GridView();
			FinalStatus = "";
		}
	}
}
