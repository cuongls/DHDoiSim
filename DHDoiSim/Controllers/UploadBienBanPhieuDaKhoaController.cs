using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Common;
using DHDoiSim.Models;

namespace DHDoiSim.Controllers
{
    public class UploadBienBanPhieuDaKhoaController : Controller
    {
        //User user = new User();
        DHDoiSimEntities db = new DHDoiSimEntities();
        // GET: UploadBienBanPhieuDaKhoa
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UploadFile(int? id)
        {
            ViewBag.ID_Sim_Phieu = id.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, string idsp)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    file.SaveAs(_path);

                    var update = (from x in db.Sim_Phieu where x.ID.ToString() == idsp select x).Single();
                    update.Image_BienBan = _FileName;
                    db.SaveChanges();
                }

                ViewBag.Message = "File Uploaded Successfully!idsp = " + idsp.ToString();
                //return View();
                return RedirectToAction("../NhanKhoaPhieu/UpdatePhieuDaKhoa/" + idsp);
                //< a href = "/DHDoiSim/NhanKhoaPhieu/EditKhoaPhieu/@item.ID
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

    }
}