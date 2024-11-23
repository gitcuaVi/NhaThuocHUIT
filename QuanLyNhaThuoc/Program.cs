using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.Areas.KhachHang.Services.VnPay;
using QuanLyNhaThuoc.KhachHang.Services.VnPay;
using QuanLyNhaThuoc.Models;

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

            // Add authentication services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Home/AccessDenied";
                });

            // Configure MOMO and VNPAY services
            builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
            builder.Services.AddScoped<IMomoService, MomoService>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();

            // Add MVC and session services
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

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
            app.UseSession();

            // Authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Define routes
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the app
            app.Run();
        }
    }
}
