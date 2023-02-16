using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Models;

namespace DHDoiSim.Controllers
{
    public class SimTrangController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();

        // GET: SimTrang
        public ActionResult Index(string IDPhong, string SN)
        {

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
            list.Add(new SelectListItem { Text = "Tất cả", Value = "all" });
            ViewData["DSPhong"] = list;

            var sim_Trang = db.Sim_Trang.Include(s => s.DMDonViKT).Include(s => s.DMPhong).Include(s => s.DMStatusSuDung).Include(s => s.DMTo);
            if (!String.IsNullOrEmpty(IDPhong) && IDPhong.ToString() != "all")
            {
                sim_Trang = sim_Trang.Where(x => x.ID_Phong.ToString() == IDPhong.Trim());
            }

            if (!String.IsNullOrEmpty(SN) && SN.ToString() != "")
            {
                sim_Trang = sim_Trang.Where(s => s.SerialNumber == SN.Trim());
            }

            return View(sim_Trang.Take(1000).ToList());
        }

        public ActionResult TraCuu(string IDPhong, string SN)
        {
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
            list.Add(new SelectListItem { Text = "Tất cả", Value = "all" });
            ViewData["DSPhong"] = list;

            var sim_Trang = db.Sim_Trang.Include(s => s.DMDonViKT).Include(s => s.DMPhong).Include(s => s.DMStatusSuDung).Include(s => s.DMTo);
            if (!String.IsNullOrEmpty(IDPhong) && IDPhong.ToString() != "all")
            {
                sim_Trang = sim_Trang.Where(x => x.ID_Phong.ToString() == IDPhong.Trim());
            }

            if (!String.IsNullOrEmpty(SN) && SN.ToString() != "")
            {
                sim_Trang = sim_Trang.Where(s => s.SerialNumber == SN.Trim());
            }

            return View(sim_Trang.Take(1000).ToList());
        }

        // GET: SimTrang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_Trang sim_Trang = db.Sim_Trang.Find(id);
            if (sim_Trang == null)
            {
                return HttpNotFound();
            }
            return View(sim_Trang);
        }

        // GET: SimTrang/Create
        public ActionResult Create()
        {
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi");
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi");
            ViewBag.ID_StatusSuDung = new SelectList(db.DMStatusSuDungs, "ID", "StatusName");
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi");
            return View();
        }

        // POST: SimTrang/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,STT,LoaiHang,MenhGia,ThucCap,ThanhTien,SerialNumber,BaoHanh,NgayBanGiao,GhiChu,ID_Phong,ID_To,TimeUpdate,UserUpdate,Timestamps,ID_StatusSuDung,ID_DonViKT")] Sim_Trang sim_Trang)
        {
            if (ModelState.IsValid)
            {
                db.Sim_Trang.Add(sim_Trang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Trang.ID_DonViKT);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Trang.ID_Phong);
            ViewBag.ID_StatusSuDung = new SelectList(db.DMStatusSuDungs, "ID", "StatusName", sim_Trang.ID_StatusSuDung);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Trang.ID_To);
            return View(sim_Trang);
        }

        // GET: SimTrang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_Trang sim_Trang = db.Sim_Trang.Find(id);
            if (sim_Trang == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Trang.ID_DonViKT);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Trang.ID_Phong);
            ViewBag.ID_StatusSuDung = new SelectList(db.DMStatusSuDungs, "ID", "StatusName", sim_Trang.ID_StatusSuDung);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Trang.ID_To);
            return View(sim_Trang);
        }

        // POST: SimTrang/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,STT,LoaiHang,MenhGia,ThucCap,ThanhTien,SerialNumber,BaoHanh,NgayBanGiao,GhiChu,ID_Phong,ID_To,TimeUpdate,UserUpdate,Timestamps,ID_StatusSuDung,ID_DonViKT")] Sim_Trang sim_Trang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sim_Trang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Trang.ID_DonViKT);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Trang.ID_Phong);
            ViewBag.ID_StatusSuDung = new SelectList(db.DMStatusSuDungs, "ID", "StatusName", sim_Trang.ID_StatusSuDung);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Trang.ID_To);
            return View(sim_Trang);
        }

        // GET: SimTrang/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_Trang sim_Trang = db.Sim_Trang.Find(id);
            if (sim_Trang == null)
            {
                return HttpNotFound();
            }
            return View(sim_Trang);
        }

        // POST: SimTrang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sim_Trang sim_Trang = db.Sim_Trang.Find(id);
            db.Sim_Trang.Remove(sim_Trang);
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
