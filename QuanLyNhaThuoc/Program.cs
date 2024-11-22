using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Models;

//using QuanLyNhaThuoc.Areas.Admin.Data;
//using QuanLyNhaThuoc.Services;

namespace QuanLyNhaThuoc
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure database context
            builder.Services.AddDbContext<QL_NhaThuocContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("QL_NhaThuoc")));

            // Add authentication services before building the app
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.LoginPath = "/Account/Login";
         options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

       /*  options.LoginPath = "/UserDH/Login";
         options.ExpireTimeSpan = TimeSpan.FromMinutes(30);*/

         options.SlidingExpiration = true;
         options.AccessDeniedPath = "/Home/AccessDenied";
     });
            /*// Đăng ký ApplicationDbContext với Entity Framework Core
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("QL_NhaThuoc")));
            // Đăng ký service quản lý đơn hàng
            builder.Services.AddScoped<DonHangService>();*/
            // Thêm dịch vụ MVC
            builder.Services.AddControllersWithViews();


            // Configure session services
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add other necessary services
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // Build the app
            var app = builder.Build();

            // Error handling and security features
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Middleware configuration
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable session middleware
            app.UseSession();

            // Authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();

            // Endpoint routing
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
