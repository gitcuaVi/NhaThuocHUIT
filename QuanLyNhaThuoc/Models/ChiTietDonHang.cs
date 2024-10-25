using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;

namespace QuanLyNhaThuoc.Models
{
    public partial class ChiTietDonHang
    {
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public int SoLuong { get; set; }
        public int MaThuoc { get; set; }
        public decimal Gia { get; set; }

        public virtual DonHang MaDonHangNavigation { get; set; } = null!;
        [NotMapped]
        //public DonHang DonHang { get; set; }
        public  Thuoc MaThuocNavigation { get; set; } = null!;
        //public Thuoc Thuoc { get; set; }
        // public object DonHang { get; internal set; }
        // Điều hướng đến DonHang
        public DonHang DonHang { get; set; }

        // Điều hướng đến Thuoc
        public  Thuoc Thuoc { get; set; }
    }
}
