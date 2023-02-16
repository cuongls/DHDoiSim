using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Common;

namespace DHDoiSim.Controllers
{
    public class HomeController : Controller
    {
        User user = new User();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            //if (!user.IsAdmin())
            //    return View("Error");
            user.Reset();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}