using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.ViewModel.Sign.Forms
{
	public class OvertimeVerifyViewModel
	{
		public string empID { get; set; }

		public string workType { get; set; }

		public DateTime startTime { get; set; }
		public DateTime endTime { get; set; }

		public DateTime onWorkTime { get; set; }
		public DateTime noonTime { get; set; }
		public DateTime afternoonWorkTime { get; set; }
		public DateTime offWorkTime { get; set; }
		public DateTime addWorkTime { get; set; }

		public DateTime punchIN { get; set; }
		public DateTime punchOUT { get; set; }
	}
}