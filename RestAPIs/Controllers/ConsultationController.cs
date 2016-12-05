using DataAccess;
using DataAccess.CommonModels;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;


namespace RestAPIs.Controllers
{
    public class ConsultationController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/GetPatientConsultations")]
        public HttpResponseMessage GetPatientConsultations(long patientID)
        {
            try
            {
                var result = db.SP_GetPatientConsultations(patientID);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientConsultations in ConsultationController");
            }


        }

        [Route("api/GetDcotorConsultations")]
        public HttpResponseMessage GetDcotorConsultations(long doctorID)
        {
            try
            {
                var result = db.SP_GetDcotorConsultations(doctorID);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DcotorConsultations in ConsultationController");
            }


        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            return response;

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
