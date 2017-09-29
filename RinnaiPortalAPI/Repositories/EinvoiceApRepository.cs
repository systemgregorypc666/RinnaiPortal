using Newtonsoft.Json;
using RinnaiPortalAPI.Entities;
using RinnaiPortalOpenApi.Models;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceDetalisModels;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Modules;
using System;
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

        public EinvoiceDetalisResultModel GetEinvoiceDetalisByNo(string invNo)
        {
            invNo = string.Concat(invNo[0], invNo[1]).ToUpper() + invNo.Substring(2, invNo.Length - 2);
            EinvoiceDetalisSendModel send = GetEinvoiceProtoDetalisByNo(invNo);
            send.version = 0.3;
            send.type = "Barcode";
            send.invNum = invNo;
            send.action = "qryInvDetail";
            send.generation = "V2";
            //invTerm = "10610",
            //invDate = "2017/09/26",
            //randomNumber = "6157",

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

        private EinvoiceDetalisSendModel GetEinvoiceProtoDetalisByNo(string invNo)
        {
            invNo = string.Concat(invNo[0], invNo[1]).ToUpper() + invNo.Substring(2, invNo.Length - 2);
            EinvoiceDetalisSendModel send = new EinvoiceDetalisSendModel();
            try
            {
                C0401H einvoice = this.module.GetEinvoiceDetalisByNo(invNo);
                if (einvoice == null)
                    throw new Exception("查無相關發票明細");
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