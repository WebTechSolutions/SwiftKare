using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp;
using WebApp.Helper;

namespace SwiftKare.Controllers.TransactionHistory
{
    [AdminSessionExpire]
    [Authorize(Roles = "Admin")]
    public class Transaction_AdminController : Controller
    {
        //
        // GET: /Transaction/
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public ActionResult TransactionHistory()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    var doc = db.SP_selectTransactionHistory();
                    return View(doc);
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("../AdminLogin/AdminLogin");
            }
        }

        

    }
}
