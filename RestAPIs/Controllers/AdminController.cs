using RestAPIs.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess;
using DataAccess.CustomModels;

namespace RestAPIs.Controllers
{
    public class AdminController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/Admin")]
        [ResponseType(typeof(AdminModel))]
        public AdminModel GetAdminByUserId(string userId, HttpRequestMessage request)
        {
            if (!request.IsValidClient())
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Unauthorized, Client is not valid"),
                    ReasonPhrase = "Bad Request"
                };
                throw new HttpResponseException(resp);
            }
            try
            {

                AdminUser adminuser = db.AdminUsers.SingleOrDefault(o => o.userId == userId);
                if (adminuser == null)
                    return null;

                var objModel = new AdminModel();
                objModel.adminID = adminuser.adminID;
                objModel.firstName = adminuser.FirstName;
                objModel.lastName = adminuser.lastName;
                objModel.userId = adminuser.userId;
                objModel.email = adminuser.email;
                objModel.active = adminuser.active;
                return objModel;
            }

            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }
    }
}
