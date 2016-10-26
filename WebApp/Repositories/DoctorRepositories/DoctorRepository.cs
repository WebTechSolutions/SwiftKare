using System;
using System.Linq;
using DataAccess;
using WebApp.Interfaces;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership.Models;

namespace WebApp.Repositories.DoctorRepositories
{
    public class DoctorRepository : IRepository<Doctor>
    {
        public Doctor Add(Doctor t)
        {
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/Doctors", strContent);
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

        public Doctor GetById(long id)
        {
            throw new NotImplementedException();
        }
        public Doctor GetByUserId(string userId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/Doctors?userId=" + userId);
            var result = JsonConvert.DeserializeObject<Doctor>(response);
            return result;
        }

        public long GetId(string userId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/Doctors/Id?userId=" + userId);
            var result = JsonConvert.DeserializeObject<long>(response);
            return result;
        }

        public IQueryable<Doctor> GetList()
        {
            throw new NotImplementedException();
        }

        public Doctor Put(long id, Doctor t)
        {
            throw new NotImplementedException();
        }
    }
}
