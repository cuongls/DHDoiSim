using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Models;
using DHDoiSim.Common;

namespace DHDoiSim.Controllers
{
    public class ThongKesController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();
        User user = new User();
        NhanKhoaPhieuDA Model = new NhanKhoaPhieuDA();

        // GET: ThongKes
        public ActionResult KetQuaDoiSim(string TuNgay, string DenNgay)
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            //if (user.PERMISSION > 8)
            //    return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Thống kê kết quả Đổi sim", Url.Action("KetQuaDoiSim"));

            //foreach (var ds_baocao in db.ThongKes)
            //{
            //    ds_baocao.TongSoPhieu = (from x in db.Sim_Phieu.Where(y => y.ID_Phong == ds_baocao.ID_Phong) select x).Count();
            //    ds_baocao.DoiSimThanhCong = (from x in db.Sim_Phieu.Where(y => y.ID_Phong == ds_baocao.ID_Phong && y.ID_KetQuaThucHien == 3) select x).Count();
            //    ds_baocao.DoiSimKhongThanhCong = (from x in db.Sim_Phieu.Where(y => y.ID_Phong == ds_baocao.ID_Phong && y.ID_KetQuaThucHien == 4) select x).Count();
            //    ds_baocao.ChuaCoKetQua = (from x in db.Sim_Phieu.Where(y => y.ID_Phong == ds_baocao.ID_Phong && y.ID_KetQuaThucHien != 3 && y.ID_KetQuaThucHien != 4) select x).Count();

            //    ds_baocao.SoSimTrangDaCap = (from x in db.Sim_Trang.Where(y => y.ID_Phong == ds_baocao.ID_Phong) select x).Count();
            //}
            //db.SaveChanges();

            //db.ThongKes.ToList().ForEach(a => a.TyLeDoiSimThanhCong = (a.DoiSimThanhCong)*100/a.TongSoPhieu);
            //db.SaveChanges();

            if (!String.IsNullOrEmpty(TuNgay) && !String.IsNullOrEmpty(DenNgay))
            {
                var statdate = DateTime.Parse(TuNgay + " 00:00:00");
                var todate = DateTime.Parse(DenNgay + " 23:59:59");
                var data = db.SP_ThongKeKetQuaDoiSim(statdate, todate);
            }
            else
            {
                return View(db.ThongKes.Where(x => x.ID == -1).ToList());
            }

            ViewBag.TongSoPhieu = db.ThongKes.Sum(x => x.TongSoPhieu);
            ViewBag.DoiSimThanhCong = db.ThongKes.Sum(x => x.DoiSimThanhCong);
            ViewBag.DoiSimKhongThanhCong = db.ThongKes.Sum(x => x.DoiSimKhongThanhCong);
            ViewBag.ChuaCoKetQua = db.ThongKes.Sum(x => x.ChuaCoKetQua);
            ViewBag.SoSimTrangDaCap = db.ThongKes.Sum(x => x.SoSimTrangDaCap);
            //ViewBag.TyLeDoiSimThangCong = 10;
            ViewBag.TyLeDoiSimThangCong = (db.ThongKes.Sum(x => x.DoiSimThanhCong)*100)/ db.ThongKes.Sum(x => x.TongSoPhieu);
            var thongKes = db.ThongKes.Include(t => t.DMPhong);
            return View(thongKes.ToList());
        }

        public ActionResult NSCL(string IDKy, string Nhom, string UserName)
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 8)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Thống kê NSCL", Url.Action("NSCL"));

            List<SelectListItem> list = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
            var aKy = db.DMKyThongKeNSCLs.Where(x => x.ID > 1).OrderByDescending(y =>y.ID).ToArray();
            for (int i = 0; i < aKy.Length; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = aKy[i].GhiChu.ToString(),
                    Value = aKy[i].ID.ToString(),
                });
            }
            //list.Add(new SelectListItem { Text = "Chọn thời gian", Value = "", Selected = true });
            ViewData["DSKy"] = list;

            List<SelectListItem> list2 = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
            var aNhom = db.DMDonViThongKeNSCLs.ToArray();
            for (int i = 0; i < aNhom.Length; i++)
            {
                list2.Add(new SelectListItem
                {
                    Text = aNhom[i].TenDonVi.ToString(),
                    Value = aNhom[i].MaDonVi.ToString(),
                });
            }
            //list2.Add(new SelectListItem { Text = "Chọn nhóm", Value = "", Selected = true });
            ViewData["DSNhom"] = list2;
   
            var Thang = (from x in db.DMKyThongKeNSCLs.Where(y => y.ID.ToString() == IDKy) select x.Thang).FirstOrDefault();
            var Nam = (from x in db.DMKyThongKeNSCLs.Where(y => y.ID.ToString() == IDKy) select x.Nam).FirstOrDefault();
            var data = db.SP_ThongKeNSCL(Thang,Nam,Nhom);

            //GoiHenDoiSim4G_XuatPhieuDoiSim
            var list_tk1 = db.Sim_KH.Where(x => x.ID_StatusOBKH == 3);
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(IDKy))
            {
                list_tk1 = list_tk1.Where(x => x.ID < 1);
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                list_tk1= list_tk1.Where(x => x.NguoiGoi == UserName);
            }
            if (!String.IsNullOrEmpty(IDKy))
            {
                list_tk1 = list_tk1.Where(x => x.GioGoi.Value.Month == Thang && x.GioGoi.Value.Year == Nam);
            }
            ViewBag.list_GoiHenDoiSim4G_XuatPhieuDoiSim = list_tk1; ;

            //GoiHenDoiSim4G_ThangCong
            var list_tk2 = db.Sim_KH.Where(x => x.ID_StatusOBKH == 3 && x.ID_KetQuaThucHien == 3);
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(IDKy))
            {
                list_tk2 = list_tk2.Where(x => x.ID < 1);
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                list_tk2 = list_tk2.Where(x => x.NguoiGoi == UserName);
            }
            if (!String.IsNullOrEmpty(IDKy))
            {
                list_tk2 = list_tk2.Where(x => x.GioGoi.Value.Month == Thang && x.GioGoi.Value.Year == Nam);
            }
            ViewBag.list_GoiHenDoiSim4G_ThanhCong = list_tk2; ;

            //GoiHenDoiSim4G_KHDaDoiTaiQuay
            var list_tk3 = db.Sim_KH.Where(x => x.ID_StatusOBKH == 7);
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(IDKy))
            {
                list_tk3 = list_tk3.Where(x => x.ID < 1);
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                list_tk3 = list_tk3.Where(x => x.NguoiGoi == UserName);
            }
            if (!String.IsNullOrEmpty(IDKy))
            {
                list_tk3 = list_tk3.Where(x => x.GioGoi.Value.Month == Thang && x.GioGoi.Value.Year == Nam);
            }
            ViewBag.list_GoiHenDoiSim4G_KHDaDoiTaiQuay = list_tk3; 

            //
            var list_tk4 = db.Sim_Phieu.Where(x => x.UserThucHien == UserName);
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(IDKy))
            {
                list_tk4 = list_tk4.Where(x => x.ID < 1);
            }
            if (!String.IsNullOrEmpty(UserName))
            {
                list_tk4 = list_tk4.Where(x => x.UserThucHien == UserName);
            }
            if (!String.IsNullOrEmpty(IDKy))
            {
                list_tk4 = list_tk4.Where(x => x.DateThucHien.Value.Month == Thang && x.DateThucHien.Value.Year == Nam);
            }
            ViewBag.list_ThucHienDoiSim4G_ThanhCong = list_tk4;

            var thongKes = db.ThongKeNSCLs.Where(x =>x.Thang == Thang && x.Nam == Nam && x.MaDonVi == Nhom);
            return View(thongKes.ToList());
        }

        public ActionResult ThongKeOutBound(string Nhom, string TuNgay, string DenNgay)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            //if (user.PERMISSION > 8)
            //    return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Thống kê OutBound", Url.Action("ThongKeOutBound"));

            List<SelectListItem> list2 = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
            var aNhom = db.DMDonViThongKeNSCLs.ToArray();
            for (int i = 0; i < aNhom.Length; i++)
            {
                list2.Add(new SelectListItem
                {
                    Text = aNhom[i].TenDonVi.ToString(),
                    Value = aNhom[i].MaDonVi.ToString(),
                });
            }
            //list2.Add(new SelectListItem { Text = "Chọn nhóm", Value = "", Selected = true });
            ViewData["DSNhom"] = list2;

            if (!String.IsNullOrEmpty(TuNgay) && !String.IsNullOrEmpty(DenNgay) && !String.IsNullOrEmpty(Nhom))
            {
                var statdate = DateTime.Parse(TuNgay + " 00:00:00");
                var todate = DateTime.Parse(DenNgay + " 23:59:59");
                var data = db.SP_ThongKeOutBound(Nhom, statdate, todate);
            }
            else
            {
                return View(db.ThongKeOutBounds.Where(x=>x.ID == -1).ToList());
            }
            var thongKeOutBound = db.ThongKeOutBounds;
            return View(thongKeOutBound.ToList().OrderBy(x=>x.MaDonVi).ThenBy(y => y.UserName));
        }

        // GET: ThongKes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongKe thongKe = db.ThongKes.Find(id);
            if (thongKe == null)
            {
                return HttpNotFound();
            }
            return View(thongKe);
        }

        // GET: ThongKes/Create
        public ActionResult Create()
        {
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi");
            return View();
        }

        // POST: ThongKes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ID_Phong,DoiSimThanhCong,SoSimTrangDaCap,DoiSimKhongThanhCong,ChuaCoKetQua,TongSoPhieu,TyLeDoiSimThanhCong,NgayBC,NgayGioTaoBaoCao")] ThongKe thongKe)
        {
            if (ModelState.IsValid)
            {
                db.ThongKes.Add(thongKe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", thongKe.ID_Phong);
            return View(thongKe);
        }

        // GET: ThongKes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongKe thongKe = db.ThongKes.Find(id);
            if (thongKe == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", thongKe.ID_Phong);
            return View(thongKe);
        }

        // POST: ThongKes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ID_Phong,DoiSimThanhCong,SoSimTrangDaCap,DoiSimKhongThanhCong,ChuaCoKetQua,TongSoPhieu,TyLeDoiSimThanhCong,NgayBC,NgayGioTaoBaoCao")] ThongKe thongKe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thongKe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", thongKe.ID_Phong);
            return View(thongKe);
        }

        // GET: ThongKes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongKe thongKe = db.ThongKes.Find(id);
            if (thongKe == null)
            {
                return HttpNotFound();
            }
            return View(thongKe);
        }

        // POST: ThongKes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThongKe thongKe = db.ThongKes.Find(id);
            db.ThongKes.Remove(thongKe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
