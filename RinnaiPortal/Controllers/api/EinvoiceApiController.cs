using RinnaiPortal.Repository;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceDetalisModels;
using RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels;
using RinnaiPortalOpenApi.Repositories;
using System;
using System.Net.Http;
using System.Web.Http;

namespace RinnaiPortalOpenApi.Controllers
{
    public class EinvoiceApiController : ApiController
    {
        private EinvoiceApRepository m_repository = new EinvoiceApRepository();
        private EinvoiceApRepository Repository { get { return m_repository; } set { this.m_repository = value; } }

        /// <summary>
        /// 查詢統一發票中獎名單
        /// </summary>
        /// <param name="invTerm">查詢月份，需為雙數 ex.10608</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetEinvoiceWinningNumber([FromUri] string invTerm)
        {
            EinvoiceWinningNumberResultModel result = this.Repository.GetEinvoiceWinningNumbers(invTerm);
            return RinnaiPortalOpenApi.Repositories.APIRepository.CreateDataResponse(Request, System.Net.HttpStatusCode.OK, result); ;
        }

        /// <summary>
        /// 查詢統一發票明細
        /// </summary>
        /// <param name="invTerm">查詢月份，需為雙數 ex.10608</param>
        /// <returns></returns>
        [HttpGet]
        public EinvoiceDetalisResultModel GetEinvoiceDetalisByOrderNo([FromUri] string orderNo)
        {
            EinvoiceDetalisResultModel result = new EinvoiceDetalisResultModel();
            try
            {
                string invNo = this.Repository.GetEinvoiceNoByOrderNo(orderNo);

                #region 測試資料請給真實發票號碼做測試

                //string invNo = "XC47895844";

                #endregion 測試資料請給真實發票號碼做測試

                result = this.Repository.GetEinvoiceDetalisByNo(invNo);
            }
            catch (Exception ex)
            {
                var now = DateTime.UtcNow.AddHours(8);
                string dd = now.Year + now.Month.ToString().PadLeft(2, '0') + now.Day.ToString().PadLeft(2, '0');
                string exInfo = PublicRepository.ExceptionDetalisMessages(ex);
                PublicRepository.SaveMesagesToTextFile(@"D:\OrderEinvSearchLog\", orderNo + "_" + dd + ".txt", exInfo);
                result.msg = ex.Message;
                result.code = "1";
            }
            return result;
        }
    }
}