using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int iMasach, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra sách này tồn tại trong Session["Giohang"] chưa?
            Giohang sanpham = lstGiohang.Find(n => n.iMasach == iMasach);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien) + 20000;
            }
            return iTongTien;

        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }

            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP); //Neu ton tai thị cho sua Soluong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP); return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            if (sanpham != null)
            {
                int selectedLoaiThue = int.Parse(f["ddlLoaiThue"]);
                sanpham.iMaloaithue = selectedLoaiThue;
                var loaiThue = sanpham.LoaiThueList.SingleOrDefault(l => l.MaLoaiThue == selectedLoaiThue);
                if (loaiThue != null)
                {
                    sanpham.dTilegia = double.Parse(loaiThue.TiLeGia.ToString());
                    sanpham.sTenloaithue = loaiThue.TenLoaiThue.ToString();
                }

            }
            return RedirectToAction("Giohang");
        }
        public ActionResult UpdateLoaiThue(int masach, int maLoaiThue)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == masach);

            if (sanpham != null)
            {
                sanpham.iMaloaithue = maLoaiThue;
                var loaiThue = sanpham.LoaiThueList.SingleOrDefault(l => l.MaLoaiThue == maLoaiThue);
                if (loaiThue != null)
                {
                    sanpham.dTilegia = double.Parse(loaiThue.TiLeGia.ToString());
                    sanpham.sTenloaithue = loaiThue.TenLoaiThue.ToString();
                }

                return RedirectToAction("Giohang");
            }

            return Json(new { success = false });
        }

        public ActionResult GiamSoLuong(int masach)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == masach);
            ViewBag.Mess = null;
            if (sanpham != null)
            {
                sanpham.iSoluong--;
                if(sanpham.iSoluong < 1)
                {
                    sanpham.iSoluong = 1;
                }
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult TangSoLuong(int masach)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == masach);
            var sach = data.SACHes.SingleOrDefault(n => n.Masach == masach);
            ViewBag.Mess = null;
            if (sanpham != null)
            {
                if(sanpham.iSoluong < sach.Soluongton)
                {
                    sanpham.iSoluong++;
                }
                else
                {
                    ViewBag.Mess = "Sản phẩm này đã đạt số lượng tối đa !";
                }
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "BookStore");
        }
        public ActionResult Dangxuat()
        {
            Session["Taikhoan"] = null; //Xóa đối tượng tài khoản khỏi Session
            return RedirectToAction("Index", "BookStore"); //Chuyển hướng về trang chủ
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }
            //Lay gio hang tu Session.
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"]; 
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.MaTinhTrang =1;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                if (item.iMaloaithue != 4)
                {
                    CHITIETDONTHUE ctdt = new CHITIETDONTHUE();
                    ctdt.MaDonThue = ddh.MaDonHang;
                    ctdt.Masach = item.iMasach;
                    ctdt.MaLoaiThue = item.iMaloaithue;
                    ctdt.SoLuong = item.iSoluong;
                    if (item.iMaloaithue == 1)
                    {
                        ctdt.NgayTra = ddh.Ngaygiao.Value.AddMonths(1);
                    }
                    else if (item.iMaloaithue == 2)
                    {
                        ctdt.NgayTra = ddh.Ngaygiao.Value.AddMonths(3);
                    }
                    else if (item.iMaloaithue == 3)
                    {
                        ctdt.NgayTra = ddh.Ngaygiao.Value.AddMonths(6);
                    }
                    ctdt.DonGia = (decimal)item.dDongia;
                    data.CHITIETDONTHUEs.InsertOnSubmit(ctdt);
                }
                else if (item.iMaloaithue == 4)
                {
                    CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                    ctdh.MaDonHang = ddh.MaDonHang;
                    ctdh.Soluong = item.iSoluong;
                    ctdh.Masach = item.iMasach;
                    ctdh.MaLoaiThue = item.iMaloaithue;
                    ctdh.Dongia = (decimal)item.dDongia;
                    data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
                    
                }
                var sach = data.SACHes.SingleOrDefault(n => n.Masach == item.iMasach);
                sach.Soluongton -= item.iSoluong;
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang () 
        {
            return View(); 
        }
        [HttpGet]
        public ActionResult DatNgay(int iMasach)
        {
            List<Giohang> lstMuaNgay = Session["Giohang"] as List<Giohang>;
            if (lstMuaNgay == null)
            {
                lstMuaNgay = new List<Giohang>();
                Session["MuaNgay"] = lstMuaNgay;
            }
            Giohang sanpham = lstMuaNgay.Find(n => n.iMasach == iMasach);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach);
                lstMuaNgay.Add(sanpham);
            }
            else
            {
                sanpham = null;
                sanpham = new Giohang(iMasach);
                lstMuaNgay.Add(sanpham);
            }
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Muangay"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.Tongsoluong = lstMuaNgay.Sum(n => n.iSoluong);
            ViewBag.Tongtien = lstMuaNgay.Sum(n => n.dThanhtien) + 20000;
            return View(lstMuaNgay);
        }
        [HttpPost]
        public ActionResult DatNgay(FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Session["MuaNgay"] as List<Giohang>;
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.MaTinhTrang = 1;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                if (item.iMaloaithue != 4)
                {
                    CHITIETDONTHUE ctdt = new CHITIETDONTHUE();
                    ctdt.MaDonThue = ddh.MaDonHang;
                    ctdt.Masach = item.iMasach;
                    ctdt.MaLoaiThue = item.iMaloaithue;
                    ctdt.SoLuong = item.iSoluong;
                    if (item.iMaloaithue == 1)
                    {
                        ctdt.NgayTra = ddh.Ngaygiao.Value.AddMonths(1);
                    }
                    else if (item.iMaloaithue == 2)
                    {
                        ctdt.NgayTra = ddh.Ngaygiao.Value.AddMonths(3);
                    }
                    else if (item.iMaloaithue == 3)
                    {
                        ctdt.NgayTra = ddh.Ngaygiao.Value.AddMonths(6);
                    }
                    ctdt.DonGia = (decimal)item.dDongia;
                    data.CHITIETDONTHUEs.InsertOnSubmit(ctdt);
                }
                else if (item.iMaloaithue == 4)
                {
                    CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                    ctdh.MaDonHang = ddh.MaDonHang;
                    ctdh.Soluong = item.iSoluong;
                    ctdh.Masach = item.iMasach;
                    ctdh.MaLoaiThue = item.iMaloaithue;
                    ctdh.Dongia = (decimal)item.dDongia;
                    data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);

                }
                var sach = data.SACHes.SingleOrDefault(n => n.Masach == item.iMasach);
                sach.Soluongton -= item.iSoluong;
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
    } 
}
