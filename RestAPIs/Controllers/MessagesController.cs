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
            try
            {
                Message email = new Message();
                string fromName = "";
                var docsender = (from doc in db.Doctors where doc.email == model.@from select new DoctorModel { firstName = doc.firstName, lastName = doc.lastName }).FirstOrDefault();
                var patsender = (from pat in db.Patients where pat.email == model.@from select new PatientModel { firstName = pat.firstName, lastName = pat.lastName }).FirstOrDefault();

            if(docsender!=null)
            {
                fromName = docsender.firstName + " " + docsender.lastName;
            }
            if (patsender != null)
            {
                fromName = patsender.firstName + " " + patsender.lastName;
            }
           
                if(model.to=="0")
                {
                    model.to = "support@swiftkare.com";
                }
               if(model.to == "" || model.to == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Reciever's email address is invalid." });
                    return response;
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

                email.cd = System.DateTime.Now;
                email.senderName = fromName;
                email.subject = model.subject;
                email.mesage = model.message;
                email.from = model.from;
                email.to = model.to;
                email.isRead = false;
                email.status = "1";
                email.replyLink = model.replyLink;
                if (model.msgFile != null)
                {
                    email.hasAttachment = true;
                }
                if (model.msgFile == null)
                {
                    email.hasAttachment = false;
                }
                db.Messages.Add(email);
                await db.SaveChangesAsync();
                if (model.msgFile!=null)
                {
                   
                    foreach (var item in model.msgFile)
                    {
                        MessageFile emailattch = new MessageFile();
                        emailattch.msgID = email.msgID;
                        emailattch.fileContent = item.fileContent;
                        emailattch.fileName = item.fileName;
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
                email.md = System.DateTime.Now;
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

        [Route("api/saveMessage")]
        public async Task<HttpResponseMessage> SaveMessage(MessageCustomModel model)
        {
            Message email = new Message();

            try
            {
                string fromName = "";
                var docsender = (from doc in db.Doctors where doc.email == model.@from select new DoctorModel { firstName = doc.firstName, lastName = doc.lastName }).FirstOrDefault();
                var patsender = (from pat in db.Patients where pat.email == model.@from select new PatientModel { firstName = pat.firstName, lastName = pat.lastName }).FirstOrDefault();

                if (docsender != null)
                {
                    fromName = docsender.firstName + " " + docsender.lastName;
                }
                if (patsender != null)
                {
                    fromName = patsender.firstName + " " + patsender.lastName;
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

                email.cd = System.DateTime.Now;
                email.senderName = fromName;
                email.subject = model.subject;
                email.mesage = model.message;
                email.from = model.from;
                email.to = model.to;
                email.isRead = true;
                email.status = "2";
                email.replyLink = model.replyLink;
                if (model.msgFile != null)
                {
                    email.hasAttachment = true;
                }
                if (model.msgFile == null)
                {
                    email.hasAttachment = false;
                }
                db.Messages.Add(email);
                await db.SaveChangesAsync();
                
                if (model.msgFile != null)
                {

                    foreach (var item in model.msgFile)
                    {
                        MessageFile emailattch = new MessageFile();
                        emailattch.msgID = email.msgID;
                        emailattch.fileContent = item.fileContent;
                        emailattch.fileName = item.fileName;
                        db.MessageFiles.Add(emailattch);
                        await db.SaveChangesAsync();
                    }
                }

                response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = email.msgID, message = "" });
                return response;

            }
            catch (Exception ex)
            {
                return ThrowError(ex, "SaveMessage in MessagesController.");
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
                email.md = System.DateTime.Now;
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
                if(!(IsValid(email)))
                    {
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid email address." });
                        return response;

                     }
                var mesgs = (from l in db.Messages
                                  where l.status != "3" && l.status != "2" && l.to == email
                                  orderby l.msgID descending
                                  select new MessageListModel
                                  {
                                      messageID = l.msgID,
                                      @from = l.@from,
                                      senderName=l.senderName,
                                      isRead = l.isRead,
                                      subject=l.subject,
                                      hasAttachment=l.hasAttachment,
                                      replyLink=l.replyLink,
                                      sentTime=l.cd
                                      
                                  }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetInboxMessages in MessagesController");
            }
        }
       

        [Route("api/getDraftMessages")]
        public HttpResponseMessage GetDraftMessages(string email)
        {
            try
            {
                if (!(IsValid(email)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid email address." });
                    return response;

                }
                var mesgs = (from l in db.Messages
                             where l.@from == email && l.status=="2"
                             orderby l.msgID descending
                             select new MessageListModel
                             {
                                 messageID = l.msgID,
                                 @from = l.@from,
                                 senderName = l.senderName,
                                 isRead = l.isRead,
                                 subject = l.subject,
                                 hasAttachment = l.hasAttachment,
                                 replyLink = l.replyLink,
                                 sentTime = l.cd

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
                if (!(IsValid(email)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid email address." });
                    return response;

                }
                var mesgs = (from l in db.Messages
                             where l.@from == email && l.status == "1"
                             orderby l.msgID descending
                             select new MessageListModel
                             {
                                 messageID = l.msgID,
                                 @from = l.@from,
                                 senderName = l.senderName,
                                 isRead = l.isRead,
                                 subject = l.subject,
                                 hasAttachment = l.hasAttachment,
                                 replyLink = l.replyLink,
                                 sentTime = l.cd

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
                if (!(IsValid(email)))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid email address." });
                    return response;

                }
                var mesgs = (from l in db.Messages
                             where l.to == email && l.status == "3"
                             orderby l.msgID descending
                             select new MessageListModel
                             {
                                 messageID = l.msgID,
                                 @from = l.@from,
                                 senderName = l.senderName,
                                 isRead = l.isRead,
                                 subject = l.subject,
                                 hasAttachment = l.hasAttachment,
                                 replyLink = l.replyLink,
                                 sentTime = l.cd

                             }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDeletedMessages in MessagesController");
            }
        }

        [Route("api/getMessageContent")]
        public HttpResponseMessage GetMessageContent(long msgID)
        {
            try
            {
                
                var mesgs = (from l in db.Messages
                             where l.status != "3" && l.status != "2" && l.msgID==msgID
                             orderby l.msgID descending
                             select new GetMessageModel
                             {
                                 msgID = l.msgID,
                                 message = l.mesage,
                                 senderName = l.senderName,
                                 subject = l.subject,
                                 sentTime = l.cd,
                                 @from=l.@from,
                                 msgFiles=(from mf in db.MessageFiles where mf.msgID==l.msgID select new MessageFileModel{ msgFileID=mf.msgFileID,msgID=mf.msgID,
                                     fileName = mf.fileName,fileContent=mf.fileContent })
                                 .ToList()

                             }).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, mesgs);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetInboxMessages in MessagesController");
            }
        }

        [Route("api/getDoctorEmails")]
        public HttpResponseMessage GetDoctorEmails(string search)
        {
            try
            {

                var result = (from doc in db.Doctors
                              where doc.email.ToLower().Contains(search.ToLower())
                              select doc.email).Distinct().Take(10);
                
                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetDoctorEmails in MessagesController");
            }
        }

        [Route("api/getPatientEmails")]
        public HttpResponseMessage GetPatientEmails(string search)
        {
            try
            {

                var result = (from pat in db.Patients
                              where pat.email.ToLower().Contains(search.ToLower())
                              select pat.email).Distinct().Take(10);

                response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientEmails in MessagesController");
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
