using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace QLBANSACH.Models
{
    public class Giohang
    {
        dbQLBanSachDataContext data = new dbQLBanSachDataContext();
        public int iMasach { set; get; }
        public int iMaloaithue { set; get; }
        public string sTensach { set; get; }
        public string sAnhbia { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }
        public string sTenloaithue { set; get; }
        public Double dTilegia { set; get; }
        public DateTime NgayGiaoDuKien { get; set; }
        public DateTime NgayTra { get; set; }
        public List<LOAITHUE> LoaiThueList { get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia * dTilegia; }
        }
        public Giohang(int Masach)
        {
            iMasach = Masach;
            SACH sach = data.SACHes.Single(n => n.Masach == iMasach);
            sTensach = sach.Tensach;
            sAnhbia = sach.Anhbia;
            dDongia = double.Parse(sach.Giaban.ToString());
            iSoluong = 1;
            LoaiThueList = data.LOAITHUEs.ToList();
            var ngayLoaiThue = LoaiThueList.FirstOrDefault(l => l.MaLoaiThue == 1);
            if (ngayLoaiThue != null)
            {
                iMaloaithue = ngayLoaiThue.MaLoaiThue;
                sTenloaithue = ngayLoaiThue.TenLoaiThue;
                dTilegia = double.Parse(ngayLoaiThue.TiLeGia.ToString());
            }
        }
    }
}
