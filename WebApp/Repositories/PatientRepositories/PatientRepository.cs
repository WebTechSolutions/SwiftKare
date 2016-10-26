using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using WebApp.Interfaces;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership.Models;

namespace WebApp.Repositories.PatientRepositories
{
    public class PatientRepository : IRepository<Patient>
    {
        public Patient Add(Patient t)
        {
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/Patients", strContent);
            var result = JsonConvert.DeserializeObject<Patient>(response);
            var userAssignRole = new UserAssignRoleModel();
            userAssignRole.UserId = t.userId;
            userAssignRole.Role = "Patient";
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

        public Patient Find(object id)
        {
            throw new NotImplementedException();
        }

        public Patient GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Patient GetByUserId(string userId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/Patients?userId=" + userId);
            var result=JsonConvert.DeserializeObject<Patient>(response);
            return result;
        }

        public IQueryable<Patient> GetList()
        {
            throw new NotImplementedException();
        }

        public Patient Put(long id, Patient t)
        {
            throw new NotImplementedException();
        }
    }
}
