using RinnaiPortal.Repository.Sign;
using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Tools
{
	public static class Authentication
	{
		public static Dictionary<string, Dictionary<string, object>> LoginList { get; set; }

		static Authentication()
		{
			LoginList = new Dictionary<string, Dictionary<string, object>>();
		}

		public static bool HasResource(string user, string resource)
		{
			if (LoginList.ContainsKey(user))
			{
				var resourceList = (List<string>)LoginList[user]["Resource"];
				return resourceList.Exists(item => item.Equals(resource));
			}
			return false;
		}

		public static MemberViewModel GetMemberViewModel(string adAccount)
		{
			MemberViewModel result = null;
			if (LoginList.ContainsKey(adAccount))
			{
				result = WebUtils.ViewModelMapping<MemberViewModel>(LoginList[adAccount]);
			}
			return result;
		}
	}
}