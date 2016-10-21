using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwiftKare.Controllers.Reports
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult showReport()
        {
            if (Session["LogedUserID"] != null)
            {

                try
                {
                    //var doc = db.SP_selectTransactionHistory();
                    return View();
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

    }
}
