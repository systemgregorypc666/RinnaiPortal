using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Tools.Sign.Forms
{
	public static class RangeComputor
	{

		// 上班前加班
		public static TimeSpan BeforeOnWorkRange(OvertimeVerifyViewModel model)
		{
			model.startTime = HMformater(model.onWorkTime, model.startTime);
			return model.endTime - model.startTime;
		}

		// 下班後加班
		public static TimeSpan AfterAddWorkRange(OvertimeVerifyViewModel model)
		{
			model.startTime = HMformater(model.addWorkTime, model.startTime);
			return model.endTime - model.startTime;
		}

		// 上午上班
		public static TimeSpan MorningRange(OvertimeVerifyViewModel model)
		{
			var range = new TimeSpan();
			model.startTime = HMformater(model.onWorkTime, model.startTime);
			if (model.endTime <= model.afternoonWorkTime)
			{
				range = model.endTime - model.startTime;
			}
			else if (model.endTime <= model.addWorkTime)
			{
				var morningRange = model.noonTime - model.startTime;
				var noonRange = model.endTime - model.afternoonWorkTime;
				range = morningRange + noonRange;
			}
			else if (model.endTime > model.addWorkTime)
			{
				var morningRange = model.noonTime - model.startTime;
				var noonRagne = model.offWorkTime - model.afternoonWorkTime;
				var overRange = OvertimeRange(model);
				range = morningRange + noonRagne + overRange;
			}

			return range;
		}

		// 下午上班
		public static TimeSpan AfternoonRange(OvertimeVerifyViewModel model)
		{
			var range = new TimeSpan();

			model.startTime = HMformater(model.afternoonWorkTime, model.startTime);
			var noonRange = model.offWorkTime - model.startTime;
			if (model.endTime <= model.addWorkTime)
			{
				range = model.endTime - model.startTime;
			}
			else if (model.endTime > model.addWorkTime)
			{
				var overRange = OvertimeRange(model);
				range = noonRange + overRange;
			}

			return range;
		}

		// 下班時間後上班
		public static TimeSpan OvertimeRange(OvertimeVerifyViewModel model)
		{
			return model.endTime - model.addWorkTime;
		}
        // 下班時間後上班 for 總務G班
        public static TimeSpan OvertimeRangeG(OvertimeVerifyViewModel model)
        {
            return model.endTime - model.startTime;
        }

		public static DateTime HMformater(DateTime basetime, DateTime target)
		{
			var result = target;

			//申請 大於 標準
			if (DateTime.Compare(target, basetime) > 0 && basetime.Minute - target.Minute != 0)
			{
				// 修正申請，無條件進位
				result = result.AddHours(1);
				result = result.AddMinutes(basetime.Minute - target.Minute);
			}

			return result;
		}
	}
}