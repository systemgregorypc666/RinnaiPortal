using DBTools;
using Newtonsoft.Json.Serialization;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Models.ApiModels;
using RinnaiPortal.Tools;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace RinnaiPortal.Repository
{
    public class APIRepository
    {
        /// <summary>
        /// 取得所有員工資料
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public List<EmpDataModel> GetEmployeesData()
        {
            List<EmpDataModel> empList = new List<EmpDataModel>();
            var empDc = ConnectionFactory.GetPortalDC();
            var empRepos = new EmployeeRepository(empDc, new RootRepository(empDc));

            EmpDataModel model = null;
            string strSQL = @"Select * from Employee ";
            DataTable resultDataTable = empDc.QueryForDataTable(strSQL, null);
            DataRow[] DataRows = resultDataTable.Select();

            foreach (var row in DataRows)
            {
                model = new EmpDataModel()
               {
                   EmployeeID = row["EmployeeID"].ToString(),
                   EmployeeName = row["EmployeeName"].ToString(),
                   DepartmentID = row["DepartmentID_FK"].ToString(),
                   EmployeeADAccount = row["ADAccount"].ToString(),
                   NationalType = row["NationalType"].ToString(),
               };
                empList.Add(model);
            }
            return empList;
        }

        /// <summary>
        /// 輸出回傳結果 回傳型別HttpResponseMessage物件
        /// </summary>
        /// <typeparam name="T">輸出的物件類型</typeparam>
        /// <param name="request"></param>
        /// <param name="statusCode">HTTP狀態碼</param>
        /// <param name="data">輸出的資料</param>
        /// <returns></returns>
        internal static HttpResponseMessage CreateDataResponse<T>(HttpRequestMessage request, HttpStatusCode statusCode, T data)
        {
            HttpResponseMessage respMsg;
            MediaTypeFormatter mediaFormatter = null;
            mediaFormatter = new JsonMediaTypeFormatter();
            (mediaFormatter as JsonMediaTypeFormatter).SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            (mediaFormatter as JsonMediaTypeFormatter).SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            respMsg = request.CreateResponse(statusCode);
            respMsg.Content = new ObjectContent<T>(data, mediaFormatter);
            return respMsg;
        }
    }
}