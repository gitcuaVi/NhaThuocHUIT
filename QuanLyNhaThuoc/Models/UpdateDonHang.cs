using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaThuoc.Models
{
    public class UpdateDonHang
    {
        public int MaDonHang { get; set; } // ID đơn hàng
        public string TrangThai { get; set; } // Trạng thái mới
    }
}