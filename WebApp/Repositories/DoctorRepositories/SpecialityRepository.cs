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

namespace WebApp.Repositories.DoctorRepositories
{
    public class SpecialityRepository : IRepository<Speciallity>
    {

        public Speciallity Add(Speciallity t)
        {
            
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/Speciallities", strContent);
            var result = JsonConvert.DeserializeObject<Speciallity>(response);

            return result;
        }
        public IQueryable<Speciallity> GetList()
        {
            throw new NotImplementedException();
        }
        public IQueryable<Speciallity> GetSpeciallities()
        {
            throw new NotImplementedException();
        }
        public void Delete(long id)
        {
            throw new NotImplementedException();
        }
        public bool Exists(object id)
        {
            throw new NotImplementedException();
        }

        public Speciallity Find(object id)
        {
            throw new NotImplementedException();
        }

        public Speciallity GetById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Speciallity> Get()
        {
            var response = ApiConsumerHelper.GetResponseString("api/Speciallities");
            var result = JsonConvert.DeserializeObject<List<Speciallity>>(response);
            // var result = JsonConvert.DeserializeObject<Language>(response);
            return result;
        }

        public Speciallity Put(long id, Speciallity t)
        {
            throw new NotImplementedException();
        }
    }
}