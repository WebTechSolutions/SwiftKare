using System.Web.Mvc;

namespace SwiftKare.Controllers
{
    public class AdminLoginController : Controller
    {
        //
        // GET: /Login/





        public ActionResult Default()
        {

            return View();
        }


        public ActionResult logoutuser()
        {
            Session["LogedUserID"] = null;
            return RedirectToAction("Index");

        }

    }
}
