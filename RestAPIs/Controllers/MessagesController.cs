using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class MessagesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;
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
        //private Utility util = new Utility();
        [Route("api/sendMessage")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> SendMessage(MessageCustomModel model)
        {
            Message email = new Message();

            try
            {
               if(model.to == "" || model.to == null)
                {
                    model.to = "support@swiftkare.com";
                }
                if (!(IsValid(model.to)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Reciever's email address is invalid." });
                    return response;
                }
                if (model.from == null || model.from == "" || !(IsValid(model.from)))
                    
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Sender's email address is invalid." });
                    return response;
                }
                if (model.message == null || model.message == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Empty message is not allowed." });
                    return response;
                }
                
                email.mesage = model.message;
                email.from = model.from;
                email.to = model.to;
                email.isRead = false;
                email.status = "1";
                email.replyLink = model.replyLink;
                db.Messages.Add(email);
                await db.SaveChangesAsync();
                //byte[] cover = null;
                //var img = from temp in db.News where temp.newsID == 1 select temp.newsThumbnail;
                //cover = img.First();
                
                //model.msgFile[0].fileContent = cover;
                //model.msgFile[1].fileContent = cover;
                //model.msgFile[2].fileContent = cover;
                if (model.msgFile!=null)
                {
                   
                    foreach (var item in model.msgFile)
                    {
                        MessageFile emailattch = new MessageFile();
                        emailattch.msgID = email.msgID;
                        emailattch.fileContent = item.fileContent;
                        db.MessageFiles.Add(emailattch);
                        await db.SaveChangesAsync();
                    }
                }
                
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = email.msgID, message = "" });
                return response;
             
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "SendMessage in MessagesController.");
            }
            


        }

        [Route("api/readMessage")]
        public async Task<HttpResponseMessage> ReadMessage(long msgID)
        {
            try
            {
                Message email = new Message();
                if (msgID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid message ID." });
                    return response;
                }
                email = db.Messages.Where(msg => msg.msgID == msgID).FirstOrDefault();

                if (email == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Message not found." });
                    return response;
                }
                email.isRead = true;
                db.Entry(email).State = EntityState.Modified;
                await db.SaveChangesAsync();
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = msgID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "ReadMessage in MessagesController.");
            }


        }

        [Route("api/deleteMessage")]
        public async Task<HttpResponseMessage> DeleteMessage(long msgID)
        {
            try
            {
                Message  email = new Message();
                if (msgID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid message ID." });
                    return response;
                }
                email = db.Messages.Where(msg => msg.msgID == msgID).FirstOrDefault();
               
                if (email == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Message not found." });
                    return response;
                }
                email.status = "3";
                db.Entry(email).State = EntityState.Modified;
                await db.SaveChangesAsync();
                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = msgID, message = "" });
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeleteMessage in MessagesController.");
            }

            
        }
        [Route("api/getInboxMessages")]
        public HttpResponseMessage GetInboxMessages(string email)
        {
            try
            {
                var mesgs = (from l in db.Messages
                                  where l.status != "3" && l.to == email
                                  orderby l.msgID descending
                                  select new GetMessageModel
                                  {
                                      msgID = l.msgID,
                                      @from = l.@from,
                                      to = l.to,
                                      isRead = l.isRead,
                                      message=l.mesage,
                                      replyLink=l.replyLink,
                                      status=l.status,
                                      msgFiles = db.MessageFiles.Where(f=>f.msgID==l.msgID).ToList()

                                  }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetInboxMessages in MessagesController");
            }
            //
        }
       

        [Route("api/getDraftMessages")]
        public HttpResponseMessage GetDraftMessages(string email)
        {
            try
            {
                var mesgs = (from l in db.Messages
                             where l.@from == email && l.status=="2"
                             orderby l.msgID descending
                             select new GetMessageModel
                             {
                                 msgID = l.msgID,
                                 @from = l.@from,
                                 to = l.to,
                                 isRead = l.isRead,
                                 message = l.mesage,
                                 replyLink = l.replyLink,
                                 status = "Draft",
                                 msgFiles = db.MessageFiles.Where(f=>f.msgID==l.msgID).ToList()
                             }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDraftMessages in MessagesController");
            }
        }

        [Route("api/getSentMessages")]
        public HttpResponseMessage GetSentMessages(string email)
        {
            try
            {
                var mesgs = (from l in db.Messages
                             where l.@from == email && l.status == "1"
                             orderby l.msgID descending
                             select new GetMessageModel
                             {
                                 msgID = l.msgID,
                                 @from = l.@from,
                                 to = l.to,
                                 message = l.mesage,
                                 replyLink = l.replyLink,
                                 status ="Sent",
                                 msgFiles = db.MessageFiles.Where(f => f.msgID == l.msgID).ToList()
                             }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetSentMessages in MessagesController");
            }
        }

        [Route("api/getDeletedMessages")]
        public HttpResponseMessage GetDeletedMessages(string email)
        {
            try
            {
                var mesgs = (from l in db.Messages
                             where l.to == email && l.status == "3"
                             orderby l.msgID descending
                             select new GetMessageModel
                             {
                                 msgID = l.msgID,
                                 @from = l.@from,
                                 to = l.to,
                                 message = l.mesage,
                                 isRead=l.isRead,
                                 replyLink = l.replyLink,
                                 status = "Deleted",
                                 msgFiles = db.MessageFiles.Where(f => f.msgID == l.msgID).ToList()
                             }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDeletedMessages in MessagesController");
            }
        }


        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {

            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
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
