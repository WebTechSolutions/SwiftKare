using DataAccess;
using DataAccess.CustomModels;
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
    public class TokBoxController : ApiController
    {
        HttpResponseMessage response;

        [HttpGet]
        [Route("api/generateOpenTokSession")]
        public HttpResponseMessage GenerateOpenTokSession()
        {
            try
            {
                var openTokSession = UserChatHelper.GenerateOpenTokSession();

                response = Request.CreateResponse(HttpStatusCode.OK, openTokSession);
                
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "generateOpenTokSession in TokBoxController");
            }
        }

        [HttpGet]
        [Route("api/generateOpenTokToken")]
        public HttpResponseMessage GenerateOpenTokToken(string sessionId)
        {
            try
            {
                var openTokToken = UserChatHelper.GenerateOpenTokToken(sessionId);

                response = Request.CreateResponse(HttpStatusCode.OK, openTokToken);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GenerateOpenTokToken in GenerateOpenTokToken");
            }
        }

        [HttpGet]
        [Route("api/generateOpenTokSessionWeb")]
        public HttpResponseMessage GenerateOpenTokSessionWeb()
        {
            try
            {
                var openTokSession = UserChatHelper.GenerateOpenTokSessionWeb();

                response = Request.CreateResponse(HttpStatusCode.OK, openTokSession);

                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "generateOpenTokSessionWeb in TokBoxController");
            }
        }

        [HttpGet]
        [Route("api/generateOpenTokTokenWeb")]
        public HttpResponseMessage GenerateOpenTokTokenWeb(string sessionId)
        {
            try
            {
                var openTokToken = UserChatHelper.GenerateOpenTokTokenWeb(sessionId);

                response = Request.CreateResponse(HttpStatusCode.OK, openTokToken);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GenerateOpenTokTokenWeb in TokBoxController");
            }
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
            return response;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
