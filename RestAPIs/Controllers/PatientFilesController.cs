using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class PatientFilesController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getFileTypes")]
        public HttpResponseMessage GetFileType()
        {
            try
            {
                var files = (from f in db.FileTypes where f.active==true select new { f.fileTypeID,f.typeName}).ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, files);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetFileTypes in PatientFilesController");
            }
        }

        [Route("api/getPatientFiles")]
        public HttpResponseMessage GetPatientFiles(long patientID)
        {
            try
            {
                var files = (from l in db.UserFiles
                             where l.active == true && l.patientID == patientID
                             orderby l.fileID descending
                             select new GetPatientUserFiles
                             {
                                 fileID = l.fileID,
                                 patientID = l.patientID,
                                 doctorID = l.doctorID,
                                 FileName = l.FileName.Trim(),
                                 fileContent = null,
                                 documentType =l.documentType,
                                 cd =l.md
                             }).ToList();
                foreach (var item in files)
                {
                    if(item.cd!=null)
                    item.createdDate = item.cd.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm tt");
                }

                response = Request.CreateResponse(HttpStatusCode.OK, files);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientFiles in PatientFilesController");
            }
        }

        [Route("api/getPatientFile")]
        public HttpResponseMessage GetPatientFile(long patientID, long fileId)
        {
            try
            {
                var files = (from l in db.UserFiles
                             where l.active == true && l.patientID == patientID && l.fileID == fileId
                             orderby l.fileID descending
                             select new GetPatientUserFiles
                             {
                                 fileID = l.fileID,
                                 patientID = l.patientID,
                                 doctorID = l.doctorID,
                                 FileName = l.FileName.Trim(),
                                 fileContent = l.fileContent,
                                 documentType = l.documentType,
                                 cd = l.md
                             }).FirstOrDefault();
                
                    if (files.cd != null)
                    files.createdDate = files.cd.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm tt");
                
                response = Request.CreateResponse(HttpStatusCode.OK, files);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetPatientFiles in PatientFilesController");
            }
        }


        [Route("api/addPatientFile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> AddPatientFiles(FilesCustomModel model)
        {
            UserFile patfile = new UserFile();
            try
            {

                //if (model.FileName == null || model.FileName == "" || !Regex.IsMatch(model.FileName.Trim(), "^[0-9a-zA-Z ]+$"))
                //{
                //    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid file name. Only letters and numbers are allowed." });
                //    return response;
                //}
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
              /*  if (model.doctorID == null || model.doctorID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid doctor ID." });
                    return response;
                }*/
                if (model.fileContent == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "File is empty. " });
                    return response;
                }
                if (model.documentType == null || model.documentType == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Please provide document type. " });
                    return response;
                }
               
                patfile = db.UserFiles.Where(m => m.FileName == model.FileName.Trim() && m.active == true).FirstOrDefault();
                if (patfile == null)
                {
                    patfile = new UserFile();
                    patfile.active = true;
                    patfile.FileName = model.FileName;
                    patfile.patientID = model.patientID;
                    patfile.cd = System.DateTime.Now;
                    patfile.md= System.DateTime.Now;
                  //  patfile.doctorID = model.doctorID == -1 ? null : model.doctorID;
                    patfile.fileContent = model.fileContent;
                    patfile.documentType = model.documentType;
                    patfile.cb = model.patientID.ToString();

                    db.UserFiles.Add(patfile);
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = patfile.fileID, message = "" });
                    return response;
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "File name already taken." });
                    return response;
                }


            }
            catch (Exception ex)
            {
                return ThrowError(ex, "AddPatientCondition in PatientFileController.");
            }
           


        }

        [Route("api/editPatientFile")]
        [ResponseType(typeof(HttpResponseMessage))]
        public async Task<HttpResponseMessage> EditPatientFile(long fileID, EditFilesModel model)
        {
            UserFile patFile = new UserFile();
            try
            {
                if (fileID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid file ID." });
                    return response;
                }
                if (model.FileName == null || model.FileName == "" || !Regex.IsMatch(model.FileName.Trim(), "^[0-9a-zA-Z ]+$"))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid file name. Only letters and numbers are allowed." });
                    return response;
                }
                if (model.patientID == null || model.patientID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid patient ID." });
                    return response;
                }
                
                if (model.fileContent == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "File is empty. " });
                    return response;
                }
                if (model.documentType == null || model.documentType == "")
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Please provide document type. " });
                    return response;
                }
                
                //check for duplicate names
                patFile = db.UserFiles.Where(m => m.fileID != fileID && m.patientID==model.patientID && m.FileName == model.FileName.Trim() && m.active == true).FirstOrDefault();
                if (patFile != null)
                {
                    
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "File name already taken." });
                    return response;
                }

                patFile = db.UserFiles.Where(m => m.fileID == fileID).FirstOrDefault();
                if (patFile == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "File not found." });
                    return response;
                }
                else
                {
                    patFile.active = true;
                    patFile.FileName = model.FileName;
                    patFile.md = System.DateTime.Now;
                    patFile.fileContent = model.fileContent;
                    patFile.documentType = model.documentType;
                    patFile.mb = model.patientID.ToString();
                    db.Entry(patFile).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = fileID, message = "" });
                    return response;

                }


            }
            catch (Exception ex)
            {
                return ThrowError(ex, "EditPatientCondition in PatientFileController.");
            }

           
        }

        [HttpPost]
        [Route("api/deletePatientFile")]
        public async Task<HttpResponseMessage> RemovePatientFile(long fileID)
        {
            try
            {
                Patient patient = new Patient();
                if (fileID == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Invalid file ID." });
                    return response;
                }
                UserFile patFile = db.UserFiles.Where(cond => cond.fileID == fileID && cond.active == true).FirstOrDefault();
               // if (patFile != null) { patient = await db.Patients.FindAsync(patFile.patientID); }
                if (patFile == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "File not found." });
                    return response;
                }
                else
                {
                    patFile.active = false;//Delete Operation changed
                    patFile.mb = patFile.patientID.ToString();
                    patFile.md = System.DateTime.Now;
                    db.Entry(patFile).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, new ApiResultModel { ID = fileID, message = "" });
                    return response;
                }
             
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "DeletePatientFile in PatientFileController.");
            }

            
        }
        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {

            response = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiResultModel { ID = 0, message = "Internal server error at" + Action });
            response.ReasonPhrase = ex.Message;
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
