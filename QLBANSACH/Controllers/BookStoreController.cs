using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using PagedList.Mvc;
namespace QLBANSACH.Controllers
{
    public class BookStoreController : Controller
    {
        // GET: BookStore
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        private List<SACH> Laysachmoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Index(int ? page)
        {
            int pageSize = 10;
            int pageNum = (page ?? 1);

            var sachmoi = Laysachmoi(36);
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(sachmoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Chude()
        {
            var chude = from cd in data.CHUDEs select cd;
            return PartialView(chude);
        }
        public ActionResult Nhaxuatban()
        {
            var nhaxuatban = from cd in data.NHAXUATBANs select cd;
            return PartialView(nhaxuatban);
        }
        public ActionResult SPTheochude(int id, int? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);

            var sach = (from s in data.SACHes where s.MaCD == id select s).ToList();
            return View(sach.ToPagedList(pageNum, pageSize));
        }

        public ActionResult SPTheoNXB(int id, int? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);

            var sach = (from s in data.SACHes where s.MaNXB == id select s).ToList();
            return View(sach.ToPagedList(pageNum, pageSize));
        }

        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes where s.Masach == id select s;
            return View(sach.Single());
        }
        public ActionResult SachGoiYCD(int maCD)
        {
            var sach = data.SACHes.Where(s => s.MaNXB == maCD).ToList();
            return View(sach);
        }
    }
}