using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Models;
using DHDoiSim.Common;

namespace DHDoiSim.Controllers
{
    public class NhapSimTrangController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();
        User user = new User();
        // GET: NhapSimTrang
        //public ActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public ActionResult NhapTheoDaiSerial(int? id)
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 6)
                return RedirectToAction("Index", "Login");

            ViewBag.ID_Sim_Phieu = id.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult NhapTheoDaiSerial(DateTime? NgayBanGiao, int? SNDau, int? SNCuoi, string GhiChu)
        {
            if (NgayBanGiao == null)
            {
                ViewBag.Message = "Chưa nhập ngày bàn giao!";
                return View();
            }

            //if (MenhGia == null)
            //{
            //    ViewBag.Message = "Chưa nhập mệnh giá!";
            //    return View();
            //}
            if(SNDau == null || SNCuoi == null)
            {
                ViewBag.Message = "Chưa nhập SNDau hoặc SNCuoi!";
                return View();
            }
            if (SNDau.ToString().Length != 10)
            { 
                ViewBag.Message = "Sai độ dài SNDau!";
                return View();
            }
            if (SNCuoi.ToString().Length != 10)
            {
                ViewBag.Message = "Sai độ dài SNCuoi!";
                return View();
            }

            bool DaNhap = false;
            if (SNCuoi > SNDau)
            {
                //Kiểm tra trong Dải SN có số đã nhập không
                for (int i = (int)SNDau; i <= (int)SNCuoi; i++)
                {
                    if (db.Sim_Trang.Where(x => x.SerialNumber == i.ToString()).Count() >0)
                    {
                        DaNhap = true;
                        break;
                    }
                }

                if (DaNhap == true)
                {
                    ViewBag.Message = "Trong dải Serial Number có số SN đã nhập!";
                    return View();
                }
                else
                    for (int i = (int)SNDau; i <= (int)SNCuoi; i ++)
                    {
                        var simtrang = new Sim_Trang();
                        simtrang.NgayBanGiao = NgayBanGiao;
                        //simtrang.LoaiHang = LoaiHang;
                        //simtrang.MenhGia = MenhGia;
                        simtrang.SerialNumber = i.ToString();
                        simtrang.ThucCap = 1;
                        //simtrang.ThanhTien = MenhGia;
                        simtrang.GhiChu = GhiChu;
                        simtrang.ID_Phong = user.ID_PHONG;
                        simtrang.UserUpdate = user.USERNAME;
                        simtrang.TimeUpdate = System.DateTime.Now;
                        simtrang.Timestamps = System.DateTime.Now;
                        simtrang.ID_DonViKT = 1;
                        simtrang.ID_StatusSuDung = 1;
                        db.Sim_Trang.Add(simtrang);
                        db.SaveChanges();
                    }
                ViewBag.Message = "Bạn đã nhập " + ((int)SNCuoi - (int)SNDau + 1).ToString() + " sim trắng";
                return View();
            }
            else
            {
                ViewBag.Message = "Số Serial Number cuối không được > Số SerialNumber đầu";
                return View();
            }
        }
        public ActionResult NhapSimTrang()
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 6)
                return RedirectToAction("Index", "Login");
            //DMPhong
            List<SelectListItem> list = new List<SelectListItem>();
            var lstPhong = db.DMPhongs.ToList();
            for (int i = 0; i < lstPhong.Count(); i++)
            {
                list.Add(new SelectListItem
                {
                    Text = lstPhong[i].TenDonVi.ToString(),
                    Value = lstPhong[i].ID.ToString(),
                });
            }
            ViewData["DSPhong"] = list;
            return View();
        }
        [HttpPost]
        public ActionResult NhapSimTrang(DateTime NgayBanGiao, int IdPhong, int SoLuong, string GhiChu)
        {
            if (NgayBanGiao == null)
            {
                ViewBag.Message = "Chưa nhập ngày bàn giao!";
                return View();
            }
            //DMPhong
            List<SelectListItem> list = new List<SelectListItem>();
            var lstPhong = db.DMPhongs.ToList();
            for (int i = 0; i < lstPhong.Count(); i++)
            {
                list.Add(new SelectListItem
                {
                    Text = lstPhong[i].TenDonVi.ToString(),
                    Value = lstPhong[i].ID.ToString(),
                });
            }
            ViewData["DSPhong"] = list;
            var simtrang = new Sim_Trang();
            simtrang.NgayBanGiao = NgayBanGiao;
            simtrang.GhiChu = GhiChu;
            simtrang.ID_Phong = IdPhong;
            simtrang.UserUpdate = user.USERNAME;
            simtrang.TimeUpdate = System.DateTime.Now;
            simtrang.Timestamps = System.DateTime.Now;
            simtrang.SoLuong = SoLuong;
            simtrang.LoaiDuLieu = "SOLUONG";
            db.Sim_Trang.Add(simtrang);
            db.SaveChanges();
            @ViewBag.Message = "Cập nhật thành công!!!!!";

            return View();
        }
    }
}