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
    public class MessageRepository
    {
        /// <summary>
        /// Gets inbox messages
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GetMessageModel> GetInboxMessage()
        {
            try
            {
                string emailId = SessionHandler.UserInfo.Email;

                var request = ApiConsumerHelper.GetResponseString("api/getInboxMessages?email=" + emailId);
                var result = JsonConvert.DeserializeObject<IEnumerable<GetMessageModel>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        /// <summary>
        /// Gets message content based on message id
        /// </summary>
        /// <param name="msgID"></param>
        /// <returns></returns>
        public GetMessageModel GetMessageContent(long msgID)
        {
            try
            {
                string emailId = SessionHandler.UserInfo.Email;

                var request = ApiConsumerHelper.GetResponseString("api/getMessageContent?msgID=" + msgID);
                var result = JsonConvert.DeserializeObject<IEnumerable<GetMessageModel>>(request).FirstOrDefault();
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
