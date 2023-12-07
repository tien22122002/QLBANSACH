using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBANSACH.Models
{
    public class DonDaThue
    {

        public int MaDonThue { get; set; }
        public bool DaThanhToan { get; set; }
        public bool TinhTrangGiaoHang { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayGiao { get; set; }
        public int MaKH { get; set; }
        public string TrangThaiThanhToan { get; internal set; }
        public string TrangThaiGiaoHang { get; internal set; }
        public List<ChiTietDonDaThue> ChiTietDonDatThues { get; set; }
        public decimal TongGiaTri { get; internal set; }
        public object TenNguoiNhan { get; internal set; }
        public object DiaChiNguoiNhan { get; internal set; }
        public object SoDienThoaiNguoiNhan { get; internal set; }
        public int CountSach { get; internal set; }
        public int? MaTinhTrang { get; internal set; }
        public string TenTinhTrang { get; internal set; }
    }

    public class ChiTietDonDaThue
    {
        internal int ThanhTien;
        public int MaDonThue { get; set; }
        public int MaSach { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public SACH Sach { get; set; }
        public LOAITHUE LoaiThue { get; set; }
        public int? MaLoaiThue { get; internal set; }
        public int? ThoiGianThue { get; internal set; }
        public DateTime? NgayTra { get; internal set; }
        public string TinhTrangTraSach { get; internal set; }
        public string TenLoaiThue { get; internal set; }
    }
}