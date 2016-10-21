using System.Web.Mvc;
using RestAPIs.Models;

namespace RestAPIs.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (ApplicationGlobalVariables.Instance.IsApiHelpEnabled)
                return View();
            return HttpNotFound();
        }
    }
}