using DataAccess.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApp.Helper;

namespace WebApp.Repositories.AdminRepository
{
    public class AdminRepository
    {
        public AdminModel GetByUserId(string userId)
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/Admin?userId=" + userId, false);
                var result = JsonConvert.DeserializeObject<AdminModel>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
           
        }
    }
}