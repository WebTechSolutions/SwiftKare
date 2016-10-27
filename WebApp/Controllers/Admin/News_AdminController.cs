using DataAccess;
using SwiftKare.Models.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;

namespace SwiftKare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class News_AdminController : Controller
    {
        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        Utility util = new Utility();

        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {



                try
                {

                    var news = db.SP_SelectNewss();
                    return View(news);

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
           
                var newsid = "";
                var newstitle = "";
                var newsdetail = "";
                byte[] thumbBytes = null;
                byte[] detailBytes = null;
                ViewBag.successMessage = "";
                ViewBag.errorMessage = "";
            if (Session["LogedUserID"] != null)
            {


                try
                {
                    var action = Request.Form["action"].ToString();
                    if (action == "create")
                    {
                        HttpPostedFileBase thumbnail = Request.Files["thumbnailimage"];
                        HttpPostedFileBase detailimage = Request.Files["detailimage"];
                        newstitle = Request.Form["newstitle"].ToString();
                        newsdetail = Request.Form["newsdetail"].ToString();
                        thumbBytes = util.ConvertToBytes(thumbnail);
                        detailBytes = util.ConvertToBytes(detailimage);

                        var news = (
                                       from p in db.News
                                       where (p.newsTitle == newstitle || p.newsDetail==newsdetail && p.active == true)
                                       select p
                                   ).FirstOrDefault();
                        if (news != null)
                        {
                            ViewBag.successMessage = "";
                            ViewBag.errorMessage = "News already exists";

                        }
                        if (news == null)
                        {
                            db.SP_AddNewss(newstitle, newsdetail, thumbBytes, detailBytes,Session["LogedUserID"].ToString());
                            db.SaveChanges();
                            ViewBag.successMessage = "Record has been saved successfully";
                            ViewBag.errorMessage = "";
                        }
                    }
                    if (action == "edit")
                    {
                        //HttpPostedFileBase thumbnail = Request.Files["tthumbnailimage"];
                       // HttpPostedFileBase detailimage = Request.Files["ddetailimage"];
                       if(Request.Files.Count==2)
                        {
                            thumbBytes = util.ConvertToBytes(Request.Files[0]); ;
                            detailBytes = util.ConvertToBytes(Request.Files[1]); ;
                        }

                        newsid = Request.Form["newsid"].ToString();
                        newstitle = Request.Form["newstitle"].ToString();
                        newsdetail = Request.Form["newsdetail"].ToString();
                        //thumbBytes = util.ConvertToBytes(thumbnail);
                        //detailBytes = util.ConvertToBytes(detailimage);

                        db.sp_UpdateNews(Convert.ToInt64(newsid), newstitle, newsdetail, thumbBytes, detailBytes, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been saved successfully";
                        ViewBag.errorMessage = "";
                       
                    }
                    if (action == "delete")
                    {
                        newsid = Request.Form["newsid"].ToString();
                        db.sp_DeleteNews(Convert.ToInt64(newsid), Session["LogedUserID"].ToString(), System.DateTime.Now);
                        db.SaveChanges();
                        ViewBag.successMessage = "Record has been deleted successfully";
                        ViewBag.errorMessage = "";
                    }
                    var __existingnewsList = db.SP_SelectNewss();
                    return View(__existingnewsList);

                }
                catch (Exception ex)
                {
                    ViewBag.errorMessage = "Error occurred while processing your request.";
                    var _existingnewsList = db.SP_SelectNewss();
                    return View(_existingnewsList);
                }
            }
            else
            {

                return RedirectToAction("../AdminLogin/AdminLogin");
            }


        }

        public ActionResult RetrieveImage(int id,string type)
        {
           

            byte[] cover = util.GetImageFromDataBase(id,type);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
       

}

    }
}
