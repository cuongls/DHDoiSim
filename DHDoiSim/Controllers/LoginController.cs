using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Models;
using DHDoiSim.Common;
namespace DHDoiSim.Controllers
{
    public class LoginController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();
        //User user = new User();
        User user;
        public ActionResult Index()
        {
            if (Session[UserSession.ISLOGIN] != null && (bool)Session[UserSession.ISLOGIN])
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsValid(model))
                {
                    user = new User();
                    if (user.IsLogin())
                    {
                        Session["UserName"] = user.USERNAME;
                        Session["Phong"] = (from x in db.DMPhongs.Where(y => y.ID == user.ID_PHONG) select x.TenDonVi).First().ToString();
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                    ViewBag.error = "Tài khoản hoặc mật khẩu không đúng";
            }
            else
                ViewBag.error = "Có lỗi xảy ra trong quá trình xử lý, vui lòng thử lại sau.";
            return View();
        }
    }
}