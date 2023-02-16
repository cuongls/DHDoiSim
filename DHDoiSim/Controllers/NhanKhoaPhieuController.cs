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
    public class NhanKhoaPhieuController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();
        User user = new User();
        NhanKhoaPhieuDA Model = new NhanKhoaPhieuDA();

        //GET: NhanKhoaPhieu
        //Nguyên bản
        //public ActionResult Index()
        //{
        //    var sim_Phieu = db.Sim_Phieu.Include(s => s.DMDonViKT).Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Include(s => s.Sim_Trang);
        //    return View(sim_Phieu.ToList());
        //}

        // GET: NhanKhoaPhieu/Details/5
        public ActionResult Index(string NguongTon, string SapXep, string SoDT)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 8)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Khóa phiếu", Url.Action("Index"));

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "Theo Tổ",
                Value = "1",
                Selected = true,
            });
            list.Add(new SelectListItem
            {
                Text = "Theo Giờ xuất phiếu",
                Value = "2",
            });
            //list.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSSapXep"] = list;

            //DMNguongTon
            List<SelectListItem> list3 = new List<SelectListItem>();
            var lstNguongTon = db.DMNguongTons.ToList();
            for (int i = 0; i < lstNguongTon.Count(); i++)
            {
                list3.Add(new SelectListItem
                {
                    Text = lstNguongTon[i].GhiChu.ToString(),
                    Value = lstNguongTon[i].NguongTon.ToString(),
                });
            }
            list3.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSNguongTon"] = list3;

            //Thêm Where(x => x.ID_Phong == user.ID_Phong)
            var sim_Phieu = db.Sim_Phieu.Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Include(s => s.DMLyDoTon).Include(s => s.DMLoaiGiayTo).Where(x => x.ID_Phong == user.ID_PHONG && x.TimeKhoaPhieu == null && x.ID_KetQuaThucHien == 2);

            //if (!String.IsNullOrEmpty(Ten_QH) && Ten_QH.ToString() != "all")
            //{
            //    sim_Phieu = sim_Phieu.Where(x => x.Ten_QH.ToString() == Ten_QH.Trim());
            //}

            if (!String.IsNullOrEmpty(NguongTon) && NguongTon.ToString() != "all")
            {
                TimeSpan t = new TimeSpan(-3, 0, 0, 0);
                DateTime dt = System.DateTime.Now.Add(t);
                sim_Phieu = sim_Phieu.Where(x => x.TimeXP < dt);
            }

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_Phieu = sim_Phieu.Where(s => s.SoDT == SoDT.Trim());
            }

            //if (String.IsNullOrEmpty(Ten_QH) && String.IsNullOrEmpty(SoDT))
            //    sim_Phieu = sim_Phieu.Where(s => s.ID == -1);

            if (SapXep == "1")
                sim_Phieu = sim_Phieu.OrderBy(x => x.ID_To);
            else
                if (SapXep == "2")
                    sim_Phieu = sim_Phieu.OrderByDescending(x => x.TimeXP);
                else
                    sim_Phieu = sim_Phieu.OrderBy(x => x.ID_To);

            return View(sim_Phieu);
        }

        public ActionResult ChiaPhieu(string SoDT)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 7)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Chia phiếu", Url.Action("ChiaPhieu"));

            List<SelectListItem> list = new List<SelectListItem>();
            var aTen_QH = db.Sim_KH.Where(x => x.Ten_QH != null).OrderBy(x => x.Ten_QH).Select(x => x.Ten_QH).Distinct().ToArray();
            for (int i = 0; i < aTen_QH.Length; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = aTen_QH[i].ToString(),
                    Value = aTen_QH[i].ToString(),
                });
            }

            list.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSQuanHuyen"] = list;

            //Thêm Where(x => x.ID_Phong == user.ID_Phong)
            //Cũ: var sim_Phieu = db.Sim_Phieu.Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Where(x => x.ID_Phong == user.ID_PHONG && x.UserThucHien == null);
            var sim_Phieu = db.Sim_Phieu.Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Where(x => x.ID_Phong == user.ID_PHONG && x.TimeKhoaPhieu == null && x.ID_KetQuaThucHien == 2);
            
            //if (!String.IsNullOrEmpty(Ten_QH) && Ten_QH.ToString() != "all")
            //{
            //    sim_Phieu = sim_Phieu.Where(x => x.Ten_QH.ToString() == Ten_QH.Trim());
            //}

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_Phieu = sim_Phieu.Where(s => s.SoDT == SoDT.Trim());
            }

            //if (String.IsNullOrEmpty(Ten_QH) && String.IsNullOrEmpty(SoDT))
            //    sim_Phieu = sim_Phieu.Where(s => s.ID == -1);

            return View(sim_Phieu.OrderBy(x => x.ID_To).ToList());
        }
        //ThaiHN thêm
        public ActionResult TraCuu(string IDPhong, string IDKetQuaThucHien,string NguongTon, string SoDT,string TuNgay,string DenNgay)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 8)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Tra cứu Phiếu đổi sim", Url.Action("TraCuu"));

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
            list.Add(new SelectListItem { Text = "Tất cả", Value = "all",Selected = true});
            ViewData["DSPhong"] = list;

            //DMKetQuaThucHien
            List<SelectListItem> list2 = new List<SelectListItem>();
            var lstDMKQTH = db.DMKetQuaThucHiens.Where(x => x.ID > 1).ToList();
            for (int i = 0; i < lstDMKQTH.Count(); i++)
            {
                list2.Add(new SelectListItem
                {
                    Text = lstDMKQTH[i].StatusName.ToString(),
                    Value = lstDMKQTH[i].ID.ToString(),
                });
            }
            list2.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSKQTH"] = list2;

            //DMNguongTon
            List<SelectListItem> list3 = new List<SelectListItem>();
            var lstNguongTon = db.DMNguongTons.ToList();
            for (int i = 0; i < lstNguongTon.Count(); i++)
            {
                list3.Add(new SelectListItem
                {
                    Text = lstNguongTon[i].GhiChu.ToString(),
                    Value = lstNguongTon[i].NguongTon.ToString(),
                });
            }
            list3.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSNguongTon"] = list3;

            //Thêm Where(x => x.ID_Phong == user.ID_Phong)
            var sim_Phieu = db.Sim_Phieu.OrderByDescending(x => x.TimeKhoaPhieu).Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Include(s => s.DMLyDoTon).Include(s => s.DMLoaiGiayTo);

            if (!String.IsNullOrEmpty(IDPhong) && IDPhong.ToString() != "all")
            {
                sim_Phieu = sim_Phieu.Where(x => x.ID_Phong.ToString() == IDPhong.Trim());
            }

            if (!String.IsNullOrEmpty(IDKetQuaThucHien) && IDKetQuaThucHien.ToString() != "all")
            {
                sim_Phieu = sim_Phieu.Where(x => x.ID_KetQuaThucHien.ToString() == IDKetQuaThucHien.Trim());
            }

            if (!String.IsNullOrEmpty(NguongTon) && NguongTon.ToString() != "all")
            {
                TimeSpan t = new TimeSpan(-3, 0, 0, 0);
                DateTime dt = System.DateTime.Now.Add(t);
                sim_Phieu = sim_Phieu.Where(x => x.TimeXP < dt);
            }

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_Phieu = sim_Phieu.Where(s => s.SoDT == SoDT.Trim());
            }
            if (String.IsNullOrEmpty(IDPhong) && String.IsNullOrEmpty(SoDT))
            {
                sim_Phieu = sim_Phieu.Where(x => x.ID == -1);
            }

            if (!String.IsNullOrEmpty(TuNgay) && !String.IsNullOrEmpty(DenNgay))
            {
                var statdate = DateTime.Parse(TuNgay + " 00:00:00");
                var todate = DateTime.Parse(DenNgay + " 23:59:59");
                sim_Phieu = sim_Phieu.Where(x => x.TimeKhoaPhieu >= statdate && x.TimeKhoaPhieu <= todate);
            }

            return View(sim_Phieu.ToList());
        }

        public ActionResult DoiSimThanhCong(string IDPhong, string SoDT, string TuNgay, string DenNgay)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            //if (user.PERMISSION > 8)
            //    return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Thống kê Đổi sim thành công", Url.Action("DoiSimThanhCong"));

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
            list.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSPhong"] = list;

            //DMKetQuaThucHien
            List<SelectListItem> list2 = new List<SelectListItem>();
            var lstDMKQTH = db.DMKetQuaThucHiens.Where(x => x.ID > 1).ToList();
            for (int i = 0; i < lstDMKQTH.Count(); i++)
            {
                list2.Add(new SelectListItem
                {
                    Text = lstDMKQTH[i].StatusName.ToString(),
                    Value = lstDMKQTH[i].ID.ToString(),
                });
            }
            list2.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSKQTH"] = list2;

            //DMNguongTon
            List<SelectListItem> list3 = new List<SelectListItem>();
            var lstNguongTon = db.DMNguongTons.ToList();
            for (int i = 0; i < lstNguongTon.Count(); i++)
            {
                list3.Add(new SelectListItem
                {
                    Text = lstNguongTon[i].GhiChu.ToString(),
                    Value = lstNguongTon[i].NguongTon.ToString(),
                });
            }
            list3.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSNguongTon"] = list3;

            //Thêm Where(x => x.ID_Phong == user.ID_Phong)
            var sim_Phieu = db.Sim_Phieu.OrderByDescending(x => x.TimeKhoaPhieu).Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Include(s => s.DMLyDoTon).Include(s => s.DMLoaiGiayTo).Where(x => x.ID_KetQuaThucHien == 3);

            if (!String.IsNullOrEmpty(IDPhong) && IDPhong.ToString() != "all")
            {
                sim_Phieu = sim_Phieu.Where(x => x.ID_Phong.ToString() == IDPhong.Trim() && x.ID_KetQuaThucHien == 3);
            }

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_Phieu = sim_Phieu.Where(s => s.SoDT == SoDT.Trim()  && s.ID_KetQuaThucHien == 3);
            }
            if (String.IsNullOrEmpty(IDPhong) && String.IsNullOrEmpty(SoDT))
            {
                sim_Phieu = sim_Phieu.Where(x => x.ID == -1);
            }

            if (!String.IsNullOrEmpty(TuNgay) && !String.IsNullOrEmpty(DenNgay))
            {
                var statdate = DateTime.Parse(TuNgay + " 00:00:00");
                var todate = DateTime.Parse(DenNgay + " 23:59:59");
                sim_Phieu = sim_Phieu.Where(x => x.TimeKhoaPhieu >= statdate && x.TimeKhoaPhieu <= todate);
            }

            return View(sim_Phieu.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            return View(sim_Phieu);
        }

        // GET: NhanKhoaPhieu/Create
        public ActionResult Create(int? id)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 5)
                return RedirectToAction("Index", "Login");

            var aSim_KH = db.Sim_KH.Where(x => x.ID == id).ToArray();

            if (aSim_KH[0].ID_StatusOBKH != 3)
            {
                //return RedirectToAction("Index","OutBound");
                return RedirectToAction("Index","OutBound", new {SoDT = aSim_KH[0].SoDT});
            }

            if (aSim_KH[0].ID_KetQuaThucHien > 1)
            {
                //Đã đổi rồi nên không được XP
                //return RedirectToAction("Index","OutBound");
                return RedirectToAction("Index", "OutBound", new { SoDT = aSim_KH[0].SoDT });
            }

            ViewBag.SoDT_ = aSim_KH[0].SoDT;
            ViewBag.TenPhong_ = aSim_KH[0].TenPhong;
            string tenphong = aSim_KH[0].TenPhong.ToString();
            ViewBag.ID_Sim_KH_ = id;
            int IDPhong = db.DMPhongs.First(x => x.TenDonVi == tenphong).ID;

            //Hết thêm

            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi");
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName");
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName");
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi",IDPhong);
            ViewBag.ID_To = new SelectList(db.DMToes.Where(x => x.ID_Phong == IDPhong), "ID", "TenDonVi",1);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT");
            ViewBag.ID_Sim_Trang = new SelectList(db.Sim_Trang, "ID", "LoaiHang");
            return View();
        }

        // POST: NhanKhoaPhieu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            //Cần bỏ tất cả [Required] tại Sim_PhieuAttribute.cs
            if (ModelState.IsValid)
            {
                //Cần cập nhật
                //Cần thêm dk: Trống NguoiGoi,GioGoi,GioHen,DiaChiHenDoiSim thì không được XP 
                //Gốc
                //db.Sim_Phieu.Add(sim_Phieu);
                //db.SaveChanges();

                //ThaiHN thêm
                var simphieu = new Sim_Phieu();
                //sim_Phieu.ID_Sim_KH đã được gán sẵn trên giao diện Create
                Sim_KH simkh = db.Sim_KH.SingleOrDefault(x => x.ID == sim_Phieu.ID_Sim_KH);
                simphieu.ID_Sim_KH = sim_Phieu.ID_Sim_KH;

                //2 lệnh này sai nhưng vẫn hoạt động
                //Có thể co Ralation tự động trỏ đến?
                //Sim_KH simkh = db.Sim_KH.SingleOrDefault(x => x.ID == sim_Phieu.ID);
                //simphieu.ID_Sim_KH = simkh.ID;
                //Hết 2 lệnh này sai nhưng vẫn hoạt động

                //Lấy từ Sim_KH
                simphieu.SoDT = simkh.SoDT;
                simphieu.LoaiDienThoai = simkh.Code;
                simphieu.ModelDienThoai = simkh.Model;
                simphieu.LoaiTB = simkh.Loai_TB;
                simphieu.Ten_QH = simkh.QHHenDoiSim;
                simphieu.Ten_PX = simkh.PXHenDoiSim;
                simphieu.GioHen = simkh.GioHen;
                simphieu.DiaChiHenDoiSim = simkh.DiaChiHenDoiSim;
                //Hết
                simphieu.ID_KetQuaThucHien = 2;
                simphieu.ID_DonViKT = 1;
                //Lấy trên Form nhập
                simphieu.ID_MucUuTien = sim_Phieu.ID_MucUuTien;
                simphieu.ID_Phong = sim_Phieu.ID_Phong;
                simphieu.ID_To = sim_Phieu.ID_To;
                simphieu.TimeXP = sim_Phieu.TimeXP;
                simphieu.UserXP = sim_Phieu.UserXP;
                simphieu.NoiDungXP = sim_Phieu.NoiDungXP;
                simphieu.GhiChu = sim_Phieu.GhiChu;
                //Hết 
                db.Sim_Phieu.Add(simphieu);
                db.SaveChanges();
                //Update Sim_KH.ID_KetQuaThucHien
                var update = (from x in db.Sim_KH where x.ID == sim_Phieu.ID_Sim_KH select x).Single();
                update.ID_KetQuaThucHien = 2;
                update.ID_DonViKT = 1;
                db.SaveChanges();
                //Hết thêm

                //Gốc
                //return RedirectToAction("Index");
                //return RedirectToAction("Index", "OutBound", new { SoDT = simkh.SoDT });
                var getkh = (from x in db.Sim_KH where x.ID == sim_Phieu.ID_Sim_KH select x).Single();
                var sdt = getkh.SoDT;
                string queryString = "?IDPhong=all&SoDT=" + sdt;
                return Redirect(Url.Action("TraCuu") + queryString);
            }

            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_Sim_Trang = new SelectList(db.Sim_Trang, "ID", "LoaiHang", sim_Phieu.ID_Sim_Trang);
            return View(sim_Phieu);
        }

        // GET: NhanKhoaPhieu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ThaiHN thêm
            //if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
            //    return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 4)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Hết thêm

            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);

            //ViewBag.ID_Sim_Trang = new SelectList(db.Sim_Trang, "ID", "LoaiHang", sim_Phieu.ID_Sim_Trang);
            //ViewBag.ID_Sim_Trang = new SelectList(db.Sim_Trang, "ID", "SerialNumber", sim_Phieu.ID_Sim_Trang);

            //ThaiHN thêm
            //ViewBag.DSSimTrang = new SelectList(db.Sim_Trang.ToList().Select(x => new { id = x.SerialNumber, SN = x.SerialNumber }), "id", "SN");
            //ViewBag.TimeTraPhieu = System.DateTime.Now.ToString();
            //ViewBag.UserTraPhieu = user.USERNAME.ToString();

            //List<SelectListItem> list = new List<SelectListItem>();
            //var aSN = db.Sim_Trang.Where(x => x.SerialNumber != null).OrderBy(x => x.SerialNumber).Select(x => x.SerialNumber).Distinct().ToArray();
            //for (int i = 0; i < aSN.Length; i++)
            //{
            //    list.Add(new SelectListItem
            //    {
            //        Text = aSN[i].ToString(),
            //        Value = aSN[i].ToString(),
            //    });
            //}
            //list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            //ViewBag.DSSimTrang = list;
            ////Hết thêm

            return View(sim_Phieu);
        }

        // GET: NhanKhoaPhieu/Edit/5
        public ActionResult EditKhoaPhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ThaiHN thêm

            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 8)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Hết thêm

            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            ViewBag.list_user_khac = db.UserLists.Where(x => x.ID_Phong != sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            //Gốc
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens.Where(x => x.ID > 2), "ID", "StatusName", 3);

            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            //ThaiHN thêm
            ViewBag.ID_Sim_Phieu = id;
            ViewBag.SoDT = sim_Phieu.SoDT;
            ViewBag.CMT_Truoc = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CMT1.jpg";
            ViewBag.CMT_Sau = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CMT2.jpg";
            ViewBag.BienBan = "Đặt đúng tên File " + sim_Phieu.SoDT + ".PĐS.jpg";
            ViewBag.CamKetKhongCC = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CK.jpg";

            List<SelectListItem> list = new List<SelectListItem>();
            var aSN = db.Sim_Trang.Where(x => x.SerialNumber != null).OrderBy(x => x.SerialNumber).Select(x => x.SerialNumber).Distinct().ToArray();
            for (int i = 0; i < aSN.Length; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = aSN[i].ToString(),
                    Value = aSN[i].ToString(),
                });
            }
            list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            ViewBag.DSSimTrang = list;
            //Hết thêm

            return View(sim_Phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKhoaPhieu([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            if (string.IsNullOrEmpty(sim_Phieu.SerialNumber) && sim_Phieu.ID_KetQuaThucHien == 3)
            {
                ViewBag.Message = "Chưa chọn SerialNumber!";
                ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
                ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
                ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
                ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
                ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
                ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
                ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
                ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
                ViewBag.list_user_khac = db.UserLists.Where(x => x.ID_Phong != sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
                ViewBag.ID_Sim_Phieu = sim_Phieu.ID;
                ViewBag.SoDT = sim_Phieu.SoDT;
                return View(sim_Phieu);
            }

            if (sim_Phieu.DateThucHien == null && sim_Phieu.ID_KetQuaThucHien == 3)
            {
                ViewBag.Message = "Chưa nhập Ngày đổi sim!";
                ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
                ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
                ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
                ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
                ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
                ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
                ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
                ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
                ViewBag.list_user_khac = db.UserLists.Where(x => x.ID_Phong != sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
                ViewBag.ID_Sim_Phieu = sim_Phieu.ID;
                ViewBag.SoDT = sim_Phieu.SoDT;
                return View(sim_Phieu);
            }

            if (ModelState.IsValid)
            {
                //db.Entry(sim_Phieu).State = EntityState.Modified;
                var update = (from x in db.Sim_Phieu where x.ID == sim_Phieu.ID select x).Single();
                update.ID_LoaiGiayTo = sim_Phieu.ID_LoaiGiayTo;
                update.SerialNumber = sim_Phieu.SerialNumber;

                update.UserThucHien = sim_Phieu.UserThucHien;
                update.DateThucHien = sim_Phieu.DateThucHien;
                update.NoiDungThucHien = sim_Phieu.NoiDungThucHien;

                update.UserKhoaPhieu = sim_Phieu.UserKhoaPhieu;
                update.TimeKhoaPhieu = sim_Phieu.TimeKhoaPhieu;
                update.ID_KetQuaThucHien = sim_Phieu.ID_KetQuaThucHien;

                update.GhiChu = sim_Phieu.GhiChu;
                db.SaveChanges();

                var update2 = (from x in db.Sim_KH where x.ID == sim_Phieu.ID_Sim_KH select x).Single();
                update2.ID_KetQuaThucHien = sim_Phieu.ID_KetQuaThucHien;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            return View(sim_Phieu);
        }

        public ActionResult PhieuDaKhoa(string SoDT)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 1006)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Update Phiếu đã khóa", Url.Action("PhieuDaKhoa"));

            List<SelectListItem> list = new List<SelectListItem>();
            var aTen_QH = db.Sim_KH.Where(x => x.Ten_QH != null).OrderBy(x => x.Ten_QH).Select(x => x.Ten_QH).Distinct().ToArray();
            for (int i = 0; i < aTen_QH.Length; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = aTen_QH[i].ToString(),
                    Value = aTen_QH[i].ToString(),
                });
            }

            list.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSQuanHuyen"] = list;

            //Thêm Where(x => x.ID_Phong == user.ID_Phong)
            var sim_Phieu = db.Sim_Phieu.Include(s => s.DMKetQuaThucHien).Include(s => s.DMMucUuTien).Include(s => s.DMPhong).Include(s => s.DMTo).Include(s => s.Sim_KH).Include(s => s.DMLyDoTon).Include(s => s.DMLoaiGiayTo).Where(x => x.ID_Phong == user.ID_PHONG && x.TimeKhoaPhieu != null && x.ID_KetQuaThucHien == 3 && x.UserKhoaPhieu == user.USERNAME);

            //if (!String.IsNullOrEmpty(Ten_QH) && Ten_QH.ToString() != "all")
            //{
            //    sim_Phieu = sim_Phieu.Where(x => x.Ten_QH.ToString() == Ten_QH.Trim());
            //}

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_Phieu = sim_Phieu.Where(s => s.SoDT == SoDT.Trim());
            }

            if (String.IsNullOrEmpty(SoDT))
            {
                sim_Phieu = sim_Phieu.Where(s => s.ID == -1);
            }

            //if (String.IsNullOrEmpty(Ten_QH) && String.IsNullOrEmpty(SoDT))
            //    sim_Phieu = sim_Phieu.Where(s => s.ID == -1);

            return View(sim_Phieu.OrderBy(x => x.ID_To).ToList());
        }

        //UpdatePhieuDaKhoa
        public ActionResult UpdatePhieuDaKhoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ThaiHN thêm

            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 1006)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Hết thêm

            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            //Gốc
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens.Where(x => x.ID > 2), "ID", "StatusName", 3);

            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            //ThaiHN thêm
            ViewBag.ID_Sim_Phieu = id;
            ViewBag.SoDT = sim_Phieu.SoDT;
            ViewBag.CMT_Truoc = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CMT1.jpg";
            ViewBag.CMT_Sau = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CMT2.jpg";
            ViewBag.BienBan = "Đặt đúng tên File " + sim_Phieu.SoDT + ".PĐS.jpg";
            ViewBag.CamKetKhongCC = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CK.jpg";

            List<SelectListItem> list = new List<SelectListItem>();
            var aSN = db.Sim_Trang.Where(x => x.SerialNumber != null).OrderBy(x => x.SerialNumber).Select(x => x.SerialNumber).Distinct().ToArray();
            for (int i = 0; i < aSN.Length; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = aSN[i].ToString(),
                    Value = aSN[i].ToString(),
                });
            }
            list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            ViewBag.DSSimTrang = list;
            //Hết thêm

            return View(sim_Phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePhieuDaKhoa([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            //if (string.IsNullOrEmpty(sim_Phieu.SerialNumber) && sim_Phieu.ID_KetQuaThucHien == 3)
            //{
            //    ViewBag.Message = "Chưa chọn SerialNumber!";
            //    ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            //    ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            //    ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            //    ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            //    ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            //    ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            //    ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            //    ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            //    ViewBag.ID_Sim_Phieu = sim_Phieu.ID;
            //    ViewBag.SoDT = sim_Phieu.SoDT;
            //    return View(sim_Phieu);
            //}

            if (ModelState.IsValid)
            {
                //db.Entry(sim_Phieu).State = EntityState.Modified;
                var update = (from x in db.Sim_Phieu where x.ID == sim_Phieu.ID select x).Single();
                update.ID_LoaiGiayTo = sim_Phieu.ID_LoaiGiayTo;
                update.SerialNumber = sim_Phieu.SerialNumber;
                db.SaveChanges();

                return RedirectToAction("PhieuDaKhoa");
            }
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            return View(sim_Phieu);
        }
        //End

        //UpdatePhieu
        public ActionResult UpdatePhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ThaiHN thêm

            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 4)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Hết thêm

            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }

            if (sim_Phieu.ID_KetQuaThucHien != 2)
            {
                return HttpNotFound();
            }

            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            //Gốc
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens.Where(x => x.ID > 2), "ID", "StatusName", 3);

            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            //ThaiHN thêm
            ViewBag.ID_Sim_Phieu = id;
            ViewBag.SoDT = sim_Phieu.SoDT;
            ViewBag.CMT_Truoc = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CMT1.jpg";
            ViewBag.CMT_Sau = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CMT2.jpg";
            ViewBag.BienBan = "Đặt đúng tên File " + sim_Phieu.SoDT + ".PĐS.jpg";
            ViewBag.CamKetKhongCC = "Đặt đúng tên File " + sim_Phieu.SoDT + ".CK.jpg";

            List<SelectListItem> list = new List<SelectListItem>();
            var aSN = db.Sim_Trang.Where(x => x.SerialNumber != null).OrderBy(x => x.SerialNumber).Select(x => x.SerialNumber).Distinct().ToArray();
            for (int i = 0; i < aSN.Length; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = aSN[i].ToString(),
                    Value = aSN[i].ToString(),
                });
            }
            list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            ViewBag.DSSimTrang = list;
            //Hết thêm

            return View(sim_Phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePhieu([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            if (ModelState.IsValid)
            {
                var update = (from x in db.Sim_Phieu where x.ID == sim_Phieu.ID select x).Single();
                update.ID_Phong = sim_Phieu.ID_Phong;
                string TenPhong = (from x in db.DMPhongs.Where(x => x.ID == sim_Phieu.ID_Phong) select x.TenDonVi).FirstOrDefault();
                db.SaveChanges();
                var update2 = (from x in db.Sim_KH where x.ID == sim_Phieu.ID_Sim_KH select x).Single();
                update2.TenPhong = TenPhong;
                db.SaveChanges();

                //Insert Sim_Phieu_NhatKyUpdate
                var simphieunkupd = new Sim_Phieu_NhatKyUpdate();
                simphieunkupd.ID_Sim_Phieu = sim_Phieu.ID;
                simphieunkupd.SoDT = sim_Phieu.SoDT;
                simphieunkupd.TimeUpdate = System.DateTime.Now;
                simphieunkupd.UserUpdate = user.USERNAME;
                simphieunkupd.NoiDungUpDate = "Thay đổi Đơn vị thực hiện";
                db.Sim_Phieu_NhatKyUpdate.Add(simphieunkupd);
                db.SaveChanges();
                //End Insert Sim_Phieu_NhatKyUpdate
                return RedirectToAction("TraCuu");
            }
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_LoaiGiayTo = new SelectList(db.DMLoaiGiayToes, "ID", "LoaiGiayTo", sim_Phieu.ID_LoaiGiayTo);
            return View(sim_Phieu);
        }
        //End

        public ActionResult EditChiaPhieu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ThaiHN thêm

            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 7)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Hết thêm

            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            //Gốc

            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes.Where(x => x.ID_Phong == user.ID_PHONG), "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            //ThaiHN thêm
            ViewBag.ID_Sim_Phieu = id;
            ViewBag.SoDT = sim_Phieu.SoDT;
            //Hết thêm

            return View(sim_Phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditChiaPhieu([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            //if (string.IsNullOrEmpty(sim_Phieu.SerialNumber))
            //{
            //    ViewBag.Message = "Chưa chọn SerialNumber!";
            //    return View();
            //}

            if (ModelState.IsValid)
            {
                //db.Entry(sim_Phieu).State = EntityState.Modified;
                var update = (from x in db.Sim_Phieu where x.ID == sim_Phieu.ID select x).Single();
                update.UserChiaPhieu = sim_Phieu.UserChiaPhieu;
                update.TimeChiaPhieu = sim_Phieu.TimeChiaPhieu;
                update.NoiDungChiaPhieu = sim_Phieu.NoiDungChiaPhieu;
                update.ID_To = sim_Phieu.ID_To;
                db.SaveChanges();

                return RedirectToAction("ChiaPhieu");
            }
            //ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes.Where(x => x.ID_Phong == user.ID_PHONG), "ID", "TenDonVi", sim_Phieu.ID_To);
            return View(sim_Phieu);
        }

        public ActionResult EditLyDoTon(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ThaiHN thêm

            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 8)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Hết thêm

            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.list_user = db.UserLists.Where(x => x.ID_Phong == sim_Phieu.ID_Phong).OrderBy(y => y.UserName);
            //Gốc

            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_LyDoTon = new SelectList(db.DMLyDoTons, "ID", "LyDoTon", sim_Phieu.ID_LyDoTon);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            //ThaiHN thêm
            ViewBag.ID_Sim_Phieu = id;
            ViewBag.SoDT = sim_Phieu.SoDT;
            //Hết thêm

            return View(sim_Phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLyDoTon([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            //if (string.IsNullOrEmpty(sim_Phieu.SerialNumber))
            //{
            //    ViewBag.Message = "Chưa chọn SerialNumber!";
            //    return View();
            //}

            if (ModelState.IsValid)
            {
                //db.Entry(sim_Phieu).State = EntityState.Modified;
                var update = (from x in db.Sim_Phieu where x.ID == sim_Phieu.ID select x).Single();
                update.UserNhapTon = sim_Phieu.UserNhapTon;
                update.TimeNhapTon = sim_Phieu.TimeNhapTon;
                update.NgayHenMoi = sim_Phieu.NgayHenMoi;
                update.NoiDungNhapTon = sim_Phieu.NoiDungNhapTon;
                update.ID_LyDoTon = sim_Phieu.ID_LyDoTon;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            //ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_LyDoTon = new SelectList(db.DMLyDoTons, "ID", "LyDoTon", sim_Phieu.ID_To);
            return View(sim_Phieu);
        }
        // POST: NhanKhoaPhieu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ID_Sim_KH,ID_Sim_Trang,SoDT,LoaiTB,LoaiDienThoai,ModelDienThoai,Ten_QH,Ten_PX,DiaChiHenDoiSim,GioHen,ID_MucUuTien,ID_Phong,ID_To,TimeXP,UserXP,NoiDungXP,TimeChiaPhieu,UserChiaPhieu,NoiDungChiaPhieu,TimeNhanPhieu,UserNhanPhieu,NoiDungNhanPhieu,TimeCapNhatKetQuaThucHien,UserThucHien,DateThucHien,NoiDungThucHien,ID_KetQuaThucHien,Image_CMT_Truoc,Image_CMT_Sau,Image_BienBan,SerialNumber,GhiChu,ID_DonViKT,PhanMemKhoaPhieu,TimeChiaPhieu2,UserChiaPhieu2,NoiDungChiaPhieu2,TimeKhoaPhieu,UserKhoaPhieu,NoiDungKhoaPhieu,Image_CamKetKhongCC,ID_LyDoTon,TimeNhapTon,UserNhapTon,NoiDungNhapTon,ID_LoaiGiayTo,NgayHenMoi")] Sim_Phieu sim_Phieu)
        {
            //Cần cập nhật:
            //Sim_KH.ID_KetQuaThucHien
            //Sim_Trang.ID_StatusSuDung
            if (ModelState.IsValid)
            {
                db.Entry(sim_Phieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_Phieu.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_Phieu.ID_KetQuaThucHien);
            ViewBag.ID_MucUuTien = new SelectList(db.DMMucUuTiens, "ID", "StatusName", sim_Phieu.ID_MucUuTien);
            ViewBag.ID_Phong = new SelectList(db.DMPhongs, "ID", "TenDonVi", sim_Phieu.ID_Phong);
            ViewBag.ID_To = new SelectList(db.DMToes, "ID", "TenDonVi", sim_Phieu.ID_To);
            ViewBag.ID_Sim_KH = new SelectList(db.Sim_KH, "ID", "SoDT", sim_Phieu.ID_Sim_KH);
            ViewBag.ID_Sim_Trang = new SelectList(db.Sim_Trang, "ID", "LoaiHang", sim_Phieu.ID_Sim_Trang);
            return View(sim_Phieu);
        }

        // GET: NhanKhoaPhieu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            if (sim_Phieu == null)
            {
                return HttpNotFound();
            }
            return View(sim_Phieu);
        }

        // POST: NhanKhoaPhieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sim_Phieu sim_Phieu = db.Sim_Phieu.Find(id);
            db.Sim_Phieu.Remove(sim_Phieu);
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
