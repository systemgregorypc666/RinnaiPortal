using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Extensions
{
	public static class ResponseAction
	{
		public static void ShowMessageAndRefresh(this HttpResponse response, string message)
		{
			response.Write(message);
			response.AddHeader("Refresh", "0");
		}
	}
}