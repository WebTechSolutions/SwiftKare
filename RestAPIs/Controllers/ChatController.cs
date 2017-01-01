using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class ChatController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        private HttpResponseMessage response;

        [Route("api/getChatMessages")]
        public HttpResponseMessage GetChatMessages(long consultID)
        {
            try
            {
                var chatmsgs = (from l in db.ChatLogs
                              where l.consultID == consultID
                              select new { chatID = l.chatID, sender = l.sender, reciever = l.reciever, message = l.message, sentDate=l.cd }).ToList();

                response = Request.CreateResponse(HttpStatusCode.OK, chatmsgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetChatMessages in ChatController");
            }


        }

        private bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("api/addChatMessages")]
        public async Task<HttpResponseMessage> AddChatMessages(ChatMessageModel model)
        {
            ChatLog chatLog = new ChatLog();
            try
            {
                if (model.consultID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid consult ID." });
                    return response;
                }

                //if (model.sender == "" || model.sender == null)//|| !(IsValid(model.sender)
                //{
                //    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for sender." });
                //    return response;
                //}

                if (model.reciever == "" || model.reciever == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Provide valid email for reciever." });
                    return response;
                }
                if (model.message == "" || model.message == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Blank message is not allowed." });
                    return response;
                }

                chatLog.consultID = model.consultID;
                chatLog.cd = System.DateTime.Now;
                chatLog.sender = model.sender;
                chatLog.reciever = model.reciever;
                chatLog.message = model.message;
                db.ChatLogs.Add(chatLog);
                await db.SaveChangesAsync();
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = chatLog.chatID, message = "" });
                return response;
             
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddChatMessages in ChatController");
            }
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            return response;

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
