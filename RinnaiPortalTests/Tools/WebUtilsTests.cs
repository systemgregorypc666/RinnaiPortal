using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RinnaiPortal.Tools;
using RinnaiPortal.Extensions;
using Xunit;
using Xunit.Extensions;
using System.Web;
using RinnaiPortal.Interface;
using RinnaiPortal.Area.Manage;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortalTests.Tools.Compare;
using System.Web.UI;
using System.Web.UI.WebControls;
using RinnaiPortal.ViewModel;
using Moq;
using System.Security.Principal;
using RinnaiPortal.Area.Sign;
using RinnaiPortalTests.Tools.TestData;
using System.Data;

namespace RinnaiPortal.Tools.Tests
{
	public class WebUtilsTests
	{

		public static IEnumerable<object[]> PaggerParmsData
		{
			get
			{
				// lst TestData
				yield return new object[] 
				{ 
					new HttpRequest(null, "Http://127.0.01", "orderField=EmployeeID&descending=True&queryText=F&pageIndex=1&pageSize=30"), 
					new PaggerParms()
					{
						Descending = true,
						OrderField = "EmployeeID",
						PageIndex = 1,
						PageSize = 30
					}
				};
				// 2ed TestData
				yield return new object[] 
				{ 
					new HttpRequest(null, "Http://127.0.01", "orderField=EmployeeID&descending=True"), 
					new PaggerParms()
					{
						Descending = true,
						OrderField = "EmployeeID",
						PageIndex = 1,
						PageSize = 10,
					}
				};
				// 3rd TestData
				yield return new object[] 
				{ 
					new HttpRequest(null, "Http://127.0.01", "PageIndex=EmployeeID&PageSize=True"), 
					new PaggerParms()
					{
						Descending = false,
						OrderField = "CreateDate",
						PageIndex = 1,
						PageSize = 10,
					}
				};
			}
		}

		[Theory]
		[MemberData("PaggerParmsData")]
		public static void ParseQueryStringTest(HttpRequest request, PaggerParms expect)
		{
			var actual = WebUtils.ParseQueryString<PaggerParms>(request);
			Assert.Equal(expect, actual,new ParmsCompare<PaggerParms>());
		}

		public static IEnumerable<object[]> SignListParmsData
		{
			get
			{
				// lst TestData
				yield return new object[] 
				{ 
					new HttpRequest(null, "Http://127.0.01", "queryText=F&SignDocID=OT201507060001"), 
					new SignListParms()
					{
						QueryText = "F",
						SignDocID = "OT201507060001",
					}
				};
				// 2ed TestData
				yield return new object[] 
				{ 
					new HttpRequest(null, "Http://127.0.01", "queryText=F"), 
					new SignListParms()
					{
						QueryText = "F",
						SignDocID = String.Empty
					}
				};
			}
		}

		[Theory]
		[MemberData("SignListParmsData")]
		public static void ParseQueryStringTest(HttpRequest request, SignListParms expect)
		{
			var actual = WebUtils.ParseQueryString<SignListParms>(request);
			Assert.Equal(expect, actual, new ParmsCompare<SignListParms>());
		}

		public static IEnumerable<object[]> PageViewModelMappingData
		{
			get
			{
				var dateTimeNow = DateTime.Now;
				var page = new MyPage();

				yield return new object[] 
				{ 
					page ,
					new DepartmentViewModel()
					{
						DepartmentID = "9999",
						FilingEmployeeID_FK = "01497",
						Disabled = false,
						DisabledDate = dateTimeNow,
						TimeStamp = dateTimeNow.FormatDatetime(),
						UpperDepartmentID = "8888",
						FilingEmployeeID_FKSelectedIndex = 0,
						Creator = null,
						Modifier = null,
						ChiefID_FK = null,
						ChiefID_FKSelectedIndex = 0,
						DepartmentLevel = 0,
						DepartmentName = null,
					}
				};
			}
		}

		[Theory]
		[MemberData("PageViewModelMappingData")]
		public static void PageViewModelMappingTest<T>(MyPage page, T e)
		{
			if (e is DepartmentViewModel) 
			{
				var expect = e as DepartmentViewModel;
				var actual = WebUtils.ViewModelMapping<DepartmentViewModel>(page);
				Assert.Equal(expect, actual, new ParmsCompare<DepartmentViewModel>());
			}
		}

		public static IEnumerable<object[]> DicViewModelMappingData
		{
			get
			{
				var dateTimeNow = DateTime.Now;

				yield return new object[]
				{
					new Dictionary<string, object> ()
					{
						{ "DepartmentID", "9999" },
						{ "FilingEmployeeID_FK", "01497" },
						{ "Disabled", false },
						{ "DisabledDate", dateTimeNow },
						{ "TimeStamp", dateTimeNow.FormatDatetime() },
						{ "UpperDepartmentID", "8888" },
						{ "FilingEmployeeID_FKSelectedIndex", 0 },
						{ "ChiefID_FKSelectedIndex", 0 },
						{ "DepartmentLevel", 0 },
					} ,
					new DepartmentViewModel()
					{
						DepartmentID = "9999",
						FilingEmployeeID_FK = "01497",
						Disabled = false,
						DisabledDate = dateTimeNow,
						TimeStamp = dateTimeNow.FormatDatetime(),
						UpperDepartmentID = "8888",
						FilingEmployeeID_FKSelectedIndex = 0,
						Creator = null,
						Modifier = null,
						ChiefID_FK = null,
						ChiefID_FKSelectedIndex = 0,
						DepartmentLevel = 0,
						DepartmentName = null,
					}
				};
			}
		}

		[Theory]
		[MemberData("DicViewModelMappingData")]
		public static void DicViewModelMappingTest<T>(Dictionary<string, object> data, T e)
		{
			if (e is DepartmentViewModel)
			{
				var expect = e as DepartmentViewModel;
				var actual = WebUtils.ViewModelMapping<DepartmentViewModel>(data);
				Assert.Equal(expect, actual, new ParmsCompare<DepartmentViewModel>());
			}
		}

		public static IEnumerable<object[]> RowViewModelMappingData
		{
			get
			{
				var dateTimeNow = DateTime.Now;
				var table = new DataTable();
				table.Columns.Add("DepartmentID");
				table.Columns.Add("FilingEmployeeID_FK");
				table.Columns.Add("Disabled");
				table.Columns.Add("DisabledDate");
				table.Columns.Add("TimeStamp");
				table.Columns.Add("UpperDepartmentID");
				table.Columns.Add("FilingEmployeeID_FKSelectedIndex");
				table.Columns.Add("ChiefID_FKSelectedIndex");

				var newRow = table.NewRow();

				newRow["DepartmentID"] = "9999";
				newRow["FilingEmployeeID_FK"] = "01497";
				newRow["Disabled"] = false;
				newRow["DisabledDate"] = dateTimeNow;
				newRow["TimeStamp"] = dateTimeNow.FormatDatetime();
				newRow["UpperDepartmentID"] = "8888";
				newRow["FilingEmployeeID_FKSelectedIndex"] = 0;
				newRow["ChiefID_FKSelectedIndex"] = 0;

				yield return new object[]
				{
					newRow,
					new DepartmentViewModel()
					{
						DepartmentID = "9999",
						FilingEmployeeID_FK = "01497",
						Disabled = false,
						DisabledDate = dateTimeNow,
						TimeStamp = dateTimeNow.FormatDatetime(),
						UpperDepartmentID = "8888",
						FilingEmployeeID_FKSelectedIndex = 0,
						Creator = null,
						Modifier = null,
						ChiefID_FK = null,
						ChiefID_FKSelectedIndex = 0,
						DepartmentLevel = 0,
						DepartmentName = null,
					}
				};
			}
		}

		[Theory]
		[MemberData("RowViewModelMappingData")]
		public static void RowViewModelMappingTest<T>(DataRow data, T e)
		{
			if (e is DepartmentViewModel)
			{
				var expect = e as DepartmentViewModel;
				var actual = WebUtils.ViewModelMapping<DepartmentViewModel>(data);
				Assert.Equal(expect, actual, new ParmsCompare<DepartmentViewModel>());
			}
		}


		public static IEnumerable<object[]> TypeToObjectData
		{
			get
			{
				var dateTimeNow = DateTime.Now;

				yield return new object[] { "123", "123", typeof(String) };
				yield return new object[] { 123, "123", typeof(String) };

				yield return new object[] { 123, 123, typeof(Int32) };
				yield return new object[] { "123", 123, typeof(Int32) };
				yield return new object[] { "ddd", null, typeof(Int32) };

				yield return new object[] { "true", true, typeof(Boolean) };
				yield return new object[] { false, false, typeof(Boolean) };
				yield return new object[] { "123", null, typeof(Boolean) };

				yield return new object[] { "0001/1/1 上午 12:00:00", DateTime.MinValue, typeof(DateTime) };
				yield return new object[] { "0001/1/1 00:00:00", DateTime.MinValue, typeof(DateTime) };
				yield return new object[] { "0001-1-1 00:00:00", DateTime.MinValue, typeof(DateTime) };
				yield return new object[] { "2015-1-1 00:00:00", new DateTime(2015,1,1), typeof(DateTime) };
				yield return new object[] { "2015-1-1", new DateTime(2015, 1, 1), typeof(DateTime) };
				yield return new object[] { "2015/12/31", new DateTime(2015, 12, 31), typeof(DateTime) };
				yield return new object[] { dateTimeNow.Date, dateTimeNow.Date, typeof(DateTime) };
				yield return new object[] { dateTimeNow.Year, null, typeof(DateTime) };
				yield return new object[] { "20151231", null, typeof(DateTime) };
				yield return new object[] { "123dfeq", null, typeof(DateTime) };

				yield return new object[] { 0.1, 0.1, typeof(Double) };
				yield return new object[] { 1, 1.0, typeof(Double) };
				yield return new object[] { 1.1, 1.1, typeof(Double) };
				yield return new object[] { "1", 1.0, typeof(Double) };
				yield return new object[] { "1.2", 1.2, typeof(Double) };
				yield return new object[] { "0", 0.0, typeof(Double) };
				yield return new object[] { 0, 0.0, typeof(Double) };
				yield return new object[] { 0.0, 0.0, typeof(Double) };
				yield return new object[] { "123dfeq", null, typeof(Double) };
			}
		}

		[Theory]
		[MemberData("TypeToObjectData")]
		public static void TypeToObjectTest(string orgData, object expect, Type type)
		{
			object actual = WebUtils.TypeToObject(orgData, type);
			Assert.Equal(expect, actual);
		}
	}
}
