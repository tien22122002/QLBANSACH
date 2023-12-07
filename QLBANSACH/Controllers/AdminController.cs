using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Routing;
using System.Data.Linq.Mapping;

namespace QLBANSACH.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        dbQLBanSachDataContext db = new dbQLBanSachDataContext();
        /*public dbQLBanSachDataContext() :
            base(global::System.Configuration.ConfigurationManager.ConnectionStrings["QLBANSACH_oldConection"].ConnectionString, mappingSource)
        {
            OnCreated();
        }*/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dangxuat()
        {
            Session["Taikhoanadein"] = null;
            return RedirectToAction("Login", "LoginAdmin");
        }
        public ActionResult Sach(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoisach()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisach(SACH sach, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu duong dan cua file.
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/image"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sach");
            }
        }
        public ActionResult Chitietsach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        public ActionResult Xoasach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");

        }

        public ActionResult Suasach(int id)
        {
            // Lấy thông tin sách từ cơ sở dữ liệu dựa trên id
            SACH sach = db.SACHes.SingleOrDefault(s => s.Masach == id);

            if (sach != null)
            {
                ViewBag.MaCDList = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
                ViewBag.MaNXBList = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
                return View(sach);
            }
            else
            {
                // Xử lý trường hợp không tìm thấy sách
                return HttpNotFound();
            }

        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                // Tìm sách trong cơ sở dữ liệu bằng ID
                ViewBag.MaCDList = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
                ViewBag.MaNXBList = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
                var existingSach = db.SACHes.SingleOrDefault(s => s.Masach == sach.Masach);

                if (existingSach != null)
                {
                    // Kiểm tra xem người dùng đã chọn ảnh bìa mới hay chưa

                    if (fileUpload != null)
                    {
                        // Lưu tên file, lưu ý bổ sung thư viện System.IO
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/image"), fileName);

                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại!";
                        }
                        else
                        {
                            // Lưu hình ảnh vào đường dẫn
                            fileUpload.SaveAs(path);
                        }
                        existingSach.Anhbia = fileName;
                    }

                    // Cập nhật thông tin sách từ dữ liệu nhập từ form
                    existingSach.Tensach = sach.Tensach;
                    existingSach.Giaban = sach.Giaban;
                    existingSach.Mota = sach.Mota;
                    existingSach.Ngaycapnhat = sach.Ngaycapnhat;
                    existingSach.Soluongton = sach.Soluongton;
                    existingSach.MaCD = sach.MaCD;
                    existingSach.MaNXB = sach.MaNXB;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();

                    // Chuyển hướng về trang danh sách sách hoặc trang chi tiết sách
                    return RedirectToAction("Index"); // Thay "DanhSachSach" bằng tên action hoặc route cần chuyển hướng
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy sách
                    return HttpNotFound();
                }
            }
            else
            {
                return View(sach);
            }

        }
        



    }
}