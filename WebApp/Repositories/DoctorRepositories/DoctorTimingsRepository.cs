using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Interfaces;
using DataAccess;
using WebApp.Helper;
using Newtonsoft.Json;
using DataAccess.CustomModels;

namespace WebApp.Repositories.DoctorRepositories
{
    class DoctorTimingsRepository : IRepository<DoctorTimingsModel>
    {
        public DoctorTimingsModel Add(DoctorTimingsModel t)
        {
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/DoctorTimings", strContent);
            var result = JsonConvert.DeserializeObject<DoctorTimingsModel>(response);
            return result;
        }

        public void Delete(long id)
        {
            var response = ApiConsumerHelper.DeleteData("api/DoctorTimings/"+id);
            var result = JsonConvert.DeserializeObject<DoctorTimingsModel>(response);
        }

        public bool Exists(object id)
        {
            throw new NotImplementedException();
        }

        public DoctorTimingsModel Find(object id)
        {
            throw new NotImplementedException();
        }

        public DoctorTimingsModel GetById(long id)
        {
            var response = ApiConsumerHelper.GetResponseString("api/DoctorTimings/" + id);
            var result = JsonConvert.DeserializeObject<DoctorTimingsModel>(response);
            return result;
        }

        public IQueryable<DoctorTiming> GetList()
        {
            throw new NotImplementedException();
        }
        public List<DoctorTimingsModel> GetListByDoctorId(long doctorId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/DoctorTimings?doctorId=" + doctorId);
            var result = JsonConvert.DeserializeObject<List<DoctorTimingsModel>>(response);
            return result;
        }

        public DoctorTimingsModel Put(long id, DoctorTimingsModel t)
        {
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PutData("api/DoctorTimings/" + id, strContent);
            var result = JsonConvert.DeserializeObject<DoctorTimingsModel>(response);
            return result;
            
        }

        IQueryable<DoctorTimingsModel> IRepository<DoctorTimingsModel>.GetList()
        {
            throw new NotImplementedException();
        }
    }
}
