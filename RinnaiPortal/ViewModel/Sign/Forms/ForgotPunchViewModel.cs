using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel.Sign.Forms
{
	public class ForgotPunchViewModel : RinnaiForms
	{
		public ForgotPunchViewModel()
		{
			FormSeries = "ForgotPunch";
		}

		[Valiator(ValidateRuleOption.Required, "轉換申請日期為必填欄位!")]
		[Valiator(ValidateRuleOption.DateFormat, "轉換申請日期格式失敗!")]
		public DateTime ApplyDateTime { get; set; }

		[Valiator(ValidateRuleOption.Required, "忘刷員工編號為必填欄位!")]
		public string EmployeeID_FK { get; set; }

		[Valiator(ValidateRuleOption.Required, "忘刷員工姓名為必選!")]
		[Valiator(ValidateRuleOption.Number, "忘刷員工姓名值需為數字!")]
		[Valiator(ValidateRuleOption.GreaterThanZero, "忘刷員工姓名值需為大於零的整數!")]
		public int EmployeeID_FKSelectedIndex { get; set; }

		public string EmployeeName { get; set; }

		[Valiator(ValidateRuleOption.Required, "忘刷員工部門為必填欄位!")]
		public string DepartmentID_FK { get; set; }

		[Valiator(ValidateRuleOption.Required, "忘刷員工部門為必選!")]
		[Valiator(ValidateRuleOption.Number, "忘刷員工部門值需為數字!")]
		[Valiator(ValidateRuleOption.GreaterThanZero, "忘刷員工部門值需為大於零的整數!")]
		public int DepartmentID_FKSelectedIndex { get; set; }

		public string DepartmentName { get; set; }

		[Valiator(ValidateRuleOption.Required, "忘刷類型為必選!")]
		[Valiator(ValidateRuleOption.Number, "忘刷類型值需為數字!")]
		[Valiator(ValidateRuleOption.GreaterThanZero, "忘刷類型值需為大於零的整數!")]
		public int PeriodType { get; set; }

		public string PunchName { get; set; }

		[Valiator(ValidateRuleOption.DateFormatNullable, "轉換上班忘刷日期格式失敗!")]
		public DateTime? ForgotPunchInDateTime { get; set; }

		[Valiator(ValidateRuleOption.DateFormatNullable, "轉換下班忘刷日期格式失敗!")]
		public DateTime? ForgotPunchOutDateTime { get; set; }

		public DateTime? RealPunchIn { get; set; }
		public DateTime? RealPunchOut { get; set; }

		public new bool AutoInsert = true;
	}
}