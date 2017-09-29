using DBTools;
using RinnaiPortal.Repository.Sign;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository.SmartMan
{
	public class SmartManRepository 
	{
		public bool IsConnect { get; set; }
		private DB _dc { get; set; }

		public SmartManRepository(DB dc) 
		{
			_dc = dc;
			IsConnect = _dc.TestConnection();
		}

		//取得打卡時間資料
		public List<DataRow> QueryForDailyOnOffByDate(string date)
		{
            string strSQL = @"Select * From Dailyonoff Where DutyDate = @DutyDate and ((Begintime is not Null and Endtime is not Null) and (Begintime <>'' and Endtime <>'')) ";
			var conditions = new Conditions() { { "@DutyDate", date } };
			var result = _dc.QueryForDataRows(strSQL, conditions);
			if (result == null) { return null; }

			return result;
		}

        //取得假日類別(Y:例假日 S:休息日 H:國定假日)
        public List<DataRow> QueryH_Type(string date)
        {
            string strSQL = @"Select * From Dailyonoff Where DutyDate = @DutyDate and Begintime is not Null and Endtime is not Null ";
            var conditions = new Conditions() { { "@DutyDate", date } };
            var result = _dc.QueryForDataRows(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }

		public DataRow QueryForDailyOnOff(string date, string employeeID)
		{
			if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @"Select * From Dailyonoff Where DutyDate = @DutyDate and EmployeCd=@EmployeCd ";
			var conditions = new Conditions()
			{
				{ "@DutyDate", date },
				{ "@EmployeCd", employeeID}
			};
			var result = _dc.QueryForDataRow(strSQL, conditions);
			if (result == null) { return null; }

			return result;
		}
        //查當天有無刷卡資料收進
        public DataRow QueryDailyOnOff(string date)
        {
            if (String.IsNullOrWhiteSpace(date)) { return null; }
            string strSQL = @"Select * From Dailyonoff Where DutyDate = @DutyDate and Begintime is not Null and Endtime is not Null";
            var conditions = new Conditions()
			{
				{ "@DutyDate", date }
			};
            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }
        //20161101 add取得員工即時人事成本部門
        public DataRow QueryCostDept(string employeeID)
        {
            if (String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @"Select * From EMPLOYEE where EmployeCd=@EmployeCd";
            var conditions = new Conditions()
			{	
				{ "@EmployeCd", employeeID}
			};

            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }
            return result;
        }

		//取得員工個人行事曆檔
		public DataRow QueryForDutyWork(string date, string employeeID)
		{
			if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
			string strSQL = @"Select * from DutyWork Where EmployeCd = @EmployeCD and WorkDate = @WorkDate";
			var conditions = new Conditions()
			{
				{ "@WorkDate", date},
				{"@EmployeCD",  employeeID},
			};

			var result = _dc.QueryForDataRow(strSQL, conditions);
			if (result == null) { return null; }

			return result;
		}
        //取得員工每月加班總時數(已入志元)
        //20170215 增加國定假日<=8H不列入計算
        public DataRow QueryOverWork(string date, string employeeID)
        {
            if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @"select ISNULL(sum(OVERWORKHOURS),0)-ISNULL(sum(case when H_TYPE='2' and OVERWORKHOURS>=8 then 8 else 0 end),0) as totalH  from DAILYON where EMPLOYECD=@EmployeCD and dutydate between CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'21' else convert(varchar(4),YEAR(DATEADD(MONTH,-1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate)))+'21' end and CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,+1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate)))+'20' else convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20' end";
            var conditions = new Conditions()
			{
				{ "@WorkDate", date},
				{"@EmployeCD",  employeeID},
			};

            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }

        //取得員工每月加班總時數(已入志元)
        //20170310 增加國定假日8H列入計算
        public DataRow QueryAllOverWork(string date, string employeeID)
        {
            if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @"select ISNULL(sum(OVERWORKHOURS),0) as totalH  from DAILYON where EMPLOYECD=@EmployeCD and dutydate between CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'21' else convert(varchar(4),YEAR(DATEADD(MONTH,-1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate)))+'21' end and CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,+1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate)))+'20' else convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20' end";
            var conditions = new Conditions()
			{
				{ "@WorkDate", date},
				{"@EmployeCD",  employeeID},
			};

            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }

        //取得員工某天加班資料
        public DataRow QueryOverWorkByDate(string date, string employeeID)
        {
            if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @"select *  from DAILYON where EMPLOYECD=@EmployeCD and DUTYDATE=@Dutydate";
            var conditions = new Conditions()
			{
				{ "@Dutydate", date},
				{"@EmployeCD",  employeeID},
			};

            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }

		//取得員工個人基本資料檔
		public DataRow QueryForEmployee(string employeeID)
		{
			if (String.IsNullOrWhiteSpace(employeeID)) { return null; }
			string strSQL = @"Select * from Employee Where EmployeCd = @EmployeCD";
			var conditions = new Conditions()
			{
				{"@EmployeCD",  employeeID},
			};

			var result = _dc.QueryForDataRow(strSQL, conditions);
			if (result == null) { return null; }

			return result;
		}

		// 取得公司規定假日記錄檔
        public DataRow QueryForHoliday(string date)
		{
			if (String.IsNullOrWhiteSpace(date)) { return null; }
            string strSQL = @"Select * from Holiday Where COMPANYCD='A' AND HOLDATE = @HOLDATE";
			var conditions = new Conditions()
			{
				{"@HOLDATE",  date},
			};

			var result = _dc.QueryForDataRow(strSQL, conditions);
			if (result == null) { return null; }

			return result;
		}

		// 取得公司班別時間設定檔
		public DataRow QueryForWorkTime(string workType)
		{
			if (String.IsNullOrWhiteSpace(workType)) { return null; }
			string strSQL = @"Select * from WORKTIME  Where COMPANYCD='A' AND WORKTYPE = @WorkType ";
			var conditions = new Conditions()
			{
				{"@WorkType", workType},
			};

			var result = _dc.QueryForDataRow(strSQL, conditions);
			if (result == null) { return null; }

			return result;
		}

		// 取得忘刷卡資料
		public DataTable QueryForLostCard(string empID, string date)
		{
			if (String.IsNullOrWhiteSpace(empID) || String.IsNullOrWhiteSpace(date)) { return null; }
			string strSQL = @"Select * from LostCard Where CardNo = @CardNo and CardDate = @CardDate";
			var condition = new Conditions()
			{
				{"@CardDate", date },
				{ "@CardNo", empID}
			};

			var result = _dc.QueryForDataTable(strSQL, condition);
			if (result == null) { return null; }

			return result;
		}

		// 取得考勤計薪區間設定檔
		public DataRow QueryForPayRange(string beginDate, string endDate)
		{
			if (String.IsNullOrWhiteSpace(beginDate) || String.IsNullOrWhiteSpace(endDate)) { return null; }
			string strSQL = @"Select * from PAYRANGE Where BEGINDATE <= @BeginDate and ENDDATE >= @EndDate";
			var condition = new Conditions()
			{
				{"@BeginDate", beginDate },
				{ "@EndDate", endDate}
			};

			var result = _dc.QueryForDataRow(strSQL, condition);
			if (result == null) { return null; }

			return result;
		}

		// 取得考勤計薪區間設定檔
		public Dictionary<string, string> QueryForPayRange()
		{
			string strSQL = @"Select PAYYYYYMM,PAYYYYYMM from PAYRANGE WHERE PAYYYYYMM LIKE CAST(YEAR(GETDATE()) AS NVARCHAR) +'%' order by PayyyyyMM desc";

			var result = _dc.QueryForDictionary(strSQL, null);
			if (result == null) { return null; }

			return result;
		}

		// 取得員工每月薪資記錄檔
		public DataRow QueryForEmployeePay(string empID, string date)
		{
			if (String.IsNullOrWhiteSpace(empID) || String.IsNullOrWhiteSpace(date)) { return null; }
			string strSQL = @"Select * from EmployeePay Where EMPLOYECD = @EMPLOYECD and PAYYYYYMM = @PAYYYYYMM";
			var condition = new Conditions()
			{
				{"@EMPLOYECD", empID },
				{ "@PAYYYYYMM", date}
			};

			var result = _dc.QueryForDataRow(strSQL, condition);
			if (result == null) { return null; }

			return result;
		}
	}
}