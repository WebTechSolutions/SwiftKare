using DataAccess;
using SwiftKare.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class News_AdminController : Controller
    {
        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        Utility util = new Utility();

        [HttpPost]
        public JsonResult GetNewsDetail(long newsid)
        {
            try
            {
                News newObj = new News();
                newObj = db.News.Where(n => n.newsID == newsid).FirstOrDefault();
                var serializer = new JavaScriptSerializer();

                // For simplicity just use Int32's max value.
                serializer.MaxJsonLength = Int32.MaxValue;

                var resultData = new { Success = true, Object = newObj };
                return new JsonResult()
                {
                    Data = resultData,
                    MaxJsonLength = Int32.MaxValue
                };
                //return Json(new { Success = true, Object = newObj });

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }
        public ActionResult Create()
        {
            if (Session["LogedUserID"] != null)
            {



                try
                {

                    //var news = db.SP_SelectNewss();
                    List<News> news = new List<News>();
                    news=db.News.Where(n=>n.active==true).ToList();
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

                return RedirectToAction("AdminLogin", "Account");
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
                string thumbBase64 = "";
                string detailBase64 = "";
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
                        thumbBase64= "data:image/png;base64,"+Convert.ToBase64String(thumbBytes);
                        detailBase64 ="data:image/png;base64," + Convert.ToBase64String(detailBytes);
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
                            News newsObj = new News();
                            newsObj.active = true;
                            newsObj.newsTitle = newstitle;
                            newsObj.newsDetail = newsdetail;
                            newsObj.newsThumbnailBase64 = thumbBase64;
                            newsObj.newsImageBase64 = detailBase64;
                            newsObj.cb = Session["LogedUserID"].ToString();
                            newsObj.cd = System.DateTime.UtcNow;
                            db.News.Add(newsObj);
                            db.SaveChanges();
                            //db.SP_AddNewss(newstitle, newsdetail, thumbBytes, detailBytes,Session["LogedUserID"].ToString());
                            //db.SaveChanges();
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
                            thumbBytes = util.ConvertToBytes(Request.Files[0]); 
                            detailBytes = util.ConvertToBytes(Request.Files[1]);
                            thumbBase64 = "data:image/png;base64," + Convert.ToBase64String(thumbBytes);
                            detailBase64 ="data:image/png;base64," + Convert.ToBase64String(detailBytes);
                        }

                        newsid = Request.Form["newsid"].ToString();
                        newstitle = Request.Form["newstitle"].ToString();
                        newsdetail = Request.Form["newsdetail"].ToString();
                        long nid = Convert.ToInt64(newsid);
                        News newsObj = new News();
                        newsObj = db.News.Where(n => n.newsID == nid && n.active == true).FirstOrDefault();
                        if(newsObj!=null)
                        {
                            newsObj.active = true;
                            newsObj.newsTitle = newstitle;
                            newsObj.newsDetail = newsdetail;
                            newsObj.newsThumbnailBase64 = thumbBase64;
                            newsObj.newsImageBase64 = detailBase64;
                            newsObj.mb = Session["LogedUserID"].ToString();
                            newsObj.md = System.DateTime.UtcNow;
                            db.Entry(newsObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        
                        //db.sp_UpdateNews(Convert.ToInt64(newsid), newstitle, newsdetail, thumbBytes, detailBytes, Session["LogedUserID"].ToString(), System.DateTime.Now);
                        //db.SaveChanges();
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

                return RedirectToAction("AdminLogin", "Account");
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
