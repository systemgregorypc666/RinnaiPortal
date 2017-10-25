using Newtonsoft.Json;
using RinnaiPortal.Entities;
using RinnaiPortalOpenApi.Models;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceDetalisModels;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Modules;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace RinnaiPortalOpenApi.Repositories
{
    public class EinvoiceApRepository
    {
        private EinvoiceModule m_module = new EinvoiceModule();
        private EinvoiceModule module { get { return m_module; } set { m_module = value; } }

        private APIRepository m_APIRepository = new APIRepository();
        private APIRepository Repository { get { return m_APIRepository; } set { m_APIRepository = value; } }

        public EinvoiceWinningNumberResultModel GetEinvoiceWinningNumbers(string invTerm)
        {
            EinvoiceWinningNumberSendModel send = new EinvoiceWinningNumberSendModel() { action = "QryWinningList", invTerm = invTerm };

            string resultStr = string.Empty;
            WebRequest webRequest = null;
            string sendData = this.Repository.ConvertObjectToQueryString(send);

            byte[] bytes = Encoding.UTF8.GetBytes(sendData);
            //Request
            try
            {
                webRequest = this.Repository.CreateWebRequest(ConnectionStringModel.EinvoiceSearchApiConnectionStr);
                webRequest.ContentLength = bytes.Length;
                this.Repository.SendWebRequest(webRequest, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Get Response
            try
            {
                resultStr = this.Repository.GetResponse(webRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            EinvoiceWinningNumberResultModel result = JsonConvert.DeserializeObject<EinvoiceWinningNumberResultModel>(resultStr);
            return result;
        }

        /// <summary>
        /// 取得訂單的發票號碼
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public string GetEinvoiceNoByOrderNo(string orderNo)
        {
            string einvoiceNo = string.Empty;
            ERPDB db = new ERPDB();
            var order = db.Rinnai_Service_Ledger_Entry.Where(o => o.Service_Order_No_ == orderNo && o.Document_Type == 2).FirstOrDefault();
            if (order == null)
                throw new Exception("[系統]無法取得該訂單相關資料");
            var orderDealis = db.Rinnai_Sales_Invoice_Line.Where(o => o.Document_No_ == order.Document_No_ && o.VAT_Transaction_Number != "").FirstOrDefault();
            if (orderDealis == null)
                throw new Exception("[系統]無法取得該訂單相關資料");
            einvoiceNo = orderDealis.VAT_Transaction_Number;
            return einvoiceNo;
        }

        public EinvoiceDetalisResultModel GetEinvoiceDetalisByNo(string invNo)
        {
            invNo = string.Concat(invNo[0], invNo[1]).ToUpper() + invNo.Substring(2, invNo.Length - 2);
            EinvoiceDetalisSendModel send = new EinvoiceDetalisSendModel();

            #region 取發票原始檔明細 測試時請註解 正式請取消註解

            send = GetEinvoiceProtoDetalisByNo(invNo);

            #endregion 取發票原始檔明細 測試時請註解 正式請取消註解

            send.version = 0.3;
            send.type = "Barcode";
            send.invNum = invNo;
            send.action = "qryInvDetail";
            send.generation = "V2";

            #region 測試時請取消註解並給時寄發票的資訊 正式請註解

            //send.invTerm =  "10610";
            //send.invDate = "2017/09/26";
            //send.randomNumber = "6157";

            #endregion 測試時請取消註解並給時寄發票的資訊 正式請註解

            string resultStr = string.Empty;
            WebRequest webRequest = null;
            string sendData = this.Repository.ConvertObjectToQueryString(send);

            byte[] bytes = Encoding.UTF8.GetBytes(sendData);
            //Request
            try
            {
                webRequest = this.Repository.CreateWebRequest(ConnectionStringModel.EinvoiceSearchApiConnectionStr);
                webRequest.ContentLength = bytes.Length;
                this.Repository.SendWebRequest(webRequest, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Get Response
            try
            {
                resultStr = this.Repository.GetResponse(webRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            EinvoiceDetalisResultModel result = JsonConvert.DeserializeObject<EinvoiceDetalisResultModel>(resultStr);
            return result;
        }

        /// <summary>
        /// 取得電子發票原始檔明細
        /// </summary>
        /// <param name="invNo"></param>
        /// <returns></returns>
        private EinvoiceDetalisSendModel GetEinvoiceProtoDetalisByNo(string invNo)
        {
            invNo = string.Concat(invNo[0], invNo[1]).ToUpper() + invNo.Substring(2, invNo.Length - 2);
            EinvoiceDetalisSendModel send = new EinvoiceDetalisSendModel();
            try
            {
                C0401H einvoice = this.module.GetEinvoiceDetalisByNo(invNo);
                if (einvoice == null)
                    throw new Exception("[系統]查無相關電子發票原始檔明細明細");
                int getMonth = Convert.ToInt16(einvoice.MInvoiceDate.Substring(4, 2));
                var chkMonth = (getMonth % 2) == 1;
                string invTerm = string.Format("{0}{1}", einvoice.MInvoiceDate.Substring(0, 4), chkMonth ? (getMonth + 1).ToString().PadLeft(2, '0') : (getMonth).ToString().PadLeft(2, '0'));
                string y = einvoice.MInvoiceDate.Substring(0, 4);
                string m = einvoice.MInvoiceDate.Substring(4, 2);
                string d = einvoice.MInvoiceDate.Substring(6, 2);
                string date = string.Format("{0}/{1}/{2}", y, m, d);
                send.invTerm = invTerm;
                send.invDate = date;
                send.randomNumber = einvoice.MRandomNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return send;
        }
    }
}