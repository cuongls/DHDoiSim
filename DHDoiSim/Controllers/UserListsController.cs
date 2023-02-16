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
    public class UserListsController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();
        User user = new User();
        // GET: UserLists
        public ActionResult Index()
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 3)
                return RedirectToAction("Index", "Login");

            var userLists = db.UserLists.Include(u => u.DMPhong).Include(u => u.DMTo).Include(u => u.Permission);
            return View(userLists.ToList());
        }

        // GET: UserLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserList userList = db.UserLists.Find(id);
            if (userList == null)
            {
                return HttpNotFound();
            }
            return View(userList);
        }

        // GET: UserLists/Create
        public ActionResult Create()
        {
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi");
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi");
            ViewBag.ID_Permission = new SelectList(db.Permissions, "ID_Permission", "Permission_Name");
            return View();
        }

        // POST: UserLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_User,UserName,Password,Email,Avatar,Name,Gender,Birthday,Phone,ID_Permission,Last_Login,Last_Seen,Last_Seen_URL,ID_Phong,ID_To,Timestamps,HoatDong,UserCreate,MaDonVi,GhiChu")] UserList userList)
        {
            if (ModelState.IsValid)
            {
                db.UserLists.Add(userList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", userList.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", userList.ID_To);
            ViewBag.ID_Permission = new SelectList(db.Permissions, "ID_Permission", "Permission_Name", userList.ID_Permission);

            return View(userList);
        }

        // GET: UserLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserList userList = db.UserLists.Find(id);
            if (userList == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", userList.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes.Where(x => x.ID_Phong == userList.ID_Phong), "ID", "TenDonVi", userList.ID_To);
            ViewBag.ID_Permission = new SelectList(db.Permissions, "ID_Permission", "Permission_Name", userList.ID_Permission);
            return View(userList);
        }

        // POST: UserLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_User,UserName,Password,Email,Avatar,Name,Gender,Birthday,Phone,ID_Permission,Last_Login,Last_Seen,Last_Seen_URL,ID_Phong,ID_To,Timestamps,HoatDong,UserCreate,MaDonVi,GhiChu")] UserList userList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", userList.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", userList.ID_To);
            ViewBag.ID_Permission = new SelectList(db.Permissions, "ID_Permission", "Permission_Name", userList.ID_Permission);
            return View(userList);
        }

        // GET: UserLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserList userList = db.UserLists.Find(id);
            if (userList == null)
            {
                return HttpNotFound();
            }
            return View(userList);
        }

        // POST: UserLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserList userList = db.UserLists.Find(id);
            db.UserLists.Remove(userList);
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
