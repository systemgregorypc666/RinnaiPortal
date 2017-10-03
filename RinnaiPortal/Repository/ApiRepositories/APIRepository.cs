using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;

using RinnaiPortalOpenApi.Models.EinvoiceApiModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace RinnaiPortalOpenApi.Repositories
{
    public class APIRepository
    {

        /// <summary>
        /// 物件轉成queryString參數
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ConvertObjectToQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
            // queryString will be set to "Id=1&State=26&Prefix=f&Index=oo"                  
            string queryString = String.Join("&", properties.ToArray());
            return queryString;
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


        /// <summary>
        /// 建立WebResquest物件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public WebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Timeout = 60 * 1000;
            webRequest.Method = WebRequestMethods.Http.Post; ;
            webRequest.KeepAlive = true;
            //webRequest.ContentType = "application/json;charset=utf-8";
            //charset=UTF-8";
            //webRequest.ContentType = "application/x-www-form-urlencoded";
            //webRequest.Timeout = 150000;
            //webRequest.ServerCertificateValidationCallback = delegate { return true; };
            return webRequest;
        }

        /// <summary>
        /// 設定WebRequest物件
        /// </summary>
        /// <param name="req"></param>
        /// <param name="bytes"></param>
        public void SendWebRequest(WebRequest req, byte[] bytes)
        {
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }
        }


        /// <summary>
        /// 取回WebRequest之Response
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetResponse(WebRequest req)
        {
            string result = string.Empty;
            using (WebResponse response = req.GetResponse())
            {
                //System.Diagnostics.Debug.WriteLine(((HttpWebResponse)response).StatusDescription);
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }
    }
}