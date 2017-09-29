using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Tools
{
	public static class ViewUtils
	{
		public static Dictionary<string, string> ConstructDropDownList(Dictionary<string, string> dataSource, string defaultKey = "", string defaultValue = "請選擇")
		{
			dataSource = dataSource.Reverse().ToDictionary(x => x.Key, x => x.Value);
			if (dataSource == null) { dataSource = new Dictionary<string, string>(); }
			dataSource.Add(defaultKey, defaultValue);
			return dataSource.Reverse().ToDictionary(x => x.Key, x => x.Value);
		}

		public static void SetOptions(DropDownList dropDownList, Dictionary<string, string> options) 
		{
			dropDownList.DataSource = options;
			dropDownList.DataValueField = "Key";
			dropDownList.DataTextField = "Value";
			dropDownList.DataBind();
		}

		public static void SetOptions(ListBox listBox, Dictionary<string, string> options)
		{
			listBox.DataSource = options;
			listBox.DataValueField = "Key";
			listBox.DataTextField = "Value";
			listBox.DataBind();
		}

		public static void SetGridView(GridView gridView, DataTable source)
		{
			gridView.DataSource = source;
			gridView.DataBind();
		}

		public static string ParseStatus(this string status)
		{
			string result = string.Empty;
			switch (status)
			{
				case "1":
					result = "草稿";
					break;
				case "2":
					result = "待簽核";
					break;
				case "3":
					result = "核准";
					break;
				case "4":
					result = "駁回";
					break;
				case "5":
					result = "取消";
					break;
				case "6":
					result = "結案";
					break;
				case "7":
					result = "歸檔";
					break;                
				default:
					result = String.Empty;
					break;
			}

			return result;
		}

		public static string ParseMealOrder(this string mealOrder)
		{
			string result = string.Empty;
			switch (((string)mealOrder).ToLower())
			{
				case "vegan":
					result = "素食";
					break;
				case "carnivore":
					result = "葷食";
					break;
				default:
				case "none":
					result = "不訂餐";
					break;
			}
			return result;
		}

		public static string ParsePayType(this string payType)
		{
			string result = string.Empty;
			switch (((string)payType).ToLower())
			{
				case "overtimeleave":
                    result = "換休";
					break;
				case "overtimepay":
					result = "加班費";
					break;
				default:
					break;
			}
			return result;
		}

		public static string ParseNationalType(this string nationalType)
		{
			string result = string.Empty;
			switch (((string)nationalType).ToLower())
			{
				case "taiwan":
					result = "台灣";
					break;
				case "japan":
					result = "日本";
					break;
				case "indonesia":
					result = "印尼";
					break;
				case "vietnam":
					result = "越南";
					break;
				default:
					break;
			}
			return result;
		}

		public static string ParseBoolean(this string b)
		{
			return "True".Equals(b) ? "是" : "否";
		}

		public static void ButtonOn(this Button mainBtn, LinkButton coverBtn) 
		{
			mainBtn.CssClass = mainBtn.CssClass.Replace("display-none", "");
			coverBtn.CssClass += " display-none";
		}

		public static void ButtonOff(this Button mainBtn, LinkButton coverBtn) 
		{
			//btn處理
			coverBtn.CssClass = coverBtn.CssClass.Replace("display-none", "");
			mainBtn.CssClass += " display-none";
		}

		public static void ShowRefreshBtn(LinkButton coverBtn, HtmlAnchor refreshBtn)
		{
			refreshBtn.Attributes["class"] = refreshBtn.Attributes["class"].Replace("display-none", "");
			coverBtn.CssClass += " display-none";
		}

	}
}