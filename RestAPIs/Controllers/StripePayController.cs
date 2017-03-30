using DataAccess;
using DataAccess.CustomModels;
using RestAPIs.Helper;
using RestAPIs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace RestAPIs.Controllers
{
    [Authorize]
    public class StripePayController : ApiController
    {
        HttpResponseMessage response;


        [HttpPost]
        [Route("api/performStripeCharge")]
        public HttpResponseMessage PerformStripeCharge(string tokenId, int amount, string desc = "")
        {
            var oResp =  StripePayHelper.PerformStripeCharge(tokenId, amount, desc);

            response = Request.CreateResponse(HttpStatusCode.OK, oResp.ToString());
            return response;
        }


        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            response.ReasonPhrase = ex.Message;
            return response;
        }
        protected override void Dispose(bool disposing)
        {
             base.Dispose(disposing);
        }
    }
}
