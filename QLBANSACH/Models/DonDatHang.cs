using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBANSACH.Models
{
    public class DonDatHang
    {
        internal decimal TongGiaTri;

        public int MaDonHang { get; set; }
        public bool DaThanhToan { get; set; }
        public bool TinhTrangGiaoHang { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayGiao { get; set; }
        public int MaKH { get; set; }
        public List<ChiTietDonDatHang> ChiTietDonDatHangs { get; set; }
        public string TrangThaiThanhToan { get; internal set; }
        public string TrangThaiGiaoHang { get; internal set; }
        public decimal tongGiaTri { get; internal set; }
        public object TenNguoiNhan { get; internal set; }
        public object DiaChiNguoiNhan { get; internal set; }
        public object SoDienThoaiNguoiNhan { get; internal set; }
        public int CountSach { get; internal set; }
        public int? MaTinhTrang { get; internal set; }
        public List<ChiTietDonDaThue> ChiTietDonThues { get; internal set; }
    }

    public class ChiTietDonDatHang
    {
        public int MaDonHang { get; set; }
        public int MaSach { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public SACH Sach { get; set; }
        public string TenLoaiThue { get; internal set; }
    }

}