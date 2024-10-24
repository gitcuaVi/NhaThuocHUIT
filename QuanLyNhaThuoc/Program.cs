﻿using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;

//using QuanLyNhaThuoc.Areas.Admin.Data;
//using QuanLyNhaThuoc.Services;

namespace QuanLyNhaThuoc
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Thêm DbContext vào DI container với chuỗi kết nối từ appsettings.json
            builder.Services.AddDbContext<QL_NhaThuocContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("QL_NhaThuoc")));
            /*// Đăng ký ApplicationDbContext với Entity Framework Core
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("QL_NhaThuoc")));
            // Đăng ký service quản lý đơn hàng
            builder.Services.AddScoped<DonHangService>();*/
            // Thêm dịch vụ MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Cấu hình HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Cấu hình route cho khu vực (Area)
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    

    }

}
