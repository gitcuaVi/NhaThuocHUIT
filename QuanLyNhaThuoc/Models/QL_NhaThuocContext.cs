using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QuanLyNhaThuoc.Areas.KhachHang.Models;
using QuanLyNhaThuoc.ViewModels;
namespace QuanLyNhaThuoc.Models
{
    public partial class QL_NhaThuocContext : DbContext
    {
       

        public QL_NhaThuocContext(DbContextOptions<QL_NhaThuocContext> options)
            : base(options)
        {
        }
       
             public DbSet<LichsudonhangView> LichSuDonHang { get; set; }
        public DbSet<ProductViewModel> ViewSanPhamByDanhMuc { get; set; }

        public DbSet<ProductViewDetailsModel> vw_ChiTietThuoc { get; set; }


        public DbSet<ProductViewDetailsModel> ViewSanPhamByid { get; set; }
        public virtual DbSet<GioHang> GioHangs { get; set; } = null!;
        public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; } = null!;
        public DbSet<ChartData> MonthlyOrders { get; set; }

        public virtual DbSet<CaLamViec> CaLamViecs { get; set; } = null!;
        public virtual DbSet<ChamCong> ChamCongs { get; set; } = null!;
        public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;
        public virtual DbSet<ChiTietPn> ChiTietPns { get; set; } = null!;
        public virtual DbSet<DanhMuc> DanhMucs { get; set; } = null!;
        public virtual DbSet<DonHang> DonHangs { get; set; } = null!;
        public virtual DbSet<Faq> Faqs { get; set; } = null!;
        public virtual DbSet<HinhAnh> HinhAnhs { get; set; } = null!;
        public virtual DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; } = null!;
        public virtual DbSet<Luong> Luongs { get; set; } = null!;
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; } = null!;
        public virtual DbSet<SaoLuuVaPhucHoi> SaoLuuVaPhucHois { get; set; } = null!;
        public virtual DbSet<ThanhToan> ThanhToans { get; set; } = null!;
        public virtual DbSet<Thuoc> Thuocs { get; set; } = null!;
        public virtual DbSet<TonKho> TonKhos { get; set; } = null!;
        public virtual DbSet<VaiTro> VaiTros { get; set; } = null!;
        public DbSet<ThongTinCaNhanView> ThongTinCaNhanView { get; set; }
        public DbSet<ThongTinKhachHangViewModel> ThongTinKhachHangViewModel { get; set; }

        public DbSet<DonHangKhachHangViewModels> DonHangKhachHangViewModels { get; set; }
        public DbSet<UpdateDonHang> UpdateDonHangs { get; set; }
        public DbSet<DonHangDetailsViewModel> DonHangDetailsViewModels { get; set; }
        public DbSet<ChiTietDonHangViewModel> ChiTietDonHangViewModels { get; set; }
        public DbSet<ThongKeDonHangView> vw_ThongKeDonHang { get; set; }
        public DbSet<ThongKePhieuNhapView> vw_ThongKePhieuNhap { get; set; }
        public DbSet<BangTamTKDH> BangTamTKDHs { get; set; }
        public DbSet<GioHangViewModel> GioHangViewModels { get; set; }
        public DbSet<PaymentModel> PaymentModels { get; set; }

        public DbSet<ThongTinKhachHangGioHang> ThongTinKhachHangGioHangs { get; set; }

        public DbSet<ThongTinDatHangViewModel> ThongTinDatHangViewModels { get; set; }

        public DbSet<DonHang> donHangs { get; set; }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=THANHSANG;Initial Catalog=QL_NhaThuoc;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentModel>().HasNoKey();

            modelBuilder.Entity<LichsudonhangView>().HasNoKey();

            modelBuilder.Entity<ProductViewModel>().HasNoKey();
            modelBuilder.Entity<ProductViewDetailsModel>().HasNoKey();

            modelBuilder.Entity<ProductViewModel>().HasNoKey();
            modelBuilder.Entity<ThongTinKhachHangViewModel>().HasNoKey();
            modelBuilder.Entity<GioHangViewModel>().HasNoKey();

            modelBuilder.Entity<ThongTinKhachHangGioHang>().HasNoKey();

            modelBuilder.Entity<ThongTinDatHangViewModel>().HasNoKey();




            modelBuilder.Entity<CaLamViec>(entity =>
            {
                entity.HasKey(e => e.MaCaLam)
                    .HasName("PK__CaLamVie__662C15A21D5C25D3");

                entity.ToTable("CaLamViec");

                entity.Property(e => e.ThoiGianTao)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<ThongTinCaNhanView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("v_ThongTinCaNhan");
            });
            modelBuilder.Entity<ChiTietDonHangViewModel>().HasNoKey();
            modelBuilder.Entity<DonHangDetailsViewModel>().HasNoKey();
            modelBuilder.Entity<ThongKeDonHangView>().HasNoKey();
            modelBuilder.Entity<BangTamTKDH>().HasNoKey();
            modelBuilder.Entity<ChartData>().HasNoKey();

            modelBuilder.Entity<ThongKePhieuNhapView>().HasNoKey();
            modelBuilder.Entity<ChamCong>(entity =>
            {
                entity.HasKey(e => new { e.MaChamCong, e.MaNhanVien, e.NgayChamCong })
                    .HasName("PK__ChamCong__307331A15F10C452");

                entity.ToTable("ChamCong");

                entity.Property(e => e.MaChamCong).ValueGeneratedOnAdd();

                entity.Property(e => e.NgayChamCong)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GhiChu)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(N'Không Đạt')");

                entity.Property(e => e.ThoiGianRaVe).HasColumnType("datetime");

                entity.Property(e => e.ThoiGianVaoLam).HasColumnType("datetime");

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.ChamCongs)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChamCong_NhanVien");
            });


          
            modelBuilder.Entity<DonHangKhachHangViewModels>().HasNoKey();
            modelBuilder.Entity<UpdateDonHang>().HasNoKey();

            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.ToTable("ChiTietDonHang"); // Đảm bảo ánh xạ đúng tên bảng

                // Thiết lập khóa chính
                entity.HasKey(e => e.MaChiTiet);

                // Quan hệ với bảng DonHang
                entity.HasOne(d => d.DonHang)
                    .WithMany(p => p.ChiTietDonHangs) // Sửa tên Navigation Property
                    .HasForeignKey(d => d.MaDonHang)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ với bảng Thuoc
                entity.HasOne(d => d.Thuoc)
                    .WithMany(p => p.ChiTietDonHangs) // Sửa tên Navigation Property
                    .HasForeignKey(d => d.MaThuoc)
                    .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<ChiTietPn>(entity =>
            {
                entity.HasKey(e => e.MaChiTietPn)
                    .HasName("PK__ChiTietP__651D0AF77C50A2D3");

                entity.ToTable("ChiTietPN");

                entity.HasIndex(e => new { e.MaThuoc, e.MaPhieuNhap }, "UQ_ChiTietPN_MaThuoc_MaPhieuNhap")
                    .IsUnique();

                entity.Property(e => e.MaChiTietPn).HasColumnName("MaChiTietPN");

                entity.Property(e => e.DonGiaXuat).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.MaPhieuNhapNavigation)
                    .WithMany(p => p.ChiTietPns)
                    .HasForeignKey(d => d.MaPhieuNhap)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietPN_MaPhieuNhap");

                entity.HasOne(d => d.MaThuocNavigation)
                    .WithMany(p => p.ChiTietPns)
                    .HasForeignKey(d => d.MaThuoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietPN_MaThuoc");

                entity.HasOne(d => d.MaTonKhoNavigation)
                    .WithMany(p => p.ChiTietPns)
                    .HasForeignKey(d => d.MaTonKho)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietPN_MaTonKho");
            });
            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.HasKey(e => e.MaDanhMuc);

                entity.ToTable("DanhMuc");

                entity.Property(e => e.TenDanhMuc).HasMaxLength(50);
            });

            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.MaDonHang)
                    .HasName("PK__DonHang__129584AD8F05CCBB");

                entity.ToTable("DonHang");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.NgayCapNhat).HasColumnType("date");

                entity.Property(e => e.NgayDatHang)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NgayGiaoHang).HasColumnType("date");

                entity.Property(e => e.TongTien).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaKhachHang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DonHang_KhachHang");

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DonHang_MaNhanVien");
            });
            /*modelBuilder.Entity<DonHang>()
                .HasMany(d => d.ChiTietDonHangs)
                .WithOne(c => c.MaDonHangNavigation) // Sử dụng MaDonHangNavigation thay vì DonHang
                .HasForeignKey(c => c.MaDonHang);*/
          /*  modelBuilder.Entity<DonHang>()
        .HasOne(d => d.KhachHang)
        .WithMany()
        .HasForeignKey(d => d.MaKhachHang);*/
            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<Faq>(entity =>
            {
                entity.HasKey(e => e.MaCauHoi)
                    .HasName("PK__FAQ__1937D77BE483274F");

                entity.ToTable("FAQ");

                entity.Property(e => e.MaCauHoi).ValueGeneratedNever();

                entity.Property(e => e.DanhMucCauHoi).HasMaxLength(255);

                entity.Property(e => e.NgayCapNhatCauHoi).HasColumnType("date");

                entity.Property(e => e.NgayTaoCauHoi)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasKey(e => e.MaGioHang)
                    .HasName("PK__GioHang__F5001DA348C369DB");

                entity.ToTable("GioHang");

                entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

                // Adding foreign key relationship for MaKhachHang
                entity.HasOne(d => d.MaKhachHangNavigation)
                    .WithMany(p => p.GioHangs)
                    .HasForeignKey(d => d.MaKhachHang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GioHang_KhachHang");
            });
            modelBuilder.Entity<ChiTietGioHang>(entity =>
            {
                entity.HasKey(e => e.MaChiTietGioHang)
                    .HasName("PK_ChiTietGioHang");

                entity.ToTable("ChiTietGioHang");

                entity.Property(e => e.SoLuong)
                    .IsRequired();

                entity.HasOne(e => e.GioHangNavigation)
                    .WithMany(gh => gh.ChiTietGioHangs)
                    .HasForeignKey(e => e.MaGioHang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietGioHang_GioHang");

                entity.HasOne(e => e.Thuoc)
                    .WithMany(t => t.ChiTietGioHangs)
                    .HasForeignKey(e => e.MaThuoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietGioHang_Thuoc");
            });

            modelBuilder.Entity<HinhAnh>(entity =>
            {
                entity.HasKey(e => e.MaHinh)
                    .HasName("PK__HinhAnh__13EE1084F53BE805");

                entity.ToTable("HinhAnh");

                entity.HasOne(d => d.MaThuocNavigation)
                    .WithMany(p => p.HinhAnhs)
                    .HasForeignKey(d => d.MaThuoc)
                    .HasConstraintName("FK_HinhAnh_MaThuoc");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKhachHang)
                    .HasName("PK__KhachHan__88D2F0E5FB3D1819");

                entity.ToTable("KhachHang");

                entity.Property(e => e.MaKhachHang).ValueGeneratedNever();

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.GioiTinh).HasMaxLength(10);

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.TenKhachHang).HasMaxLength(255);

                entity.HasOne(d => d.MaNguoiDungNavigation)
                    .WithMany(p => p.KhachHangs)
                    .HasForeignKey(d => d.MaNguoiDung)
                    .HasConstraintName("FK_KhachHang_MaNguoiDung");
            });

            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.MaLoaiSanPham)
                    .HasName("PK__LoaiSanP__ECCF699F0BD2FA30");

                entity.ToTable("LoaiSanPham");

                entity.Property(e => e.TenLoai).HasMaxLength(255);

                entity.HasOne(d => d.MaDanhMucNavigation)
                    .WithMany(p => p.LoaiSanPhams)
                    .HasForeignKey(d => d.MaDanhMuc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaDanhMuc");
            });

            modelBuilder.Entity<Luong>(entity =>
            {
                entity.HasKey(e => new { e.MaLuong, e.MaNhanVien, e.LuongThang })
                    .HasName("PK__Luong__6609A48DBF719780");

                entity.ToTable("Luong");

                entity.Property(e => e.MaLuong).ValueGeneratedOnAdd();

                entity.Property(e => e.LuongThang).HasColumnType("date");

                entity.Property(e => e.GhiChu).HasMaxLength(255);

                entity.Property(e => e.KhauTru).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.LuongThucNhan).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.LuongThuong).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.NgayTraLuong)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.Luongs)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Luong_MaNhanVien");
            });

            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasKey(e => e.MaNguoiDung)
                    .HasName("PK__NguoiDun__C539D7625E27EED9");

                entity.ToTable("NguoiDung");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.NgayTao)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.TenNguoiDung).HasMaxLength(255);

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaVaiTroNavigation)
                    .WithMany(p => p.NguoiDungs)
                    .HasForeignKey(d => d.MaVaiTro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NguoiDung_MaVaiTro");
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNhanVien)
                    .HasName("PK__NhanVien__77B2CA472CF2237E");

                entity.ToTable("NhanVien");

                entity.Property(e => e.ChucVu).HasMaxLength(50);

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.GioiTinh).HasMaxLength(10);

                entity.Property(e => e.Ho).HasMaxLength(50);

                entity.Property(e => e.LuongCoBan1Ca).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.LuongTangCa1Gio).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.NgayTuyenDung)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ten).HasMaxLength(50);

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaCaLamViecNavigation)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.MaCaLamViec)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NhanVien_MaCaLamViec");

                entity.HasOne(d => d.MaNguoiDungNavigation)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.MaNguoiDung)
                    .HasConstraintName("FK_NhanVien_MaNguoiDung");
            });

            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.HasKey(e => e.MaPhieuNhap)
                    .HasName("PK__PhieuNha__1470EF3B36F3F0AA");

                entity.ToTable("PhieuNhap");

                entity.HasIndex(e => e.MaPhieuNhap, "UQ_PhieuNhap_MaPhieuNhap")
                    .IsUnique();

                entity.Property(e => e.GhiChu).HasMaxLength(10);

                entity.Property(e => e.NgayNhap)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NhaCungCap).HasMaxLength(255);

                entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.PhieuNhaps)
                    .HasForeignKey(d => d.MaNhanVien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhieuNhap_MaNhanVien");
            });
            modelBuilder.Entity<SaoLuuVaPhucHoi>(entity =>
            {
                entity.HasKey(e => e.MaSaoLuu)
                    .HasName("PK__SaoLuuVa__7ABE083C2FC08E21");

                entity.ToTable("SaoLuuVaPhucHoi");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.TenFileSaoLuu).HasMaxLength(255);

                entity.Property(e => e.ThoiGianPhucHoi).HasColumnType("datetime");

                entity.Property(e => e.ThoiGianSaoLuu)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TrangThaiSaoLuu).HasMaxLength(50);

                entity.HasOne(d => d.MaNhanVienNavigation)
                    .WithMany(p => p.SaoLuuVaPhucHois)
                    .HasForeignKey(d => d.MaNhanVien)
                    .HasConstraintName("FK_SaoLuuVaPhucHoi_MaNhanVien");
            });

            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.HasKey(e => e.MaThanhToan)
                    .HasName("PK__ThanhToa__D4B25844E6E7518F");

                entity.ToTable("ThanhToan");

                entity.Property(e => e.GhiChu).HasMaxLength(255);

                entity.Property(e => e.MaQr)
                    .HasMaxLength(255)
                    .HasColumnName("MaQR");

                entity.Property(e => e.NgayThanhToan)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);

                entity.Property(e => e.SoTien).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.TrangThaiThanhToan).HasMaxLength(50);

                entity.HasOne(d => d.MaDonHangNavigation)
                    .WithMany(p => p.ThanhToans)
                    .HasForeignKey(d => d.MaDonHang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ThanhToan_MaDonHang");
            });

            modelBuilder.Entity<Thuoc>(entity =>
            {
                entity.HasKey(e => e.MaThuoc)
                    .HasName("PK__Thuoc__4BB1F6201849222A");

                entity.ToTable("Thuoc");

                entity.HasIndex(e => e.MaThuoc, "UQ_Thuoc_MaThuoc")
                    .IsUnique();

                entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.HanSuDung).HasColumnType("date");

                entity.Property(e => e.SoLuongTon).HasDefaultValueSql("((0))");

                entity.Property(e => e.TenThuoc).HasMaxLength(255);
                entity.Property(e => e.DonVi).HasMaxLength(255);

                entity.Property(e => e.DonVi).HasMaxLength(255);

                entity.HasOne(d => d.MaLoaiSanPhamNavigation)
                    .WithMany(p => p.Thuocs)
                    .HasForeignKey(d => d.MaLoaiSanPham)
                    .HasConstraintName("FK_Thuoc_MaLoaiSanPham");
            });

            modelBuilder.Entity<TonKho>(entity =>
            {
                entity.HasKey(e => e.MaTonKho)
                    .HasName("PK__TonKho__2D970FAA48FBE53C");

                entity.ToTable("TonKho");

                entity.Property(e => e.NgayGioCapNhat)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaThuocNavigation)
                    .WithMany(p => p.TonKhos)
                    .HasForeignKey(d => d.MaThuoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TonKho_MaThuoc");
            });

            modelBuilder.Entity<VaiTro>(entity =>
            {
                entity.HasKey(e => e.MaVaiTro)
                    .HasName("PK__VaiTro__C24C41CFB0D1B022");

                entity.ToTable("VaiTro");

                entity.Property(e => e.TenVaiTro).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
