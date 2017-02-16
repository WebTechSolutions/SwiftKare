using System;
using System.Linq;
using DataAccess;
using WebApp.Interfaces;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership.Models;
using WebApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using DataAccess.CustomModels;
using Newtonsoft.Json.Linq;

namespace WebApp.Repositories.DoctorRepositories
{
    public class DoctorRepository : IRepository<Doctor>
    {
        public Doctor Add(Doctor t)
        {
            //By Default Active
            t.active = true;
            t.status = true;
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/Doctors", strContent,false);
            var result = JsonConvert.DeserializeObject<Doctor>(response);
            var userAssignRole = new UserAssignRoleModel();
            userAssignRole.UserId = t.userId;
            userAssignRole.Role = "Doctor";
            strContent = JsonConvert.SerializeObject(userAssignRole);
            response = ApiConsumerHelper.PostData("api/Roles/AssignRole", strContent);
            var resultAssignRole = JsonConvert.DeserializeObject(response);
            return result;
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(object id)
        {
            throw new NotImplementedException();
        }

        public Doctor Find(object id)
        {
            throw new NotImplementedException();
        }
        public IQueryable<Doctor> GetList()
        {
            throw new NotImplementedException();
        }
        public Doctor GetById(long id)
        {
            throw new NotImplementedException();
        }
        public DoctorModel GetByUserId(string userId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/Doctors?userId=" + userId,false);
            var result = JsonConvert.DeserializeObject<DoctorModel>(response);
            return result;
        }

        public long GetId(string userId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/Doctors/Id?userId=" + userId,false);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }

        public IQueryable<Doctor> GetLanguagesList()
        {
            throw new NotImplementedException();
        }

        public Doctor Put(long id, Doctor t)
        {
            throw new NotImplementedException();
        }
        public Object GetRefillErrorCount()
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetRefillErr");
                var result = JsonConvert.DeserializeObject<Object>(response);
                return result;
            }
            catch (HttpResponseException ex)
            {

                throw ex;
            }

        }
        public Object GetRefillUrl()
        {
            try
            {
                var response = ApiConsumerHelper.GetResponseString("api/GetRefillReqURL");
                var result = JsonConvert.DeserializeObject<Object>(response);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
   
}
