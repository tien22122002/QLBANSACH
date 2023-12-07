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
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            if (Session["Taikhoan"] != null)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var matkhaunhaplai = collection["Matkhaunhaplai"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loil"] = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            }
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được bỏ trống";
            }
            if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập điện thoai";
            }
            else
            {
                //Gần giá trị cho đối tượng được tạo mới (kh)
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
            
            if (Session["Taikhoan"] != null)
            {
                return RedirectToAction("Index", "BookStore");
            }
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            // Gần các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["TenDN"]; 
            var matkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //Gần giá trị cho đối tượng được tạo mới (kh)
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau); if (kh != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công"; 
                    Session["Taikhoan"] = kh;
                    TempData["SuccessMessage"] = "Chào mừng "+ kh.HoTen +" đến với TienBook ❤️." ;
                    return RedirectToAction("Index", "BookStore");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();

        }
        public ActionResult MyProfile()
        {
            if (Session["Taikhoan"] != null)
            {
                KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
                return View(kh); // Trả về view và truyền dữ liệu khách hàng vào view
            }
            else
            {
                return RedirectToAction("Dangnhap"); // Ví dụ: Chuyển hướng đến trang đăng nhập nếu không có thông tin khách hàng
            }
        }
        [HttpPost]
        public ActionResult UpdateProfile(string HoTen, string TaiKhoan, string Email, string DiaChi, string DienThoai, string NgaySinh)
        {
            try
            {
                var khachHang = data.KHACHHANGs.FirstOrDefault(kh => kh.Taikhoan == TaiKhoan);
                if (khachHang != null)
                {
                    // Cập nhật thông tin khách hàng
                    khachHang.HoTen = HoTen;
                    khachHang.Taikhoan = TaiKhoan;
                    khachHang.Email = Email;
                    khachHang.DiachiKH = DiaChi;
                    khachHang.DienthoaiKH = DienThoai;
                    khachHang.Ngaysinh = DateTime.Parse(NgaySinh);

                    data.SubmitChanges();
                    Session["Taikhoan"] = khachHang;
                    // Trả về "success" nếu cập nhật thành công
                    return Content("success");
                }
                else
                {
                    // Không tìm thấy khách hàng
                    return Content("error");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Content("error");
            }
        }
        [HttpPost]
        public ActionResult ChangePassword(string TaiKhoan,string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                // Lấy thông tin khách hàng từ session
                var khachHang = data.KHACHHANGs.FirstOrDefault(kh => kh.Taikhoan == TaiKhoan);
                //KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];

                if (khachHang != null)
                {
                    // Kiểm tra mật khẩu cũ
                    if (KhachHangCoMatKhauCu(khachHang, CurrentPassword))
                    {
                        // Kiểm tra xác nhận mật khẩu mới
                        if (NewPassword == ConfirmPassword)
                        {
                            // Thực hiện thay đổi mật khẩu
                            khachHang.Matkhau = NewPassword;
                            
                                // Thực hiện cập nhật dữ liệu
                            data.SubmitChanges();
                            

                            // Lưu thông tin khách hàng đã cập nhật vào session
                            Session["Taikhoan"] = khachHang;

                            // Trả về "success" nếu thay đổi mật khẩu thành công
                            return Content("success");
                        }
                        else
                        {
                            // Xác nhận mật khẩu mới không khớp
                            return Content("passwordMismatch");
                        }
                    }
                    else
                    {
                        // Mật khẩu cũ không đúng
                        return Content("incorrectPassword");
                    }
                }
                else
                {
                    // Không tìm thấy thông tin khách hàng trong session
                    return Content("error");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Content("error");
            }
        }

        private bool KhachHangCoMatKhauCu(KHACHHANG kh, string matKhauCu)
        {
            // Thực hiện kiểm tra mật khẩu cũ ở đây, ví dụ:
            return kh.Matkhau == matKhauCu;
        }



    }
}