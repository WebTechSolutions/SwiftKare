using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class MyCareTeamController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;

        [Route("api/getMyCareTeam")]
        public HttpResponseMessage GetMyCareTeam(long patientID)
        {
            try
            {
                var favdoc = (from l in db.FavouriteDoctors
                              where l.patientID == patientID && l.active == true
                              select (from doc in db.Doctors
                                      where doc.doctorID == l.doctorID && doc.active == true
                                      select new { doctorID = doc.doctorID, firstName = doc.firstName, lastName = doc.lastName }).ToList()
                              );

                response = Request.CreateResponse(HttpStatusCode.OK, favdoc);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetMyCareTeam in SearchDoctorController");
            }


        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            response.ReasonPhrase = ex.Message;
            return response;
        }
    }
}
