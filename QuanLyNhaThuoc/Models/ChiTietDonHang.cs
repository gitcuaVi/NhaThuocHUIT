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

        // Điều hướng đến DonHang
        public virtual DonHang DonHang { get; set; } = null!;

        // Điều hướng đến Thuoc
        public virtual Thuoc Thuoc { get; set; } = null!;
    }

}
