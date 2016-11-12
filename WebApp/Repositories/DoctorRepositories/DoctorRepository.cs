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
        //public IEnumerable<SeeDoctorDTO> SeeDoctor(string docName, string gender, string langName, string specName, string day, TimeSpan time)
        //{
        //    var response = ApiConsumerHelper.GetResponseString("api/searchDoctor/docName/?docName=" + docName);// + "&gender=" + gender + "&langName=" + langName + "&specName=" + specName +
        //        //"&day=" + day + "&time=" + time);

        //    var result = JsonConvert.DeserializeObject<IEnumerable<SeeDoctorDTO>>(response);
        //    return result;
        //}
        public IEnumerable<Doctor> SeeDoctor(SearchModel searchModel)
        {

            //var response = ApiConsumerHelper.GetResponseString("api/searchDoctor/searchModel/?searchModel=" + searchModel);
            //var result = JsonConvert.DeserializeObject<IList<SeeDoctorDTO>>(response);
            try
            {
                var strContent = JsonConvert.SerializeObject(searchModel);
                var response = ApiConsumerHelper.PostData("api/searchDoctor/searchModel/?searchModel", strContent);
                var result = JsonConvert.DeserializeObject<IEnumerable<Doctor>>(response);
                return result;
            }
            catch(Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
           
        }
    }
   
}
