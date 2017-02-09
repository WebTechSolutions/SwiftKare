using System.Configuration;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class StripePayController : Controller
    {
        // GET: StripPay
        public ActionResult Index()
        {
            ViewBag.PublisherKey = ConfigurationManager.AppSettings["StripePayPublisherKey"].ToString();
            ViewBag.Amount = 2000;
            return View();
        }

        [HttpPost]
        public string ProceedWithPay(string tokenId)
        {
            var isSuceed =  Helper.StripePayHelper.PerformStripeCharge(tokenId, 2000);
            return isSuceed ? "succeed" : "failed";
        }

      

    }
    
}