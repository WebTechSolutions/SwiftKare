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

namespace WebApp.Repositories.ProfileRepositories
{
    public class NewsRepository
    {
        public List<NewsVM> GetNewsList()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getNewsList");
                var result = JsonConvert.DeserializeObject<List<NewsVM>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public NewsVM GetNewsDetail(long newsId)
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getNewsDetail?newsID=" + newsId);
                var result = JsonConvert.DeserializeObject<NewsVM>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

    }
}