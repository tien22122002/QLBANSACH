using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class DsdonhangController : BaseController
    {
        // GET: Dsdonhang
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        private decimal tongGiaTriDonHang;
        private decimal tongGiaTriDonThue;

        public ActionResult DSDonHang()
        {
            var donHangs = data.DONDATHANGs.ToList();
            List<DonDatHang> donDatHangs = new List<DonDatHang>();

            foreach (var donHang in donHangs)
            {
                var chiTietDonHangs = data.CHITIETDONTHANGs.Where(ct => ct.MaDonHang == donHang.MaDonHang).ToList();
                List<ChiTietDonDatHang> chiTietDonDatHangs = new List<ChiTietDonDatHang>();

                var chiTietDonThueList = data.CHITIETDONTHUEs.Where(ct => ct.MaDonThue == donHang.MaDonHang).ToList();
                List<ChiTietDonDaThue> chiTietDonThues = new List<ChiTietDonDaThue>();
                
                var tinhtrang = data.TINHTRANGDONs.SingleOrDefault(z => z.MaTinhTrang == donHang.MaTinhTrang);
                tongGiaTriDonHang = 20000;
                tongGiaTriDonThue = 0;

                int countSach = chiTietDonHangs.Count + chiTietDonThueList.Count;
                if(countSach > 1 )
                {
                    countSach += 1;
                }
                if(countSach == 0)
                {
                    countSach = 1;
                }
                
                foreach (var chiTietDonHang in chiTietDonHangs)
                {
                    var loaiThue = data.LOAITHUEs.SingleOrDefault(l => l.MaLoaiThue == chiTietDonHang.MaLoaiThue);
                    var sach = data.SACHes.SingleOrDefault(s => s.Masach == chiTietDonHang.Masach);
                    if (sach != null)
                    {
                        var chiTietDonDatHang = new ChiTietDonDatHang
                        {
                            MaDonHang = chiTietDonHang.MaDonHang,
                            MaSach = chiTietDonHang.Masach,
                            SoLuong = chiTietDonHang.Soluong ?? 0,
                            DonGia = chiTietDonHang.Dongia ?? 0,
                            Sach = sach,
                            TenLoaiThue = chiTietDonHang.LOAITHUE.TenLoaiThue
                        };

                        chiTietDonDatHangs.Add(chiTietDonDatHang);
                        tongGiaTriDonHang += chiTietDonDatHang.SoLuong * chiTietDonDatHang.DonGia;
                    }
                }
                
                foreach (var chiTietDonThue in chiTietDonThueList)
                {
                    var loaithue = data.LOAITHUEs.SingleOrDefault(l => l.MaLoaiThue == chiTietDonThue.MaLoaiThue);
                    var sach = data.SACHes.SingleOrDefault(s => s.Masach == chiTietDonThue.Masach);

                    if (sach != null)
                    {

                        var chiTietDonThueData = new ChiTietDonDaThue
                        {
                            MaDonThue = chiTietDonThue.MaDonThue,
                            MaSach = chiTietDonThue.Masach,
                            MaLoaiThue = chiTietDonThue.MaLoaiThue,
                            SoLuong = chiTietDonThue.SoLuong ?? 0,
                            NgayTra = chiTietDonThue.NgayTra ?? null,
                            DonGia = chiTietDonThue.DonGia ?? 0,
                            Sach = sach,
                            LoaiThue = loaithue,
                            TenLoaiThue = chiTietDonThue.LOAITHUE.TenLoaiThue

                        };
                        chiTietDonThueData.ThanhTien = (int)(chiTietDonThueData.SoLuong * chiTietDonThueData.DonGia * chiTietDonThueData.LoaiThue.TiLeGia);
                        chiTietDonThues.Add(chiTietDonThueData);
                        tongGiaTriDonThue += chiTietDonThueData.ThanhTien;
                    }
                }
                var donDatHang = new DonDatHang
                {
                    MaDonHang = donHang.MaDonHang,
                    DaThanhToan = donHang.Dathanhtoan ?? false,
                    MaTinhTrang = donHang.MaTinhTrang,
                    NgayDat = donHang.Ngaydat ?? DateTime.MinValue,
                    NgayGiao = donHang.Ngaygiao ?? DateTime.MinValue,
                    MaKH = donHang.MaKH ?? 0,
                    ChiTietDonDatHangs = chiTietDonDatHangs,
                    ChiTietDonThues = chiTietDonThues,
                    TrangThaiThanhToan = donHang.Dathanhtoan.HasValue ? (donHang.Dathanhtoan.Value ? "Đã thanh toán" : "Chưa thanh toán") : "Không xác định",
                    TrangThaiGiaoHang = donHang.TINHTRANGDON.TenTinhTrang,
                    tongGiaTri = tongGiaTriDonHang+ tongGiaTriDonThue,
                    TenNguoiNhan = donHang.KHACHHANG.HoTen,
                    SoDienThoaiNguoiNhan = donHang.KHACHHANG.DienthoaiKH,
                    DiaChiNguoiNhan = donHang.KHACHHANG.DiachiKH,
                    CountSach = countSach
                };

                donDatHangs.Add(donDatHang);
            }

            return View(donDatHangs);
        }

        [HttpPost]
        public ActionResult CapNhatGiaoHang(int maDonHang, int maTinhTrang)
        {
            // Lấy đơn hàng từ cơ sở dữ liệu
            var donHang = data.DONDATHANGs.SingleOrDefault(d => d.MaDonHang == maDonHang);

            if (donHang != null)
            {
                
                if(maTinhTrang == 1)
                {
                    donHang.MaTinhTrang = 2;
                }
                else if (maTinhTrang == 2)
                {
                    donHang.MaTinhTrang = 3;
                }
                data.SubmitChanges();
            }

            // Chuyển hướng trở lại danh sách đơn hàng
            return RedirectToAction("DSDonHang");
        }

        public ActionResult ChiTietDonHang(decimal? tongGiaTri, int maDonHang)
        {
            try
            {
                    var donHangs = data.DONDATHANGs.Where(dh => dh.MaDonHang == maDonHang).ToList();
                    List<DonDatHang> donDatHangs = new List<DonDatHang>();

                    foreach (var donHang in donHangs)
                    {
                        var chiTietDonHangs = data.CHITIETDONTHANGs.Where(ct => ct.MaDonHang == donHang.MaDonHang).ToList();
                        List<ChiTietDonDatHang> chiTietDonDatHangs = new List<ChiTietDonDatHang>();
                        var tinhtrang = data.TINHTRANGDONs.SingleOrDefault(z => z.MaTinhTrang == donHang.MaTinhTrang);
                        tongGiaTriDonHang = 20000;
                        foreach (var chiTietDonHang in chiTietDonHangs)
                        {
                            var sach = data.SACHes.SingleOrDefault(s => s.Masach == chiTietDonHang.Masach);
                            var loaiThue = data.LOAITHUEs.SingleOrDefault(l => l.MaLoaiThue == chiTietDonHang.MaLoaiThue);
                            if (sach != null)
                            {
                                var chiTietDonDatHang = new ChiTietDonDatHang
                                {
                                    MaDonHang = chiTietDonHang.MaDonHang,
                                    MaSach = chiTietDonHang.Masach,
                                    SoLuong = chiTietDonHang.Soluong ?? 0,
                                    DonGia = chiTietDonHang.Dongia ?? 0,
                                    Sach = sach,
                                    TenLoaiThue = chiTietDonHang.LOAITHUE.TenLoaiThue

                                };

                                chiTietDonDatHangs.Add(chiTietDonDatHang);
                                tongGiaTriDonHang += chiTietDonDatHang.SoLuong * chiTietDonDatHang.DonGia;
                            }
                        }
                        var chiTietDonThueList = data.CHITIETDONTHUEs.Where(ct => ct.MaDonThue == donHang.MaDonHang).ToList();
                        List<ChiTietDonDaThue> chiTietDonThues = new List<ChiTietDonDaThue>();
                        tongGiaTriDonThue = 0;
                        foreach (var chiTietDonThue in chiTietDonThueList)
                        {
                            var loaithue = data.LOAITHUEs.SingleOrDefault(l => l.MaLoaiThue == chiTietDonThue.MaLoaiThue);
                            var sach = data.SACHes.SingleOrDefault(s => s.Masach == chiTietDonThue.Masach);

                            if (sach != null)
                            {

                                var chiTietDonThueData = new ChiTietDonDaThue
                                {
                                    MaDonThue = chiTietDonThue.MaDonThue,
                                    MaSach = chiTietDonThue.Masach,
                                    MaLoaiThue = chiTietDonThue.MaLoaiThue,
                                    SoLuong = chiTietDonThue.SoLuong ?? 0,
                                    NgayTra = chiTietDonThue.NgayTra ?? null,
                                    DonGia = chiTietDonThue.DonGia ?? 0,
                                    Sach = sach,
                                    LoaiThue = loaithue,
                                    TenLoaiThue = chiTietDonThue.LOAITHUE.TenLoaiThue

                                };
                                chiTietDonThueData.ThanhTien = (int)(chiTietDonThueData.SoLuong * chiTietDonThueData.DonGia * chiTietDonThueData.LoaiThue.TiLeGia);
                                chiTietDonThues.Add(chiTietDonThueData);
                                tongGiaTriDonThue += chiTietDonThueData.ThanhTien;
                            }
                        }
                        var donDatHang = new DonDatHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            DaThanhToan = donHang.Dathanhtoan ?? false,
                            MaTinhTrang = donHang.MaTinhTrang,
                            NgayDat = donHang.Ngaydat ?? DateTime.MinValue,
                            NgayGiao = donHang.Ngaygiao ?? DateTime.MinValue,
                            MaKH = donHang.MaKH ?? 0,
                            ChiTietDonDatHangs = chiTietDonDatHangs,
                            ChiTietDonThues = chiTietDonThues,
                            TrangThaiThanhToan = donHang.Dathanhtoan.HasValue ? (donHang.Dathanhtoan.Value ? "Đã thanh toán" : "Chưa thanh toán") : "Không xác định",
                            TrangThaiGiaoHang = donHang.TINHTRANGDON.TenTinhTrang,
                            tongGiaTri = tongGiaTriDonHang + tongGiaTriDonThue,
                            TenNguoiNhan = donHang.KHACHHANG.HoTen,
                            DiaChiNguoiNhan = donHang.KHACHHANG.DiachiKH,
                            SoDienThoaiNguoiNhan = donHang.KHACHHANG.DienthoaiKH
                        };

                        donDatHangs.Add(donDatHang);
                    }

                    return View(donDatHangs);
                
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "BookStore");
            }
        }
        public ActionResult DSSachThue()
        {
            var donHangs = data.DONDATHANGs.ToList();
            List<DonDatHang> donDatHangs = new List<DonDatHang>();

            foreach (var donHang in donHangs)
            {
                var chiTietDonThueList = data.CHITIETDONTHUEs.Where(ct => ct.MaDonThue == donHang.MaDonHang).ToList();
                List<ChiTietDonDaThue> chiTietDonThues = new List<ChiTietDonDaThue>();

                var tinhtrang = data.TINHTRANGDONs.SingleOrDefault(z => z.MaTinhTrang == donHang.MaTinhTrang);

                foreach (var chiTietDonThue in chiTietDonThueList)
                {
                    var loaithue = data.LOAITHUEs.SingleOrDefault(l => l.MaLoaiThue == chiTietDonThue.MaLoaiThue);
                    var sach = data.SACHes.SingleOrDefault(s => s.Masach == chiTietDonThue.Masach);

                    if (sach != null)
                    {

                        var chiTietDonThueData = new ChiTietDonDaThue
                        {
                            MaDonThue = chiTietDonThue.MaDonThue,
                            MaSach = chiTietDonThue.Masach,
                            MaLoaiThue = chiTietDonThue.MaLoaiThue,
                            SoLuong = chiTietDonThue.SoLuong ?? 0,
                            NgayTra = chiTietDonThue.NgayTra ?? null,
                            DonGia = chiTietDonThue.DonGia ?? 0,
                            Sach = sach,
                            LoaiThue = loaithue,
                            TenLoaiThue = chiTietDonThue.LOAITHUE.TenLoaiThue,
                            TinhTrangTraSach = chiTietDonThue.TinhTrangTraHang.HasValue ? (chiTietDonThue.TinhTrangTraHang.Value ? "Đã trả sách" : "Đang trả sách") : "Chưa trả sách"

                        };
                        chiTietDonThueData.ThanhTien = (int)(chiTietDonThueData.SoLuong * chiTietDonThueData.DonGia * chiTietDonThueData.LoaiThue.TiLeGia);
                        chiTietDonThues.Add(chiTietDonThueData);
                    }
                }
                var donDatHang = new DonDatHang
                {
                    MaDonHang = donHang.MaDonHang,
                    DaThanhToan = donHang.Dathanhtoan ?? false,
                    MaTinhTrang = donHang.MaTinhTrang,
                    NgayDat = donHang.Ngaydat ?? DateTime.MinValue,
                    NgayGiao = donHang.Ngaygiao ?? DateTime.MinValue,
                    MaKH = donHang.MaKH ?? 0,
                    ChiTietDonThues = chiTietDonThues,
                    TrangThaiThanhToan = donHang.Dathanhtoan.HasValue ? (donHang.Dathanhtoan.Value ? "Đã thanh toán" : "Chưa thanh toán") : "Không xác định",
                    TrangThaiGiaoHang = donHang.TINHTRANGDON.TenTinhTrang,
                    TenNguoiNhan = donHang.KHACHHANG.HoTen,
                    SoDienThoaiNguoiNhan = donHang.KHACHHANG.DienthoaiKH,
                    DiaChiNguoiNhan = donHang.KHACHHANG.DiachiKH
                };
                if (chiTietDonThues != null && chiTietDonThues.Any())
                {
                    donDatHangs.Add(donDatHang);
                }
            }

            return View(donDatHangs);
        }
        [HttpPost]
        public ActionResult CapNhatTraHang(int maDonHang, int maSach)
        {
            // Lấy đơn hàng từ cơ sở dữ liệu
            var donHang = data.CHITIETDONTHUEs.SingleOrDefault(d => d.MaDonThue == maDonHang && d.Masach == maSach);
            if (donHang != null)
            {

                donHang.TinhTrangTraHang = true;
                data.SubmitChanges();
            }


            return RedirectToAction("DSSachThue");
        }
    }
}