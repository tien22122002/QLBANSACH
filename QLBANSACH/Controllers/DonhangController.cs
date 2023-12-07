using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class DonhangController : Controller
    {
        // GET: Donhang
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        private decimal tongGiaTriDonHang;
        private decimal tongGiaTriDonThue;

        public ActionResult DonHangDaDat(decimal? tongGiaTri)
        {
            try
            {
                // Lấy thông tin khách hàng từ Session
                KHACHHANG khachHang = (KHACHHANG)Session["Taikhoan"];

                if (khachHang != null)
                {
                    var donHangs = data.DONDATHANGs.Where(dh => dh.MaKH == khachHang.MaKH).ToList();
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
                else
                {
                    // Không tìm thấy thông tin khách hàng trong Session
                    return RedirectToAction("Index", "BookStore");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "BookStore");
            }
        }
        public ActionResult DonDaThue(decimal? tongGiaTri)
        {
            try
            {
                // Lấy thông tin khách hàng từ Session
                KHACHHANG khachHang = (KHACHHANG)Session["Taikhoan"];

                if (khachHang != null)
                {
                    var donHangs = data.DONDATHANGs.Where(dh => dh.MaKH == khachHang.MaKH).ToList();
                    List<DonDatHang> donDatHangs = new List<DonDatHang>();

                    foreach (var donHang in donHangs)
                    {
                        var chiTietDonThueList = data.CHITIETDONTHUEs.Where(ct => ct.MaDonThue == donHang.MaDonHang).ToList();
                        List<ChiTietDonDaThue> chiTietDonThues = new List<ChiTietDonDaThue>();
                        var tinhtrang = data.TINHTRANGDONs.SingleOrDefault(z => z.MaTinhTrang == donHang.MaTinhTrang);
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
                                    TenLoaiThue = chiTietDonThue.LOAITHUE.TenLoaiThue,
                                    TinhTrangTraSach = chiTietDonThue.TinhTrangTraHang.HasValue ? (chiTietDonThue.TinhTrangTraHang.Value ? "Đã trả sách" : "Đang trả sách") : "Chưa trả sách"

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
                            ChiTietDonThues = chiTietDonThues,
                            TrangThaiThanhToan = donHang.Dathanhtoan.HasValue ? (donHang.Dathanhtoan.Value ? "Đã thanh toán" : "Chưa thanh toán") : "Không xác định",
                            TrangThaiGiaoHang = donHang.TINHTRANGDON.TenTinhTrang,
                            tongGiaTri = tongGiaTriDonThue,
                            TenNguoiNhan = donHang.KHACHHANG.HoTen,
                            DiaChiNguoiNhan = donHang.KHACHHANG.DiachiKH,
                            SoDienThoaiNguoiNhan = donHang.KHACHHANG.DienthoaiKH
                        };
                        if(chiTietDonThues != null && chiTietDonThues.Any())
                        {
                            donDatHangs.Add(donDatHang);
                        }
                    }

                    return View(donDatHangs);
                }
                else
                {
                    // Không tìm thấy thông tin khách hàng trong Session
                    return RedirectToAction("Index", "BookStore");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "BookStore");
            }
        }
        public ActionResult ChiTietDonHang(decimal? tongGiaTri, int maDonHang)
        {
            try
            {
                // Lấy thông tin khách hàng từ Session
                KHACHHANG khachHang = (KHACHHANG)Session["Taikhoan"];

                if (khachHang != null)
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
                else
                {
                    // Không tìm thấy thông tin khách hàng trong Session
                    return RedirectToAction("Index", "BookStore");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "BookStore");
            }
        }
        [HttpGet]
        public ActionResult TraSach(decimal? tongGiaTri, int maDonHang)
        {
            try
            {
                // Lấy thông tin khách hàng từ Session
                KHACHHANG khachHang = (KHACHHANG)Session["Taikhoan"];

                if (khachHang != null)
                {
                    var donHangs = data.DONDATHANGs.Where(dh => dh.MaKH == khachHang.MaKH && dh.MaDonHang == maDonHang).ToList();
                    List<DonDatHang> donDatHangs = new List<DonDatHang>();

                    foreach (var donHang in donHangs)
                    {
                        var chiTietDonThueList = data.CHITIETDONTHUEs.Where(ct => ct.MaDonThue == donHang.MaDonHang).ToList();
                        List<ChiTietDonDaThue> chiTietDonThues = new List<ChiTietDonDaThue>();
                        var tinhtrang = data.TINHTRANGDONs.SingleOrDefault(z => z.MaTinhTrang == donHang.MaTinhTrang);
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
                                    TenLoaiThue = chiTietDonThue.LOAITHUE.TenLoaiThue,
                                    TinhTrangTraSach = chiTietDonThue.TinhTrangTraHang.HasValue ? (chiTietDonThue.TinhTrangTraHang.Value ? "Đã trả sách": "Đang trả sách") : "Chưa trả sách"

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
                            ChiTietDonThues = chiTietDonThues,
                            TrangThaiThanhToan = donHang.Dathanhtoan.HasValue ? (donHang.Dathanhtoan.Value ? "Đã thanh toán" : "Chưa thanh toán") : "Không xác định",
                            TrangThaiGiaoHang = donHang.TINHTRANGDON.TenTinhTrang,
                            tongGiaTri = tongGiaTriDonThue,
                            TenNguoiNhan = donHang.KHACHHANG.HoTen,
                            DiaChiNguoiNhan = donHang.KHACHHANG.DiachiKH,
                            SoDienThoaiNguoiNhan = donHang.KHACHHANG.DienthoaiKH
                        };
                        if (chiTietDonThues != null && chiTietDonThues.Any())
                        {
                            donDatHangs.Add(donDatHang);
                        }
                    }

                    return View(donDatHangs);
                }
                else
                {
                    // Không tìm thấy thông tin khách hàng trong Session
                    return RedirectToAction("Index", "BookStore");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "BookStore");
            }
        }
        [HttpPost]
        public ActionResult TraSach(List<int> selectedSach, int maDonHang, DateTime dNgaygui)
        {
            if (selectedSach != null && selectedSach.Any())
            {
                var donHangs = data.DONDATHANGs.SingleOrDefault(dh => dh.MaDonHang == maDonHang);
                foreach (var maSach in selectedSach)
                {
                    var chiTiet = data.CHITIETDONTHUEs.SingleOrDefault(ct => ct.MaDonThue == maDonHang && ct.Masach == maSach);
                    if(chiTiet != null)
                    {
                        CHITIETDONTRA ctdt = new CHITIETDONTRA();
                        ctdt.MaDonHang = maDonHang;
                        ctdt.MaSach = maSach;
                        ctdt.SoLuong = chiTiet.SoLuong;
                        ctdt.NgayTra = dNgaygui;
                        data.CHITIETDONTRAs.InsertOnSubmit(ctdt);
                        chiTiet.TinhTrangTraHang = false;
                    }
                    
                }
                data.SubmitChanges();
                ViewBag.ShowSuccessModal = true;
                return RedirectToAction("DonDaThue", "DonHang");
            }
            else
            {
                // Nếu không có sách nào được chọn, bạn có thể hiển thị thông báo lỗi hoặc thực hiện xử lý khác
                return View("Error");
            }
        }

    }
}