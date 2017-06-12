using DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApp.Helper;
using WebApp.Models;
using WebApp.Repositories.DoctorRepositories;
using WebApp.Repositories.PatientRepositories;

namespace WebApp.Controllers
{
    [PatientSessionExpire]
    [Authorize(Roles = "Patient")]
    public class PatientFilesController : Controller
    {

        #region Declarations

        PatientFilesRepository oPatientFilesRepository;

        public PatientFilesController()
        {
            oPatientFilesRepository = new PatientFilesRepository();
        }

        #endregion

        #region Action Methods


        // GET: Patient
        public ActionResult Index()
        {
            var oAllFileTypes = oPatientFilesRepository.GetPatientFileTypes();
            ViewBag.drpFileTypes = oAllFileTypes.Select(x => new SelectListItem { Text = x.typeName, Value = x.typeName });

            return View();
        }

        public PartialViewResult PartialIndex()
        {
            var oData = oPatientFilesRepository.GetPatientFiles(SessionHandler.UserInfo.Id);
            return PartialView("PartialIndex", oData);
        }


        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase inputFile = Request.Files;
                    for (int i = 0; i < inputFile.Count; i++)
                    {
                        HttpPostedFileBase oFileInput = inputFile[i];
                        string fileName;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = oFileInput.FileName.Split(new char[] { '\\' });
                            fileName = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fileName = oFileInput.FileName;
                        }

                        MemoryStream oByteStream = new MemoryStream();
                        oFileInput.InputStream.CopyTo(oByteStream);
                        byte[] filebytearr = oByteStream.ToArray();
                        string contentType = MimeMapping.GetMimeMapping(fileName);
                        oPatientFilesRepository.AddPatientFiles(new DataAccess.CustomModels.FilesCustomModel
                        {
                            documentType = Convert.ToString(Request["fileType"]),
                            FileName = Path.GetFileName(fileName),
                            fileContent = "data:"+ contentType + ";base64," + Convert.ToBase64String(filebytearr),
                            patientID = SessionHandler.UserInfo.Id,
                            doctorID = -1
                        });

                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public JsonResult deleteFile(long fileId)
        {
            var oRet = oPatientFilesRepository.DeletePatientFile(fileId);
            return Json(oRet);
        }


        public FileResult Download(long fileId)
        {
            var oFileToDownload = oPatientFilesRepository.GetPatientFile(SessionHandler.UserInfo.Id, fileId);

            string fileName = oFileToDownload.FileName;
            string contentType = string.Empty;

            switch (Path.GetExtension(oFileToDownload.FileName).Trim('.').ToLower())
            {
                case "txt":
                    contentType = "text/plain";
                    break;

                case "pdf":
                    contentType = "application/pdf";
                    break;

                case "docx":
                    contentType = "application/docx";
                    break;

                case "csv":
                    contentType = "text/csv";
                    break;

                case "jpg":
                    contentType = "image/jpeg";
                    break;

                case "png":
                    contentType = "image/png";
                    break;

                default:
                    contentType = "application/binary";
                    break;
            }
            byte[] filebytearray = null;
            var retBase64 = oFileToDownload.fileContent.Substring(oFileToDownload.fileContent.IndexOf("base64,") + 7);
            filebytearray = System.Convert.FromBase64String(retBase64);
            return File(filebytearray, contentType, fileName);
                   
        }
       
        private string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        #endregion


    }
}