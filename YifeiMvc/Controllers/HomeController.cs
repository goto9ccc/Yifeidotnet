using System.Web.Mvc;
using Yifei.Framework;
using Yifei.Framework.Common;

namespace YifeiMvc.Controllers
{
    //[Auth]
    public class HomeController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }

        [ChildActionOnly]
        public ActionResult Navbar()
        {
            ViewBag.NavBar = "";
            return View();
        }
    }
}