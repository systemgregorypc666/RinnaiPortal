using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Tools.Sign.Forms
{
	public static class DateTimeCompare
	{
		public static bool IsCovered(DateTime baseStart, DateTime baseEnd, DateTime targetStart, DateTime targetEnd)
		{
			return DateTime.Compare(targetStart, baseEnd) < 0 && DateTime.Compare(targetEnd, baseStart) > 0;
		}
	}
}