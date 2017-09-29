using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel.Sign
{
	public class SalaryLimitViewModel
	{
		public int SN { get; set; }

		private DateTime _limitDate;
		[Valiator(ValidateRuleOption.Required, "起始日期為必填欄位!")]
		[Valiator(ValidateRuleOption.DateFormat, "轉換起始日期格式失敗!")]
		public DateTime LimitDate
		{
			get
			{
				return _limitDate.Date;
			}
			set
			{
				_limitDate = value.Date;
			}
		}

		public string TimeStamp { get; set; }
		public string Creator { get; set; }
		public DateTime CreateDate { get; set; }
		public string Modifier { get; set; }
		public DateTime ModifyDate { get; set; }

	}
}