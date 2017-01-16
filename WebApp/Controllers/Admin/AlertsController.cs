using DataAccess;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApp.Helper;

namespace SwiftKare.Controllers
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class AlertsController : Controller
    {

        //
        // GET: /Doctor/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult Index()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var doc = db.SP_SelectAlerts();
                    return View(doc);
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("../Login/Index");
            }

        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    //var alerttext = Request.Form["alerttext"].ToString();
                    //var alertfor = Request.Form["alertfor"].ToString();
                    //var alert = (
                    //                from p in db.Alerts
                    //                where (p.alertFor == alertfor && p.alertText == alerttext)
                    //                select p
                    //            ).FirstOrDefault();
                    //if (alert == null)
                    //{
                    //    db.SP_AddAlerts(alerttext, alertfor, Session["LogedUserID"].ToString());
                    //    db.SaveChanges();
                    //}
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("../Login/Index");
            }
        }

        //
        // POST: /Doctor/Edit/5

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            if (Session["LogedUserID"] != null)
            {
                try
                {
                    //var id = Request.Form["id"].ToString();
                    //var alerttext = Request.Form["alerttext"].ToString();
                    //var alertfor = Request.Form["alertfor"].ToString();
                    //var alert = (
                    //                from p in db.Alerts
                    //                where (p.alertFor == alertfor && p.alertText == alerttext)
                    //                select p
                    //            ).FirstOrDefault();
                    //if (alert == null)
                    //{
                    //    db.sp_UpdateAlerts(Convert.ToInt64(id), alerttext, alertfor, Session["LogedUserID"].ToString(), System.DateTime.Now);
                    //    db.SaveChanges();
                    //}
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("../Login/Index");
            }

        }

        //
        // POST: /Doctor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["LogedUserID"] != null)
            {
                try
                {
                    db.sp_DeleteAlerts(id, Session["LogedUserID"].ToString(), System.DateTime.Now);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("../Login/Index");
            }

        }

    }
}
