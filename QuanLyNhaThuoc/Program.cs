using QuanLyNhaThuoc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace QuanLyNhaThuoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<QL_NhaThuocContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("QL_NhaThuoc")));

            // Thêm dịch vụ MVC
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // Cấu hình dịch vụ session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Admin/Admin/Login"; // Page to redirect if not logged in
                    options.AccessDeniedPath = "/Admin/Account/AccessDenied"; // Page to show if access is denied
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

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

            // Kích hoạt session và authentication middleware
            app.UseSession();
            app.UseAuthentication();
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
