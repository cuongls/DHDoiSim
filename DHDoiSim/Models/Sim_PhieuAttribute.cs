using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DHDoiSim.Models
{
    [MetadataType(typeof(Sim_PhieuAttribs))]
    public partial class Sim_Phieu
    {
        // leave it empty.
    }

    public class Sim_PhieuAttribs
    {

        public int ID { get; set; }
        public Nullable<int> ID_Sim_KH { get; set; }
        public Nullable<int> ID_Sim_Trang { get; set; }
        public string SoDT { get; set; }
        public string LoaiTB { get; set; }
        public string LoaiDienThoai { get; set; }
        public string ModelDienThoai { get; set; }
        [Display(Name = "Quận/Huyện hẹn đổi sim")]
        public string Ten_QH { get; set; }
        [Display(Name = "Phường/Xã hẹn đổi sim")]
        public string Ten_PX { get; set; }
        [Display(Name = "Đ/C hẹn đổi sim")]
        public string DiaChiHenDoiSim { get; set; }
        [Display(Name = "Ngày hẹn")]
        public Nullable<System.DateTime> GioHen { get; set; }

        [Display(Name = "Ngày hẹn mới")]
        public Nullable<System.DateTime> NgayHenMoi { get; set; }

        [Display(Name = "Mức ưu tiên")]
        public Nullable<int> ID_MucUuTien { get; set; }
        public Nullable<int> ID_Phong { get; set; }
        public Nullable<int> ID_To { get; set; }
        public Nullable<System.DateTime> TimeXP { get; set; }
        public string UserXP { get; set; }
        public string NoiDungXP { get; set; }
        [Display(Name = "Time chia phiếu")]
        public Nullable<System.DateTime> TimeChiaPhieu { get; set; }
        [Display(Name = "User chia phiếu")]
        public string UserChiaPhieu { get; set; }
        [Display(Name = "Nội dung chia phiếu")]
        public string NoiDungChiaPhieu { get; set; }
        public Nullable<System.DateTime> TimeNhanPhieu { get; set; }
        public string UserNhanPhieu { get; set; }
        public string NoiDungNhanPhieu { get; set; }
        public Nullable<System.DateTime> TimeCapNhatKetQuaThucHien { get; set; }

        [Display(Name = "Người đổi sim")]
        public string UserThucHien { get; set; }

        //[Required]
        [Display(Name = "Ngày đổi sim")]
        public Nullable<System.DateTime> DateThucHien { get; set; }

        public string NoiDungThucHien { get; set; }

        //[Range(3, 4, ErrorMessage = "Chọn đúng KQTH")]
        [Display(Name = "Kết quả thực hiện")]
        public Nullable<int> ID_KetQuaThucHien { get; set; }

        //[Required]
        [Display(Name = "Ảnh mặt trước CMT")]
        public string Image_CMT_Truoc { get; set; }

        //[Required]
        [Display(Name = "Ảnh mặt sau CMT")]
        public string Image_CMT_Sau { get; set; }

        //[Required]
        [Display(Name = "Ảnh Biên bản")]
        public string Image_BienBan { get; set; }
 
        //[Required]
        [Display(Name = "Ảnh CK không C.Chủ")]
        public string Image_CamKetKhongCC { get; set; }

        public string SerialNumber { get; set; }

        [Display(Name = "Ghi chú xuất phiếu")]
        [DataType(DataType.MultilineText)]
        public string GhiChu { get; set; }

        [Display(Name = "Time khóa phiếu")]
        public Nullable<System.DateTime> TimeKhoaPhieu { get; set; }
        [Display(Name = "User khóa phiếu")]
        public string UserKhoaPhieu { get; set; }

        public Nullable<int> ID_DonViKT { get; set; }

        //public virtual DMDonViKT DMDonViKT { get; set; }

        public virtual DMKetQuaThucHien DMKetQuaThucHien { get; set; }

        public virtual DMMucUuTien DMMucUuTien { get; set; }
        public virtual DMPhong DMPhong { get; set; }
        public virtual DMTo DMTo { get; set; }
        public virtual Sim_KH Sim_KH { get; set; }


        //Your attribs will come here.
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        //[Display(Name = "Tên_công_việc")]
        //[DataType(DataType.MultilineText)]
        //public string TenCongViec { get; set; }
        //[Display(Name = "Mức độ phức tạp")]

        //[Range(1, int.MaxValue, ErrorMessage = "Must select")]
        //[Display(Name = "Khối lượng (lần)")]
        //[DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        //[Range(0.1, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        //[Required(AllowEmptyStrings = false)]

        //public int ID { get; set; }

        //[Display(Name = "Tên thiết bị")]
        //public string TenThietBi { get; set; }

        //[Display(Name = "Quy cách thiết bị")]
        //public string QuyCachThietBi { get; set; }
        //public Nullable<int> ID_ChungLoai { get; set; }

        //[Display(Name = "Mã Tài sản/CCDC")]
        //public string MaTS { get; set; }
        //public Nullable<int> ID_DVSD { get; set; }

        //[Display(Name = "Vị trí lắp đặt (Tổ/Phòng)")]
        //public string ViTriLapDat { get; set; }

        //[Display(Name = "Bắt đầu sử dụng")]
        //public string BatDauSuDung { get; set; }

        //[Display(Name = "Chu kỳ bảo dưỡng")]
        //public Nullable<int> ChuKyBaoDuong { get; set; }

        //[Display(Name = "Ngày KTBD cuối")]
        //public Nullable<System.DateTime> NgayKTBDCuoi { get; set; }

        //[Display(Name = "Ngày KTBD tiếp theo")]
        //public Nullable<System.DateTime> NgayKTBDTiepTheo { get; set; }

        //[Display(Name = "Dòng không tải")]
        //public string DongKhongTaiTrenFullTaiCuoi { get; set; }

        //[Display(Name = "Nội dung KTBD cuối")]
        //public string NoiDungKTBDCuoi { get; set; }

        //[Display(Name = "Ghi chú")]
        //public string GhiChu { get; set; }

        //public Nullable<int> ID_TrangThai { get; set; }

        //[Display(Name = "Số ngày quá hạn")]
        //public Nullable<int> SNQuaHan { get; set; }

        //[Display(Name = "Ngày nhập")]
        //public Nullable<System.DateTime> NgayNhap { get; set; }

        //[Display(Name = "Người nhập")]
        //public string NguoiNhap { get; set; }
        //public string SerialNumber { get; set; }

        //[Display(Name = "Năm sản xuất")]
        //public string NamSanXuat { get; set; }

        //public virtual DMChungLoaiDieuHoa DMChungLoaiDieuHoa { get; set; }
        //public virtual DMDVSD DMDVSD { get; set; }
        //public virtual DMTrangThaiDieuHoa DMTrangThaiDieuHoa { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NhatKyBienDongDieuHoa> NhatKyBienDongDieuHoas { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SCBDDieuHoa> SCBDDieuHoas { get; set; }
    }
}
