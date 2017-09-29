using RinnaiPortal.Models.ApiModels;
using RinnaiPortal.Repository;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RinnaiPortal.Service.Controllers.AdminService
{
    public class AdminServiceApiController : ApiController
    {
        private APIRepository m_aPIRepository = new APIRepository();

        private APIRepository APIServiceRepository { get { return this.m_aPIRepository; } }

        // GET api/adminserviceapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/adminserviceapi/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AuthenticateConfirm([FromBody]string pwd)
        {
            HttpResponseMessage resultResponse = null;

            if (pwd != null)
            {
                if (pwd == "0000")
                {
                    resultResponse = APIRepository.CreateDataResponse(Request, HttpStatusCode.OK, "success");
                }
                else
                {
                    resultResponse = APIRepository.CreateDataResponse(Request, HttpStatusCode.BadRequest, "Fail");
                }
            }
            return resultResponse;
        }


        // POST api/adminserviceapi
        /// <summary>
        /// 取得所有員工資料
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public HttpResponseMessage GetEmployeesData([FromBody]string value)
        {
            HttpResponseMessage resultResponse = null;

            List<EmpDataModel> empData = this.APIServiceRepository.GetEmployeesData();

            resultResponse = APIRepository.CreateDataResponse(Request, HttpStatusCode.OK, empData);

            return resultResponse;
        }

        // PUT api/adminserviceapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/adminserviceapi/5
        public void Delete(int id)
        {
        }
    }
}