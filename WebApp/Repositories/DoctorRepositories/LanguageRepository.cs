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
using System.Collections;

namespace WebApp.Repositories.DoctorRepositories
{
   

   
        public class LanguageRepository : IRepository<Language>
    {

        public Language Add(Language t)
        {
           
            var strContent = JsonConvert.SerializeObject(t);
            var response = ApiConsumerHelper.PostData("api/Languages", strContent);
            var result = JsonConvert.DeserializeObject<Language>(response);
           
            return result;
        }
        public void Delete(long id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Language> Get()
        {
            var response = ApiConsumerHelper.GetResponseString("api/Languages");
            var result = JsonConvert.DeserializeObject<List<Language>>(response);
           // var result = JsonConvert.DeserializeObject<Language>(response);
            return result;
        }
        public IQueryable<Language> GetLanguages()
        {
            throw new NotImplementedException();
        }
       

        public bool Exists(object id)
            {
                throw new NotImplementedException();
            }

            public Language Find(object id)
            {
                throw new NotImplementedException();
            }

            public Language GetById(long id)
            {
                throw new NotImplementedException();
            }

            public Language GetByUserId(string userId)
            {
                var response = ApiConsumerHelper.GetResponseString("api/Languages?userId=" + userId);
                var result = JsonConvert.DeserializeObject<Language>(response);
                return result;
            }

        public IQueryable<Language> GetLanguages(long langId)
        {
            var response = ApiConsumerHelper.GetResponseString("api/Languages?languageId=" + langId);
            var result = JsonConvert.DeserializeObject<IQueryable<Language>>(response);
            return result;
        }

        public Language Put(long id, Language t)
            {
                throw new NotImplementedException();
            }
        public IQueryable<Language> GetList()
        {
            throw new NotImplementedException();
        }


    }
    }

