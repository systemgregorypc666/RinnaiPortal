using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace RinnaiPortal.Tools
{
	public static class ConfigUtils
	{
		public static Dictionary<string, string> ParsePageSetting(string configKey)
		{
			if (String.IsNullOrWhiteSpace(configKey)) { return null; }

			var jsonData = System.Configuration.ConfigurationManager.AppSettings[configKey];
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			var data = serializer.Deserialize<Dictionary<string, string>>(jsonData);

			return data;
		}
	}
}