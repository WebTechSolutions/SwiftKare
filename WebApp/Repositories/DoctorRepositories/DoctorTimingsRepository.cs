using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Interfaces;
using DataAccess;
using WebApp.Helper;
using Newtonsoft.Json;

namespace WebApp.Repositories.DoctorRepositories
{
    class DoctorTimingsRepository : IRepository<DoctorTiming>
    {
        public DoctorTiming Add(DoctorTiming t)
        {
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/DoctorTimings", strContent);
            var result = JsonConvert.DeserializeObject<DoctorTiming>(response);
            return result;
        }

        public void Delete(long id)
        {
            var response = ApiConsumerHelper.DeleteData("api/DoctorTimings/"+id);
            var result = JsonConvert.DeserializeObject<DoctorTiming>(response);
        }

        public bool Exists(object id)
        {
            throw new NotImplementedException();
        }

        public DoctorTiming Find(object id)
        {
            throw new NotImplementedException();
        }

        public DoctorTiming GetById(long id)
        {
            var response = ApiConsumerHelper.GetResponseString("api/DoctorTimings/" + id);
            var result = JsonConvert.DeserializeObject<DoctorTiming>(response);
            return result;
        }

        public IQueryable<DoctorTiming> GetLanguagesList()
        {
            throw new NotImplementedException();
        }
        public IQueryable<DoctorTiming> GetListByDoctorId(long doctorId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/DoctorTimings?doctorId=" + doctorId);
            var result = JsonConvert.DeserializeObject<IQueryable<DoctorTiming>>(response);
            return result;
        }

        public DoctorTiming Put(long id, DoctorTiming t)
        {
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PutData("api/DoctorTimings/" + id, strContent);
            var result = JsonConvert.DeserializeObject<DoctorTiming>(response);
            return result;
            
        }
    }
}
