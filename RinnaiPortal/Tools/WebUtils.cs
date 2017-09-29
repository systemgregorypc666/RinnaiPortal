using DBTools;
using RinnaiPortal.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Tools
{
	public static class WebUtils
	{
		/// <summary>
		/// 解析當前 request，並將parms 與 T of instance mapping，並 return T of instance
		/// </summary>
		public static T ParseQueryString<T>(HttpRequest request)
		{
			Type type = typeof(T);
			T instance = (T)Activator.CreateInstance(type);
			type.GetProperties().All(ppinfo =>
			{
				if (!request.Params.AllKeys.Select(x => x.ToUpper()).Contains(ppinfo.Name.ToUpper())) { return true; }
				var value = request.Params[ppinfo.Name];

				object correctValue = TypeToObject(value, ppinfo.PropertyType);
				if (correctValue == null) { return true; }
				ppinfo.SetValue(instance, correctValue);
				return true;
			});

			return instance;
		}

		/// <summary>
		/// 解析 member name of instance, 對應到 page server controls 
		/// 根據 server controls type 取出 value/text
		/// 最後跟 member of instance mapping
		/// 當 sufix 為 SelectedIndex 則會以捨去 SelectedIndex 的詞綴作為型別判斷 DropDownList Index
		/// </summary>
		public static T ViewModelMapping<T>(Page page)
		{
			Type type = typeof(T);
			T instance = (T)Activator.CreateInstance(type);
			type.GetProperties().All(ppinfo =>
			{
				var control = FindControlRecursive(page, ppinfo.Name);
				string value = null;
				if (control is HiddenField)
				{
					value = ((HiddenField)control).Value;
				}
				else if (control is TextBox)
				{
					value = ((TextBox)control).Text;
				}
				else if (control is DropDownList)
				{
					value = ((DropDownList)control).SelectedValue;
				}
				else if (control is ListBox)
				{
					value = String.Join(",", ((ListBox)control).GetSelectedIndices().Select(index => ((ListBox)control).Items[index].Value));
				}
				else if (control is CheckBox)
				{
					value = ((CheckBox)control).Checked.ToString();
				}
				else if (control is RadioButtonList)
				{
					value = ((RadioButtonList)control).SelectedValue;
				}
				else if (control is Label)
				{
					value = ((Label)control).Text;
				}
				else if (Regex.IsMatch(ppinfo.Name, @"SelectedIndex$", RegexOptions.IgnoreCase))
				{
					control = FindControlRecursive(page, Regex.Replace(ppinfo.Name, @"SelectedIndex$", ""));
					if (control == null) { return true; }
					if (control is DropDownList) { value = ((DropDownList)control).SelectedIndex.ToString(); }
					if (control is ListBox) { value = ((ListBox)control).GetSelectedIndices().Sum().ToString(); }

				}
				else
				{
					switch (ppinfo.Name)
					{
						case "Creator":
						case "Modifier":
							value = getCurrentUserName();
							break;
						case "CreateDate":
						case "ModifyDate":
							value = DateTime.Now.FormatDatetime();
							break;
					}
				}
				var correctValue = TypeToObject(value, ppinfo.PropertyType);
				if (correctValue == null) { return true; }
				ppinfo.SetValue(instance, correctValue);
				return true;
			});

			return instance;
		}

		/// <summary>
		/// 解析 member name of instance, 對應到 Dictionary value 
		/// 根據 Dictionary 取出 value
		/// 最後跟 member of instance mapping
		/// </summary>
		public static T ViewModelMapping<T>(Dictionary<string, object> data)
		{
			Type type = typeof(T);
			T instance = (T)Activator.CreateInstance(type);
			type.GetProperties().All(ppinfo =>
			{
				string value = null;
				if (data.Keys.Contains(ppinfo.Name))
				{
					value = data[ppinfo.Name].ToString();
				}

				switch (ppinfo.Name)
				{
					case "Creator":
					case "Modifier":
						value = getCurrentUserName();
						break;
					case "CreateDate":
					case "ModifyDate":
						value = DateTime.Now.FormatDatetime();
						break;
				}
				var correctValue = TypeToObject(value, ppinfo.PropertyType);
				if (correctValue == null) { return true; }
				ppinfo.SetValue(instance, correctValue);

				return true;
			});

			return instance;
		}

		/// <summary>
		/// 將 DataRow 轉為 Dictionary&lt;string, object&gt;
		/// 轉呼叫 ViewModelMapping&lt;T&gt;(Dictionary&lt;string, object&gt; data)
		/// </summary>
		public static T ViewModelMapping<T>(DataRow data)
		{
			Dictionary<string, object> dicData = data.Table.Columns.Cast<DataColumn>().ToDictionary(key => key.ColumnName, value => data[value.ColumnName]);
			return ViewModelMapping<T>(dicData);
		}

		private static string getCurrentUserName()
		{
			HttpContext context = HttpContext.Current;
			if (context == null) { return null; }
			IPrincipal user = context.User;
			if (user == null) { return null; }
			return user.Identity.Name;
		}

		/// <summary>
		/// 解析 member name of instance, 取出model member 實值
		/// 跟 page server controls mapping
		/// </summary>
		public static void PageDataBind(object model, Page page)
		{
			if (model == null) { return; }
			Type type = model.GetType();
			type.GetProperties().All(ppinfo =>
			{
				Control control = FindControlRecursive(page, ppinfo.Name);
				if (control == null) { return true; }
				var valueObj = ppinfo.GetValue(model, null);
				string value = valueObj != null ? valueObj.ToString() : null;

				DateTime d;
				if (DateTime.TryParse(value, out d)) { value = value.ToDateTimeFormateString(); }

				if (control is HiddenField)
				{
					((HiddenField)control).Value = value;
				}
				else if (control is TextBox)
				{
					((TextBox)control).Text = value;
				}
				else if (control is DropDownList)
				{
					((DropDownList)control).SelectedValue = value;
				}
				else if (control is ListBox)
				{
					var listBox = ((ListBox)control);
					value.Split(',').ToList().ForEach(val =>
					{
						var listItem = listBox.Items.Cast<ListItem>().SingleOrDefault(li => li.Value == val);
						if (listItem != null) { listItem.Selected = true; }
					});
				}
				else if (control is CheckBox)
				{
					bool b;
					((CheckBox)control).Checked = Boolean.TryParse(value, out b) ? b : false;
				}
				else if (control is Label)
				{
					((Label)control).Text = value;
				}
				return true;
			});

		}

		/// <summary>
		/// 將 value 轉型為 指定 type  並指派到 object 最後 return object
		/// </summary>
		/// <param name="value">要轉型的value</param>
		/// <param name="type">目標 type/param>
		/// <returns >object</returns>
		public static object TypeToObject(string value, Type type)
		{
			bool isNullable = (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
			if (isNullable) { type = type.GetGenericArguments().First(); }

			object result = null;
			try
			{
				result = Convert.ChangeType(value, type);
			}
			catch (Exception)
			{
				return null;
			}
			return result;
		}

		[Obsolete("暫時使用typeToObject Method 替代、觀察!")]
		/// <summary>
		/// 判斷 type， return 驗證後的 value
		/// </summary>
		/// <param name="type">property type/param>
		/// <param name="value">要驗證的 value</param>
		/// <returns >object</returns>        
		private static object identifyType(Type type, string value)
		{
			object result = null;
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Int32:
					int resultInt32;
					if (Int32.TryParse(value, out resultInt32)) { result = resultInt32; }
					break;
				case TypeCode.String:
					result = value;
					break;
				case TypeCode.Boolean:
					bool resultBoolean;
					if (Boolean.TryParse(value, out resultBoolean)) { result = resultBoolean; }
					break;
				case TypeCode.DateTime:
					DateTime resultDatetime;
					if (DateTime.TryParse(value, out resultDatetime)) { result = resultDatetime; }
					break;
				default:
					if (type == typeof(Nullable<DateTime>))
					{
						DateTime resultDatetimeNullable;
						if (DateTime.TryParse(value, out resultDatetimeNullable)) { result = resultDatetimeNullable; }
					}
					break;
			}
			return result;
		}

		/// <summary>
		/// 遞迴尋找 page 中指定 id 的 Server Control
		/// </summary>
		public static Control FindControlRecursive(Control root, string id)
		{
			if (root.ID == id) return root;
			foreach (Control control in root.Controls)
			{
				Control result = FindControlRecursive(control, id);
				if (result != null) return result;
			}
			return null;
		}

		/// <summary>
		/// 取得分頁 PaginationBar Html
		/// </summary>
		/// <param name="pagination">Pagination information</param>
		/// <param name="request">HttpRequest(Current request)</param>\
		/// <param name="barSet">PaginationBar 每 barSet個為一組呈現</param>
		/// <returns></returns>
		public static string GetPagerNumericString(Pagination pagination, HttpRequest request, int barSet = 5)
		{

			//分頁bar 共被分為幾分 = 總分頁數 / barSet
			int barSplitTotalCount = pagination.TotalPages / barSet;
			if (pagination.TotalPages % barSet > 0)
			{
				barSplitTotalCount++;
			}

			StringBuilder resultHtml = new StringBuilder();
			resultHtml.Append(@"<ul class='pagination form-inline'>");
			//上一頁按鈕
			string preBtnHtml = (pagination.HasPreviousPage) ? ConstructQueryString("pageIndex", (pagination.PageIndex - 1).ToString(), request) : "#";
			resultHtml.AppendFormat("<li class='prev {0}'><a runat=\"server\" href='{1}'>«</a></li>", "#".Equals(preBtnHtml) ? "disabled" : "", preBtnHtml);
			resultHtml.AppendFormat("<li ><a href='{0}'>First</a></li>", ConstructQueryString("pageIndex", "1", request));

			int barStartIndex = (((pagination.PageIndex - 1) / barSet) * barSet) + 1;
			int barEndIndex = barStartIndex + (barSet - 1);
			if (barEndIndex > pagination.TotalPages)
			{
				barEndIndex = pagination.TotalPages;
			}

			//目前在哪一份(從1算起)，為了顯示「...」
			int currentBarSplitNumber = ((pagination.PageIndex - 1) / barSet) + 1;
			if (currentBarSplitNumber > 1)
			{
				resultHtml.AppendFormat("<li><a href='{0}'>...</a></li>", ConstructQueryString("pageIndex", (barStartIndex - 1).ToString(), request));
			}
			//產生頁數按鈕(從1算起)
			for (int i = barStartIndex; i <= barEndIndex; i++)
			{
				resultHtml.AppendFormat("<li class='{0}'><a href='{1}'>{2}</a></li>", i == pagination.PageIndex ? "active" : "", ConstructQueryString("pageIndex", i.ToString(), request), i);
			}
			if (currentBarSplitNumber < barSplitTotalCount)
			{
				resultHtml.AppendFormat("<li><a href='{0}'>...</a></li>", ConstructQueryString("pageIndex", (barEndIndex + 1).ToString(), request));
			}

			//下一頁按鈕
			string nextBtnHtml = (pagination.HasNextPage ? ConstructQueryString("pageIndex", (pagination.PageIndex + 1).ToString(), request) : "#");
			resultHtml.AppendFormat("<li ><a href='{0}'>Last</a></li>", ConstructQueryString("pageIndex", (pagination.TotalPages).ToString(), request));
			resultHtml.AppendFormat("<li class='next {0}'><a href='{1}'>»</a></li>", "#".Equals(nextBtnHtml) ? "disabled" : "", nextBtnHtml);
			resultHtml.Append("</ul>");

			return resultHtml.ToString();
		}

        //改這個
        public static string GetDailyPagerNumericString(Pagination pagination1, HttpRequest request1, int barSet1 = 5)
        {

            //分頁bar 共被分為幾分 = 總分頁數 / barSet
            int barSplitTotalCount1 = pagination1.TotalPages / barSet1;
            if (pagination1.TotalPages % barSet1 > 0)
            {
                barSplitTotalCount1++;
            }

            StringBuilder resultHtml1 = new StringBuilder();
            resultHtml1.Append(@"<ul class='pagination form-inline'>");
            //上一頁按鈕
            string preBtnHtml1 = (pagination1.HasPreviousPage) ? ConstructQueryString("pageIndex1", (pagination1.PageIndex - 1).ToString(), request1) : "#";
            resultHtml1.AppendFormat("<li class='prev {0}'><a runat=\"server\" href='{1}'>«</a></li>", "#".Equals(preBtnHtml1) ? "disabled" : "", preBtnHtml1);
            resultHtml1.AppendFormat("<li ><a href='{0}'>First</a></li>", ConstructQueryString("pageIndex1", "1", request1));

            int barStartIndex1 = (((pagination1.PageIndex - 1) / barSet1) * barSet1) + 1;
            int barEndIndex1 = barStartIndex1 + (barSet1 - 1);
            if (barEndIndex1 > pagination1.TotalPages)
            {
                barEndIndex1 = pagination1.TotalPages;
            }

            //目前在哪一份(從1算起)，為了顯示「...」
            int currentBarSplitNumber1 = ((pagination1.PageIndex - 1) / barSet1) + 1;
            if (currentBarSplitNumber1 > 1)
            {
                resultHtml1.AppendFormat("<li><a href='{0}'>...</a></li>", ConstructQueryString("pageIndex1", (barStartIndex1 - 1).ToString(), request1));
            }
            //產生頁數按鈕(從1算起)
            for (int i = barStartIndex1; i <= barEndIndex1; i++)
            {
                resultHtml1.AppendFormat("<li class='{0}'><a href='{1}'>{2}</a></li>", i == pagination1.PageIndex ? "active" : "", ConstructQueryString("pageIndex", i.ToString(), request1), i);
            }
            if (currentBarSplitNumber1 < barSplitTotalCount1)
            {
                resultHtml1.AppendFormat("<li><a href='{0}'>...</a></li>", ConstructQueryString("pageIndex1", (barEndIndex1 + 1).ToString(), request1));
            }

            //下一頁按鈕
            string nextBtnHtml1 = (pagination1.HasNextPage ? ConstructQueryString("pageIndex1", (pagination1.PageIndex + 1).ToString(), request1) : "#");
            resultHtml1.AppendFormat("<li ><a href='{0}'>Last</a></li>", ConstructQueryString("pageIndex1", (pagination1.TotalPages).ToString(), request1));
            resultHtml1.AppendFormat("<li class='next {0}'><a href='{1}'>»</a></li>", "#".Equals(nextBtnHtml1) ? "disabled" : "", nextBtnHtml1);
            resultHtml1.Append("</ul>");

            return resultHtml1.ToString();
        }

		/// <summary>
		/// 建立 QueryString, 三參數 Method
		/// </summary>
		/// <param name="key">參數名稱</param>
		/// <param name="value">參數值</param>
		/// <param name="request">當前 HttpRequest</param>
		/// <returns>string</returns>
		public static string ConstructQueryString(string key, string value, HttpRequest request)
		{
			return ConstructQuerytString(new Dictionary<string, string>() { { key, value } }, request);
		}

		/// <summary>
		/// 建立 QueryString, 雙參數 Method
		/// </summary>
		/// <param name="queryParms">參數名與參數值成對的Dictionary</param>
		/// <param name="request">當前 HttpRequest</param>
		/// <returns>string</returns>
		public static string ConstructQuerytString(Dictionary<string, string> queryParms, HttpRequest request)
		{
			if (queryParms == null) { return (string)null; }
			var baseQueryString = request != null ? request.Url.Query : String.Empty;
			var queryStringNameValueCollection = HttpUtility.ParseQueryString(baseQueryString);
			queryParms.All(parm =>
			{
				queryStringNameValueCollection[parm.Key] = parm.Value;
				return true;
			});
			return String.Concat("?", queryStringNameValueCollection);
		}

		/// <summary>
		/// 建立 GridView, table -> th 的排序 link
		/// </summary>
		public static string GetGridViewHeadUrl(object fieldName, HttpRequest request)
		{
            //SendDate LogDatetime
			//透過 QueryString
			//取得原有 descending
			//若取到的值 非True/true 即為 false
			bool descending = true;
			if (!String.IsNullOrEmpty(request.QueryString["descending"]))
			{
				descending = "True".Equals(request.QueryString["descending"], StringComparison.OrdinalIgnoreCase);
			}

			//當下的 orderField 為欄位名稱參數
			string currentOrderField = fieldName.ToString();

			//透過 QueryString
			//取得原有 orderField
			string orginalOrderField = String.IsNullOrWhiteSpace(request.QueryString["orderField"]) ? "CreateDate" : request.QueryString["orderField"].ToString();

			//如果當前 orderField 與 原有 orderField 不相等 就要將 descending 反向
			if (currentOrderField.Equals(orginalOrderField, StringComparison.OrdinalIgnoreCase))
			{
				descending = !descending;
			}

			var parms = new Dictionary<string, string>()
			{
				{"orderField", currentOrderField},
				{"descending", descending.ToString()}
			};

			return WebUtils.ConstructQuerytString(parms, request);
		}


	}
}