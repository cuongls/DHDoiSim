using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DHDoiSim.Models;
using PagedList;
using DHDoiSim.Common;
using System.Data.Entity.Validation;
using System.Data.OleDb;

namespace DHDoiSim.Controllers
{
    public class OutBoundController : Controller
    {
        private DHDoiSimEntities db = new DHDoiSimEntities();
        User user = new User();
        OutBoundDA Model = new OutBoundDA();

        //GET: OutBound
        //Gốc
        //public ActionResult Index()
        //{
        //    var sim_KH = db.Sim_KH.Include(s => s.DMDonViKT).Include(s => s.DMKetQuaThucHien).Include(s => s.DMStatusOBKH);
        //    return View(sim_KH.ToList());
        //}

        public ActionResult Index(int? page, int? pageSize, string Ten_QH,string Ten_Phong, string ID_StatusOB, string ID_KetQuaThucHien, string NguoiGoi, string SoDT)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 5)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Gọi OutBound", Url.Action("Index"));

            List<SelectListItem> list = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
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

            //Phòng
            List<SelectListItem> listphong = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
            var aTen_Phong = db.Sim_KH.Where(x => x.GhiChu3 != null).OrderBy(x => x.GhiChu3).Select(x => x.GhiChu3).Distinct().ToArray();
            for (int i = 0; i < aTen_Phong.Length; i++)
            {
                listphong.Add(new SelectListItem
                {
                    Text = aTen_Phong[i].ToString(),
                    Value = aTen_Phong[i].ToString(),
                });
            }
            listphong.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSPhong"] = listphong;

            //set viewbag value
            ViewBag.Ten_QH = Ten_QH ?? "all";
            ViewBag.Ten_Phong = Ten_Phong ?? "all";

            //ViewBag.ID_StatusOB = ID_StatusOB ?? "all";
            //ViewBag.ID_KetQuaThucHien = ID_KetQuaThucHien ?? "all";
            ViewBag.ID_StatusOB = ID_StatusOB ?? "all";
            ViewBag.ID_KetQuaThucHien = ID_KetQuaThucHien ?? "all";
            ViewBag.NguoiGoi = NguoiGoi ?? "all";

            List<SelectListItem> list2 = new List<SelectListItem>();

            var aStatusOB = db.DMStatusOBKHs.ToList();
            for (int i = 0; i < aStatusOB.Count(); i++)
            {
                if (aStatusOB[i].ID == 1)
                    list2.Add(new SelectListItem
                    {
                        Text = aStatusOB[i].StatusName.ToString(),
                        Value = aStatusOB[i].ID.ToString(),
                        Selected = true,
                    });
                else
                    list2.Add(new SelectListItem
                    {
                        Text = aStatusOB[i].StatusName.ToString(),
                        Value = aStatusOB[i].ID.ToString(),
                    });
            }
            list2.Add(new SelectListItem { Text = "Tất cả", Value = "all"});
            ViewData["DSTrangThai"] = list2;

            List<SelectListItem> list3 = new List<SelectListItem>();

            var aKetQuaThucHien = db.DMKetQuaThucHiens.ToList();
            for (int i = 0; i < aKetQuaThucHien.Count(); i++)
            {
                if (aKetQuaThucHien[i].ID == 1)
                    list3.Add(new SelectListItem
                    {
                        Text = aKetQuaThucHien[i].StatusName.ToString(),
                        Value = aKetQuaThucHien[i].ID.ToString(),
                        Selected = true,
                    });
                else
                    list3.Add(new SelectListItem
                    {
                        Text = aKetQuaThucHien[i].StatusName.ToString(),
                        Value = aKetQuaThucHien[i].ID.ToString(),
                    });
            }
            list3.Add(new SelectListItem { Text = "Tất cả", Value = "all"});
            ViewData["DSKetQuaThucHien"] = list3;

            //Người gọi
            List<SelectListItem> listnguoigoi = new List<SelectListItem>();
            var aNguoiGoi = db.Sim_KH.Where(x => x.NguoiGoi != null).OrderBy(x => x.NguoiGoi).Select(x => x.NguoiGoi).Distinct().ToArray();
            for (int i = 0; i < aNguoiGoi.Length; i++)
            {
                listnguoigoi.Add(new SelectListItem
                {
                    Text = aNguoiGoi[i].ToString(),
                    Value = aNguoiGoi[i].ToString(),
                });
            }
            listnguoigoi.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSNguoiGoi"] = listnguoigoi;

            if (pageSize == null)
            {
                pageSize = 200;
            }

            int pageIndex = 1;
            ViewBag.Page = page == null ? "1" : page.ToString();
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<Sim_KH> simkh = null;
            //var Theo_Doi_Vat_Tu = db.So_Theo_Doi_Vat_Tu.Include(s => s.DM_Nguon_Tai_San).Include(s => s.DM_Vat_Tu_Chi_Tiet).Include(s => s.DM_TinhTrangVT).Include(s => s.Thiet_Bi);

            var sim_KHs = db.Sim_KH.Include(s => s.DMDonViKT).Include(s => s.DMStatusOBKH).Where(x => x.ID > 0);

            if (!String.IsNullOrEmpty(Ten_QH) && Ten_QH.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.Ten_QH.ToString() == Ten_QH.Trim());
            }

            if (!String.IsNullOrEmpty(Ten_Phong) && Ten_Phong.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.GhiChu3.ToString() == Ten_Phong.Trim());
            }

            if (!String.IsNullOrEmpty(ID_StatusOB) && ID_StatusOB.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.ID_StatusOBKH.ToString() == ID_StatusOB.Trim());
            }

            if (!String.IsNullOrEmpty(ID_KetQuaThucHien) && ID_KetQuaThucHien.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.ID_KetQuaThucHien.ToString() == ID_KetQuaThucHien.Trim());
            }

            if (!String.IsNullOrEmpty(NguoiGoi) && NguoiGoi.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.NguoiGoi.ToString() ==NguoiGoi.Trim());
            }

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_KHs = sim_KHs.Where(s => s.SoDT == SoDT.Trim());
            }

            if (String.IsNullOrEmpty(Ten_QH) && String.IsNullOrEmpty(ID_StatusOB) && String.IsNullOrEmpty(ID_KetQuaThucHien) && String.IsNullOrEmpty(NguoiGoi) && String.IsNullOrEmpty(SoDT))
                sim_KHs = sim_KHs.Where(s => s.ID == -1);

            simkh = sim_KHs.OrderByDescending(s => s.Ten_QH).ToPagedList(pageIndex, pageSize ?? 50);
            return View(simkh);

            //Hết thêm 
            //return View(sim_KHs.ToList());
        }

        //ThaiHN thêm
        public ActionResult TraCuu(int? page, int? pageSize, string Ten_QH, string Ten_Phong, string Ten_Phong_NP, string ID_StatusOB, string ID_KetQuaThucHien, string NguoiGoi, string SoDT)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 8)
                return RedirectToAction("Index", "Login");

            Model.UpdateLastLogin();
            Model.UpdateLastSeen("Tra cứu kết quả OutBound", Url.Action("TraCuu"));

            List<SelectListItem> list = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
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

            //Phòng
            List<SelectListItem> listphong = new List<SelectListItem>();
            //var Ten_QH = (from c in db.Sim_KH select c).ToArray();
            var aTen_Phong = db.Sim_KH.Where(x => x.GhiChu3 != null).OrderBy(x => x.GhiChu3).Select(x => x.GhiChu3).Distinct().ToArray();
            for (int i = 0; i < aTen_Phong.Length; i++)
            {
                listphong.Add(new SelectListItem
                {
                    Text = aTen_Phong[i].ToString(),
                    Value = aTen_Phong[i].ToString(),
                });
            }
            listphong.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSPhong"] = listphong;

            //set viewbag value
            ViewBag.Ten_QH = Ten_QH ?? "all";
            ViewBag.Ten_Phong = Ten_Phong ?? "all";
            ViewBag.Ten_Phong_NP = Ten_Phong_NP ?? "all";

            ViewBag.ID_StatusOB = ID_StatusOB ?? "all";
            ViewBag.ID_KetQuaThucHien = ID_KetQuaThucHien ?? "all";
            ViewBag.NguoiGoi = NguoiGoi ?? "all";

            List<SelectListItem> list2 = new List<SelectListItem>();

            var aStatusOB = db.DMStatusOBKHs.ToList();
            for (int i = 0; i < aStatusOB.Count(); i++)
            {
                list2.Add(new SelectListItem
                {
                    Text = aStatusOB[i].StatusName.ToString(),
                    Value = aStatusOB[i].ID.ToString(),
                });
            }
            list2.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSTrangThai"] = list2;

            List<SelectListItem> list3 = new List<SelectListItem>();

            var aKetQuaThucHien = db.DMKetQuaThucHiens.ToList();
            for (int i = 0; i < aKetQuaThucHien.Count(); i++)
            {
                list3.Add(new SelectListItem
                {
                    Text = aKetQuaThucHien[i].StatusName.ToString(),
                    Value = aKetQuaThucHien[i].ID.ToString(),
                });
            }
            list3.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSKetQuaThucHien"] = list3;

            //Người gọi
            List<SelectListItem> listnguoigoi = new List<SelectListItem>();
            var aNguoiGoi = db.Sim_KH.Where(x => x.NguoiGoi != null).OrderBy(x => x.NguoiGoi).Select(x => x.NguoiGoi).Distinct().ToArray();
            for (int i = 0; i < aNguoiGoi.Length; i++)
            {
                listnguoigoi.Add(new SelectListItem
                {
                    Text = aNguoiGoi[i].ToString(),
                    Value = aNguoiGoi[i].ToString(),
                });
            }
            listnguoigoi.Add(new SelectListItem { Text = "Tất cả", Value = "all", Selected = true });
            ViewData["DSNguoiGoi"] = listnguoigoi;

            if (pageSize ==null)
            {
                pageSize = 200;
            }

            int pageIndex = 1;
            ViewBag.Page = page == null ? "1" : page.ToString();
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<Sim_KH> simkh = null;

            var sim_KHs = db.Sim_KH.Include(s => s.DMDonViKT).Include(s => s.DMStatusOBKH).Where(x => x.ID > 0);

            if (!String.IsNullOrEmpty(Ten_QH) && Ten_QH.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.Ten_QH.ToString() == Ten_QH.Trim());
            }

            if (!String.IsNullOrEmpty(Ten_Phong) && Ten_Phong.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.GhiChu3.ToString() == Ten_Phong.Trim());
            }

            if (!String.IsNullOrEmpty(Ten_Phong_NP) && Ten_Phong_NP.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.TenPhong.ToString() == Ten_Phong_NP.Trim());
            }

            if (!String.IsNullOrEmpty(ID_StatusOB) && ID_StatusOB.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.ID_StatusOBKH.ToString() == ID_StatusOB.Trim());
            }

            if (!String.IsNullOrEmpty(ID_KetQuaThucHien) && ID_KetQuaThucHien.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.ID_KetQuaThucHien.ToString() == ID_KetQuaThucHien.Trim());
            }

            if (!String.IsNullOrEmpty(NguoiGoi) && NguoiGoi.ToString() != "all")
            {
                sim_KHs = sim_KHs.Where(x => x.NguoiGoi.ToString() == NguoiGoi.Trim());
            }

            if (!String.IsNullOrEmpty(SoDT) && SoDT.ToString() != "")
            {
                sim_KHs = sim_KHs.Where(s => s.SoDT == SoDT.Trim());
            }

            if (String.IsNullOrEmpty(Ten_QH) && String.IsNullOrEmpty(Ten_Phong) && String.IsNullOrEmpty(Ten_Phong_NP) && String.IsNullOrEmpty(ID_StatusOB) && String.IsNullOrEmpty(ID_KetQuaThucHien) && String.IsNullOrEmpty(NguoiGoi) && String.IsNullOrEmpty(SoDT))
                sim_KHs = sim_KHs.Where(s => s.ID == -1);

            simkh = sim_KHs.OrderByDescending(s => s.Ten_QH).ToPagedList(pageIndex, pageSize??50);
            return View(simkh);

            //Hết thêm 
            //return View(sim_KHs.ToList());
        }

        // GET: OutBound/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_KH sim_KH = db.Sim_KH.Find(id);
            if (sim_KH == null)
            {
                return HttpNotFound();
            }
            return View(sim_KH);
        }

        // GET: OutBound/Create
        public ActionResult Create()
        {
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi");
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName");
            ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName");
            return View();
        }

        // POST: OutBound/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SoDT,Loai_TB,BTS_Name,Code,Model,Dia_Chi_BTS,Don_Vi,TTVT,Dia_Chi_BTS2,Phuong_Xa_ID,Quan_Huyen_ID,Ve_Tinh_ID,TenPhong,Ten_TT,Ten_QH,Ten_PX,Doi_ID,Ten_Doi,TT_ID,TimeUpFile,UserUpFile,Timestamps,ID_DonViKT,ID_StatusOBKH,NguoiGoi,GioGoi,GioHen,DiaChiHenDoiSim,GhiChu,ID_KetQuaThucHien,LyDoPhanDiaBan,TieuChi,GhiChu1,GhiChu2,GhiChu3,QHHenDoiSim,PXHenDoiSim")] Sim_KH sim_KH)
        {
            if (ModelState.IsValid)
            {
                db.Sim_KH.Add(sim_KH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_KH.ID_KetQuaThucHien);
            ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);
            return View(sim_KH);
        }

        public ActionResult CreateThueBaoMoi()
        {

            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 4)
                return RedirectToAction("Index", "Login");

            ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");

            ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
            ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());

            //ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi");
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName");
            //ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName");
            return View();
        }

        // POST: OutBound/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateThueBaoMoi([Bind(Include = "ID,SoDT,Loai_TB,BTS_Name,Code,Model,Dia_Chi_BTS,Don_Vi,TTVT,Dia_Chi_BTS2,Phuong_Xa_ID,Quan_Huyen_ID,Ve_Tinh_ID,TenPhong,Ten_TT,Ten_QH,Ten_PX,Doi_ID,Ten_Doi,TT_ID,TimeUpFile,UserUpFile,Timestamps,ID_DonViKT,ID_StatusOBKH,NguoiGoi,GioGoi,GioHen,DiaChiHenDoiSim,GhiChu,ID_KetQuaThucHien,LyDoPhanDiaBan,TieuChi,GhiChu1,GhiChu2,GhiChu3,QHHenDoiSim,PXHenDoiSim")] Sim_KH sim_KH)
        {
            //Cần KTra không trùng số ĐT
            int ii = db.Sim_KH.Where(x => x.SoDT == sim_KH.SoDT).Count();
            if (ii > 0)
            {
                ViewBag.Message = "Lỗi! Trùng với Số Điện thoại đã có!";

                ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
                ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
                ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
                return View(sim_KH);
            }
            if (ModelState.IsValid)
            {
                var simkh = new Sim_KH();
                //Các giá trị Default
                simkh.ID_StatusOBKH = 1;
                simkh.ID_KetQuaThucHien = 1;
                simkh.ID_DonViKT = 1;
                //Lấy trên Form nhập
                simkh.SoDT = sim_KH.SoDT;
                simkh.Loai_TB = sim_KH.Loai_TB;
                simkh.Code = sim_KH.Code;
                simkh.Model = sim_KH.Model;
                simkh.Dia_Chi_BTS = sim_KH.Dia_Chi_BTS;
                simkh.Ten_QH = sim_KH.Ten_QH;
                simkh.Ten_PX = sim_KH.Ten_PX;
                simkh.GhiChu3 = sim_KH.GhiChu3;
                simkh.TimeUpFile = sim_KH.TimeUpFile;
                simkh.UserUpFile = sim_KH.UserUpFile;
                simkh.Timestamps = sim_KH.Timestamps;
                simkh.LyDoPhanDiaBan = "NGOAIDANHSACH";
                //hết lấy trên Form nhập
                db.Sim_KH.Add(simkh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_KH.ID_KetQuaThucHien);
            //ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);
            return View(sim_KH);
        }

        public ActionResult CreateThueBaoMoiVaXP()
        {

            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 6)
                return RedirectToAction("Index", "Login");

            ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
            ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs.Where(x =>x.ID ==3), "ID", "StatusName",3);

            ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
            ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
            ViewBag.list_tp = db.DMPhongs;

            //ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi");
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName");
            //ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName");
            return View();
        }

        // POST: OutBound/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateThueBaoMoiVaXP([Bind(Include = "ID,SoDT,Loai_TB,BTS_Name,Code,Model,Dia_Chi_BTS,Don_Vi,TTVT,Dia_Chi_BTS2,Phuong_Xa_ID,Quan_Huyen_ID,Ve_Tinh_ID,TenPhong,Ten_TT,Ten_QH,Ten_PX,Doi_ID,Ten_Doi,TT_ID,TimeUpFile,UserUpFile,Timestamps,ID_DonViKT,ID_StatusOBKH,NguoiGoi,GioGoi,GioHen,DiaChiHenDoiSim,GhiChu,ID_KetQuaThucHien,LyDoPhanDiaBan,TieuChi,GhiChu1,GhiChu2,GhiChu3,QHHenDoiSim,PXHenDoiSim")] Sim_KH sim_KH)
        {
            //Cần KTra không trùng số ĐT
            int ii = db.Sim_KH.Where(x => x.SoDT == sim_KH.SoDT).Count();
            if (ii > 0)
            {
                ViewBag.Message = "Lỗi! Trùng với Số Điện thoại đã có trong DS gọi OutBound của Phòng VIP!";
                ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
                ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs.Where(x => x.ID == 3), "ID", "StatusName", 3);
                ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
                ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
                ViewBag.list_tp = db.DMPhongs;
                return View(sim_KH);
            }

            if (string.IsNullOrEmpty(sim_KH.SoDT))
            {
                ViewBag.Message = "Chưa nhập số ĐT!";
                ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
                ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs.Where(x => x.ID == 3), "ID", "StatusName", 3);
                ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
                ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
                ViewBag.list_tp = db.DMPhongs;
                return View(sim_KH);
            }

            if (string.IsNullOrEmpty(sim_KH.DiaChiHenDoiSim))
            {
                ViewBag.Message = "Chưa nhập Địa chỉ hẹn đổi sim!";
                ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
                ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs.Where(x => x.ID == 3), "ID", "StatusName", 3);
                ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
                ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
                ViewBag.list_tp = db.DMPhongs;
                return View(sim_KH);
            }

            if (string.IsNullOrEmpty(sim_KH.TenPhong))
            {
                ViewBag.Message = "Chưa nhập Tên Phòng!";
                ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
                ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs.Where(x => x.ID == 3), "ID", "StatusName", 3);
                ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
                ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
                ViewBag.list_tp = db.DMPhongs;
                return View(sim_KH);
            }

            if (ModelState.IsValid)
            {
                //Insert
                var simkh = new Sim_KH();
                //Các giá trị Default
                //simkh.ID_StatusOBKH = 1;
                simkh.ID_KetQuaThucHien = 2;
                simkh.ID_DonViKT = 1;
                //Lấy trên Form nhập
                simkh.SoDT = sim_KH.SoDT;
                simkh.ID_StatusOBKH = sim_KH.ID_StatusOBKH;
                simkh.Loai_TB = sim_KH.Loai_TB;
                simkh.Code = sim_KH.Code;
                simkh.Model = sim_KH.Model;
                simkh.DiaChiHenDoiSim = sim_KH.DiaChiHenDoiSim;
                simkh.QHHenDoiSim = sim_KH.QHHenDoiSim;
                simkh.PXHenDoiSim = sim_KH.PXHenDoiSim;
                simkh.TenPhong = sim_KH.TenPhong;
                simkh.TimeUpFile = sim_KH.TimeUpFile;
                simkh.UserUpFile = sim_KH.UserUpFile;
                simkh.Timestamps = sim_KH.Timestamps;
                simkh.LyDoPhanDiaBan = "NGOAIDANHSACH";
                //Hết lấy trên Form nhập
                db.Sim_KH.Add(simkh);
                db.SaveChanges();

                //Xuất phiếu luôn
                var ID_Sim_KH = simkh.ID;
                var simphieu = new Sim_Phieu();
                simphieu.ID_Sim_KH = ID_Sim_KH;
                simphieu.SoDT = sim_KH.SoDT;
                simphieu.LoaiTB = sim_KH.Loai_TB;
                simphieu.LoaiDienThoai = sim_KH.Code;
                simphieu.ModelDienThoai = sim_KH.Model;
                simphieu.DiaChiHenDoiSim = sim_KH.DiaChiHenDoiSim;
                simphieu.Ten_QH = sim_KH.QHHenDoiSim;
                simphieu.Ten_PX = sim_KH.PXHenDoiSim;
                simphieu.ID_KetQuaThucHien = 2;
                simphieu.ID_MucUuTien = 1;
                simphieu.ID_Phong = (from x in db.DMPhongs.Where(y => y.TenDonVi == sim_KH.TenPhong) select x.ID).First();
                simphieu.TimeXP = sim_KH.TimeUpFile;
                simphieu.UserXP = sim_KH.UserUpFile;
                simphieu.NoiDungXP = "Đổi Sim ngoài Danh sách";
                db.Sim_Phieu.Add(simphieu);
                db.SaveChanges();
                var ID_Sim_Phieu = simphieu.ID;

                return RedirectToAction("../NhanKhoaPhieu/EditKhoaPhieu/" + ID_Sim_Phieu.ToString());

                ////ThaiHN thêm
                //var simphieu = new Sim_Phieu();
                ////sim_Phieu.ID_Sim_KH đã được gán sẵn trên giao diện Create
                //Sim_KH simkh = db.Sim_KH.SingleOrDefault(x => x.ID == sim_Phieu.ID_Sim_KH);
                //simphieu.ID_Sim_KH = sim_Phieu.ID_Sim_KH;

                ////2 lệnh này sai nhưng vẫn hoạt động
                ////Có thể co Ralation tự động trỏ đến?
                ////Sim_KH simkh = db.Sim_KH.SingleOrDefault(x => x.ID == sim_Phieu.ID);
                ////simphieu.ID_Sim_KH = simkh.ID;
                ////Hết 2 lệnh này sai nhưng vẫn hoạt động

                ////Lấy từ Sim_KH
                //simphieu.GioHen = simkh.GioHen;
                ////Hết
                //simphieu.ID_To = sim_Phieu.ID_To;
                //simphieu.GhiChu = sim_Phieu.GhiChu;
                //db.Sim_Phieu.Add(simphieu);
                //db.SaveChanges();
                ////Hết thêm

                ////Gốc
                ////return RedirectToAction("Index");
                ////return RedirectToAction("Index", "OutBound", new { SoDT = simkh.SoDT });
                //var getkh = (from x in db.Sim_KH where x.ID == sim_Phieu.ID_Sim_KH select x).Single();
                //var sdt = getkh.SoDT;
                //string queryString = "?IDPhong=all&SoDT=" + sdt;
                //return Redirect(Url.Action("TraCuu") + queryString);
            }

            //ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            //ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_KH.ID_KetQuaThucHien);
            //ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);
            return View(sim_KH);
        }

        // GET: OutBound/Edit/5
        public ActionResult Edit(int? id)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 4)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var aSim_KH = db.Sim_KH.Where(x => x.ID == id).ToArray();
            if (aSim_KH[0].ID_KetQuaThucHien != 4)
            {
                return RedirectToAction("Index", "OutBound");
            }

            Sim_KH sim_KH = db.Sim_KH.Find(id);
            if (sim_KH == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens.Where(x => x.ID == 1 || x.ID == 4), "ID", "StatusName", sim_KH.ID_KetQuaThucHien);
            ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);
            //ThaiHN Thêm
            ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");
            //Hết thêm
            return View(sim_KH);
        }

        // POST: OutBound/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SoDT,Loai_TB,BTS_Name,Code,Model,Dia_Chi_BTS,Don_Vi,TTVT,Dia_Chi_BTS2,Phuong_Xa_ID,Quan_Huyen_ID,Ve_Tinh_ID,TenPhong,Ten_TT,Ten_QH,Ten_PX,Doi_ID,Ten_Doi,TT_ID,TimeUpFile,UserUpFile,Timestamps,ID_DonViKT,ID_StatusOBKH,NguoiGoi,GioGoi,GioHen,DiaChiHenDoiSim,GhiChu,ID_KetQuaThucHien,LyDoPhanDiaBan,TieuChi,GhiChu1,GhiChu2,GhiChu3,QHHenDoiSim,PXHenDoiSim")] Sim_KH sim_KH)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(sim_KH).State = EntityState.Modified;
                var update = (from x in db.Sim_KH where x.ID == sim_KH.ID select x).Single();
                update.ID_KetQuaThucHien = sim_KH.ID_KetQuaThucHien;
                //update.NguoiGoi = sim_KH.NguoiGoi;
                //update.GioGoi = sim_KH.GioGoi;
                //update.GioHen = sim_KH.GioHen;
                //update.DiaChiHenDoiSim = sim_KH.DiaChiHenDoiSim;
                //update.QHHenDoiSim = sim_KH.QHHenDoiSim;
                //update.PXHenDoiSim = sim_KH.PXHenDoiSim;
                //update.TenPhong = sim_KH.TenPhong;
                //update.GhiChu = sim_KH.GhiChu;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            ViewBag.ID_KetQuaThucHien = new SelectList(db.DMKetQuaThucHiens, "ID", "StatusName", sim_KH.ID_KetQuaThucHien);
            ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);
            return View(sim_KH);
        }

        //ThaiHN thêm
        public JsonResult Get_PX(string qh)
        {
            string ds_px = "";
            List<string> ds_px_ = new List<string>();
            foreach (var item in db.DMQHPXes.Where(x => x.Ten_QH == qh).ToList())
            {
                ds_px += item.Ten_PX + ",";
                ds_px_.Add(item.Ten_PX);
            }
            return Json(ds_px, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get_TenPhong(string px)
        {
            string tenphong = "";
            tenphong = db.DMQHPXes.First(x => x.Ten_PX == px).TenPhong;
            return Json(tenphong, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditOutBound(int? id)
        {
            //ThaiHN thêm
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 5)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_KH sim_KH = db.Sim_KH.Find(id);
            if (sim_KH == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            ViewBag.ID_StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);

            //ThaiHN Thêm
            ViewBag.DSPhong = new SelectList(db.DMPhongs.ToList().Select(x => new { id = x.TenDonVi, TenDV = x.TenDonVi }), "id", "TenDV");

            ViewBag.list_qh = db.DMQHPXes.GroupBy(x => x.Ten_QH).Select(g => g.FirstOrDefault());
            ViewBag.list_px = db.DMQHPXes.Distinct().GroupBy(x => x.Ten_PX).Select(g => g.FirstOrDefault());
            ViewBag.QH = sim_KH.QHHenDoiSim;
            ViewBag.PX = sim_KH.PXHenDoiSim;
            ViewBag.SoDT_ = sim_KH.SoDT;
            //Hết thêm
            return View(sim_KH);
        }
        //ThaiHN thêm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOutBound([Bind(Include = "ID,SoDT,Loai_TB,BTS_Name,Code,Model,Dia_Chi_BTS,Don_Vi,TTVT,Dia_Chi_BTS2,Phuong_Xa_ID,Quan_Huyen_ID,Ve_Tinh_ID,TenPhong,Ten_TT,Ten_QH,Ten_PX,Doi_ID,Ten_Doi,TT_ID,TimeUpFile,UserUpFile,Timestamps,ID_DonViKT,ID_StatusOBKH,NguoiGoi,GioGoi,GioHen,DiaChiHenDoiSim,GhiChu,ID_KetQuaThucHien,LyDoPhanDiaBan,TieuChi,GhiChu1,GhiChu2,GhiChu3,QHHenDoiSim,PXHenDoiSim")] Sim_KH sim_KH)
        {
            //Chưa cập nhật KQ thực hiện
            if (ModelState.IsValid)
            {
                //db.Entry(sim_KH).State = EntityState.Modified;
                var update = (from x in db.Sim_KH where x.ID == sim_KH.ID select x).Single();
                update.ID_StatusOBKH = sim_KH.ID_StatusOBKH;
                update.NguoiGoi = sim_KH.NguoiGoi;
                update.GioGoi = sim_KH.GioGoi;
                update.GioHen = sim_KH.GioHen;
                update.DiaChiHenDoiSim = sim_KH.DiaChiHenDoiSim;
                update.QHHenDoiSim = sim_KH.QHHenDoiSim;
                update.PXHenDoiSim = sim_KH.PXHenDoiSim;
                update.TenPhong = sim_KH.TenPhong;
                update.GhiChu = sim_KH.GhiChu;
                db.SaveChanges();
                //Insert Sim_KH_NhatKyUpdate
                var simkh_nkupd = new Sim_KH_NhatKyUpdate();
                simkh_nkupd.ID_Sim_KH = sim_KH.ID;
                simkh_nkupd.SoDT = sim_KH.SoDT;
                simkh_nkupd.ID_StatusOBKH = sim_KH.ID_StatusOBKH;
                simkh_nkupd.NguoiGoi = sim_KH.NguoiGoi;
                simkh_nkupd.GioGoi = sim_KH.GioGoi;
                simkh_nkupd.GioHen = sim_KH.GioHen;
                simkh_nkupd.DiaChiHenDoiSim = sim_KH.DiaChiHenDoiSim;
                simkh_nkupd.QHHenDoiSim = sim_KH.QHHenDoiSim;
                simkh_nkupd.PXHenDoiSim = sim_KH.PXHenDoiSim;
                simkh_nkupd.TenPhong = sim_KH.TenPhong;
                simkh_nkupd.GhiChu = sim_KH.GhiChu;
                db.Sim_KH_NhatKyUpdate.Add(simkh_nkupd);
                db.SaveChanges();
                //End Insert Sim_KH_NhatKyUpdate

                var getkh = (from x in db.Sim_KH where x.ID == sim_KH.ID select x).Single();
                var sdt = getkh.SoDT;
                string queryString = "?Ten_QH=all&ID_StatusOB=all&ID_KetQuaThucHien=all&SoDT=" + sdt;
                return Redirect(Url.Action("Index") + queryString);
            }
            ViewBag.ID_DonViKT = new SelectList(db.DMDonViKTs, "ID", "TenDonVi", sim_KH.ID_DonViKT);
            ViewBag.StatusOBKH = new SelectList(db.DMStatusOBKHs, "ID", "StatusName", sim_KH.ID_StatusOBKH);
            return View(sim_KH);
        }
        // GET: OutBound/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 4)
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sim_KH sim_KH = db.Sim_KH.Find(id);
            if (sim_KH == null)
            {
                return HttpNotFound();
            }
            return View(sim_KH);
        }

        // POST: OutBound/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sim_KH sim_KH = db.Sim_KH.Find(id);
            db.Sim_KH.Remove(sim_KH);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //ThaiHN thêm
        public ActionResult ToggleStatus(int id)
        {
            if (Session[UserSession.ISLOGIN] == null || !(bool)Session[UserSession.ISLOGIN])
                return RedirectToAction("Index", "Login");

            if (user.PERMISSION > 5)
                return RedirectToAction("Index", "Login");

            int id_sim_kh = Convert.ToInt32(id);
            bool toggle = Model.ToggleStatus(id_sim_kh);
            //DoiSum không dùng
            //if (toggle)
            //{
            //    TempData["status_id"] = true;
            //    TempData["status"] = "Đã Thay Đổi Trạng Thái Đề Thi " + id_test;
            //}
            return RedirectToAction("Index/" + id_sim_kh);
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
