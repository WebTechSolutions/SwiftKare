using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class DocumentType_AdminController : Controller
    {
        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult Create()
        {

            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var doctype = db.SP_SelectDocType();
                    return View(doctype);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while loading data.";
                    return View();
                }
        }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }


}

[HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {


                var documentname = "";
                var docid = "";
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        documentname = Request.Form["documentname"].ToString();
                        var doctype = (
                                       from p in db.DocumentTypes
                                       where (p.typeName == documentname && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (doctype != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "Document type already exists";
                           
                        }
                        if (doctype == null)
                        {
                            db.SP_AddDocumentType(documentname, Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        docid = Request.Form["id"].ToString();
                        documentname = Request.Form["documentname"].ToString();
                    //    var doctype = (
                    //                   from p in db.DocumentType
                    //                   where (p.typeName == documentname && p.active == true)
                    //                   select p
                    //               ).FirstOrDefault();
                    //    if (doctype != null)
                    //    {
                    //        ViewBag.successMessage = "";
                    //        ViewBag.errorMessage = "Document type already exists";
                           
                    //    }
                    //    if (doctype == null)
                    //    {
                            db.sp_UpdateDocumentType(Convert.ToInt64(docid), documentname, Session["LogedUserID"].ToString(), System.DateTime.Now);
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        //}
                    }
                    if (action == "delete")
                    {
                        docid = Request.Form["id"].ToString();
                        db.sp_DeleteDocumentType(Convert.ToInt64(docid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var _existingdoctypeList = db.SP_SelectDocType();
                    return View(_existingdoctypeList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingdoctypeList = db.SP_SelectDocType();
                    return View(_existingdoctypeList);
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }

        }

    }

        //
        // POST: /Doctor/Edit/5

        

    }

