using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DHDoiSim.Models
{
    [MetadataType(typeof(Sim_KHAttribs))]
    public partial class Sim_KH
    {
        // leave it empty.
    }

    public class Sim_KHAttribs
    {
        public int ID { get; set; }
        public string SoDT { get; set; }
        public string Loai_TB { get; set; }
        public string BTS_Name { get; set; }
        public string Code { get; set; }
        public string Model { get; set; }
        public string Dia_Chi_BTS { get; set; }
        public string Don_Vi { get; set; }
        public string TTVT { get; set; }
        public string Dia_Chi_BTS2 { get; set; }
        public Nullable<int> Phuong_Xa_ID { get; set; }
        public Nullable<int> Quan_Huyen_ID { get; set; }
        public Nullable<int> Ve_Tinh_ID { get; set; }
        public string TenPhong { get; set; }
        public string Ten_TT { get; set; }
        public string Ten_QH { get; set; }
        public string Ten_PX { get; set; }
        public Nullable<int> Doi_ID { get; set; }
        public string Ten_Doi { get; set; }
        public Nullable<int> TT_ID { get; set; }
        [Display(Name = "Ngày nhập")]
        public Nullable<System.DateTime> TimeUpFile { get; set; }
        [Display(Name = "Người nhập")]
        public string UserUpFile { get; set; }
        public Nullable<System.DateTime> Timestamps { get; set; }
        public Nullable<int> ID_DonViKT { get; set; }
        public Nullable<int> ID_StatusOBKH { get; set; }
        public string NguoiGoi { get; set; }
        public Nullable<System.DateTime> GioGoi { get; set; }

        [Display(Name = "Ngày hẹn")]
        public Nullable<System.DateTime> GioHen { get; set; }

        [Display(Name = "Đ/C hẹn đổi sim")]
        public string DiaChiHenDoiSim { get; set; }

        public string GhiChu { get; set; }
        public Nullable<int> ID_KetQuaThucHien { get; set; }
        public string LyDoPhanDiaBan { get; set; }
        public string TieuChi { get; set; }
        public string GhiChu1 { get; set; }
        public string GhiChu2 { get; set; }
        [Display(Name = "Tên phòng theo Đ/C BTS")]
        public string GhiChu3 { get; set; }

        [Display(Name = "Quận huyện hẹn đổi sim")]
        public string QHHenDoiSim { get; set; }

        [Display(Name = "Phường xã hẹn đổi sim")]
        public string PXHenDoiSim { get; set; }

        public virtual DMDonViKT DMDonViKT { get; set; }
        public virtual DMKetQuaThucHien DMKetQuaThucHien { get; set; }
        public virtual DMStatusOBKH DMStatusOBKH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sim_Phieu> Sim_Phieu { get; set; }

    }
}
