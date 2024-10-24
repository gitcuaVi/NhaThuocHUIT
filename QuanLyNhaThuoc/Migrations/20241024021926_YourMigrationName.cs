using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaThuoc.Migrations
{
    public partial class YourMigrationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    MaCaLam = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThoiGianBatDau = table.Column<TimeSpan>(type: "time", nullable: true),
                    ThoiGianKetThuc = table.Column<TimeSpan>(type: "time", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    GhiChuCongViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioNghiTrua = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CaLamVie__662C15A21D5C25D3", x => x.MaCaLam);
                });

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMuc", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "DonHangKhachHangViewModels",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDatHang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiaChiDonHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayGiaoHang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "FAQ",
                columns: table => new
                {
                    MaCauHoi = table.Column<int>(type: "int", nullable: false),
                    CauHoiThuongGap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CauTraLoiTuongUng = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucCauHoi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTaoCauHoi = table.Column<DateTime>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    NgayCapNhatCauHoi = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FAQ__1937D77BE483274F", x => x.MaCauHoi);
                });

            migrationBuilder.CreateTable(
                name: "QuyenTruyCap",
                columns: table => new
                {
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__QuyenTru__1D4B7ED44F6F0300", x => x.MaQuyen);
                });

            migrationBuilder.CreateTable(
                name: "UpdateDonHangs",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaiTro__C24C41CFB0D1B022", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "LoaiSanPham",
                columns: table => new
                {
                    MaLoaiSanPham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoaiSanP__ECCF699F0BD2FA30", x => x.MaLoaiSanPham);
                    table.ForeignKey(
                        name: "FK_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMuc",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNguoiDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NguoiDun__C539D7625E27EED9", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: true),
                    MaQuyen = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_PhanQuyen_Quyen",
                        column: x => x.MaQuyen,
                        principalTable: "QuyenTruyCap",
                        principalColumn: "MaQuyen");
                    table.ForeignKey(
                        name: "FK_PhanQuyen_VaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuoc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HanSuDung = table.Column<DateTime>(type: "date", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    MaLoaiSanPham = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Thuoc__4BB1F6201849222A", x => x.MaThuoc);
                    table.ForeignKey(
                        name: "FK_Thuoc_MaLoaiSanPham",
                        column: x => x.MaLoaiSanPham,
                        principalTable: "LoaiSanPham",
                        principalColumn: "MaLoaiSanPham");
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    Diem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__88D2F0E5FB3D1819", x => x.MaKhachHang);
                    table.ForeignKey(
                        name: "FK_KhachHang_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTuyenDung = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    MaCaLamViec = table.Column<int>(type: "int", nullable: false),
                    LuongCoBan1Ca = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    LuongTangCa1Gio = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhanVien__77B2CA472CF2237E", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanVien_MaCaLamViec",
                        column: x => x.MaCaLamViec,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCaLam");
                    table.ForeignKey(
                        name: "FK_NhanVien_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "GioHang",
                columns: table => new
                {
                    MaGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    DonVi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GioHang__F5001DA348C369DB", x => x.MaGioHang);
                    table.ForeignKey(
                        name: "FK_GioHang_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "HinhAnh",
                columns: table => new
                {
                    MaHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaThuoc = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HinhAnh__13EE1084F53BE805", x => x.MaHinh);
                    table.ForeignKey(
                        name: "FK_HinhAnh_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "TonKho",
                columns: table => new
                {
                    MaTonKho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    SoLuongCanhBao = table.Column<int>(type: "int", nullable: false),
                    SoLuongHienTai = table.Column<int>(type: "int", nullable: false),
                    SoLuongToiDa = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayGioCapNhat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    MaThuoc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TonKho__2D970FAA48FBE53C", x => x.MaTonKho);
                    table.ForeignKey(
                        name: "FK_TonKho_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "ChamCong",
                columns: table => new
                {
                    MaChamCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayChamCong = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    ThoiGianVaoLam = table.Column<DateTime>(type: "datetime", nullable: true),
                    ThoiGianRaVe = table.Column<DateTime>(type: "datetime", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValueSql: "(N'Không Đạt')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChamCong__307331A15F10C452", x => new { x.MaChamCong, x.MaNhanVien, x.NgayChamCong });
                    table.ForeignKey(
                        name: "FK_ChamCong_NhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TongTien = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDatHang = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    NgayCapNhat = table.Column<DateTime>(type: "date", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayGiaoHang = table.Column<DateTime>(type: "date", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    KhachHangMaKhachHang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHang__129584AD8F05CCBB", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang");
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang_KhachHangMaKhachHang",
                        column: x => x.KhachHangMaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonHang_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "Luong",
                columns: table => new
                {
                    MaLuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    LuongThang = table.Column<DateTime>(type: "date", nullable: false),
                    KhauTru = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    LuongThucNhan = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    NgayTraLuong = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoCaLamViec = table.Column<int>(type: "int", nullable: false),
                    LuongThuong = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Luong__6609A48DBF719780", x => new { x.MaLuong, x.MaNhanVien, x.LuongThang });
                    table.ForeignKey(
                        name: "FK_Luong_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhap",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    GhiChu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NhaCungCap = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuNha__1470EF3B36F3F0AA", x => x.MaPhieuNhap);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "PhieuXuat",
                columns: table => new
                {
                    MaPhieuXuat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayXuat = table.Column<DateTime>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NoiNhan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuXua__26C4B5A2917AC412", x => x.MaPhieuXuat);
                    table.ForeignKey(
                        name: "FK_PhieuXuat_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "SaoLuuVaPhucHoi",
                columns: table => new
                {
                    MaSaoLuu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    ThoiGianSaoLuu = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ThoiGianPhucHoi = table.Column<DateTime>(type: "datetime", nullable: true),
                    TrangThaiSaoLuu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenFileSaoLuu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SaoLuuVa__7ABE083C2FC08E21", x => x.MaSaoLuu);
                    table.ForeignKey(
                        name: "FK_SaoLuuVaPhucHoi_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    DonHangMaDonHang = table.Column<int>(type: "int", nullable: false),
                    ThuocMaThuoc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietD__CDF0A114F8C6B04F", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang_DonHangMaDonHang",
                        column: x => x.DonHangMaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_Thuoc_ThuocMaThuoc",
                        column: x => x.ThuocMaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    SoTien = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    MaQR = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ThanhToa__D4B25844E6E7518F", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToan_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPN",
                columns: table => new
                {
                    MaChiTietPN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaXuat = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    MaTonKho = table.Column<int>(type: "int", nullable: false),
                    MaPhieuNhap = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietP__651D0AF77C50A2D3", x => x.MaChiTietPN);
                    table.ForeignKey(
                        name: "FK_ChiTietPN_MaPhieuNhap",
                        column: x => x.MaPhieuNhap,
                        principalTable: "PhieuNhap",
                        principalColumn: "MaPhieuNhap");
                    table.ForeignKey(
                        name: "FK_ChiTietPN_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                    table.ForeignKey(
                        name: "FK_ChiTietPN_MaTonKho",
                        column: x => x.MaTonKho,
                        principalTable: "TonKho",
                        principalColumn: "MaTonKho");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPX",
                columns: table => new
                {
                    MaChiTietPX = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieuXuat = table.Column<int>(type: "int", nullable: false),
                    DonGiaXuat = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    MaTonKho = table.Column<int>(type: "int", nullable: false),
                    DonVi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaThuoc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietP__651D0AC186DEEEE9", x => x.MaChiTietPX);
                    table.ForeignKey(
                        name: "FK_ChiTietPX_MaPhieuXuat",
                        column: x => x.MaPhieuXuat,
                        principalTable: "PhieuXuat",
                        principalColumn: "MaPhieuXuat");
                    table.ForeignKey(
                        name: "FK_ChiTietPX_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                    table.ForeignKey(
                        name: "FK_ChiTietPX_MaTonKho",
                        column: x => x.MaTonKho,
                        principalTable: "TonKho",
                        principalColumn: "MaTonKho");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChamCong_MaNhanVien",
                table: "ChamCong",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_DonHangMaDonHang",
                table: "ChiTietDonHang",
                column: "DonHangMaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDonHang",
                table: "ChiTietDonHang",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaThuoc",
                table: "ChiTietDonHang",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_ThuocMaThuoc",
                table: "ChiTietDonHang",
                column: "ThuocMaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPN_MaPhieuNhap",
                table: "ChiTietPN",
                column: "MaPhieuNhap");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPN_MaTonKho",
                table: "ChiTietPN",
                column: "MaTonKho");

            migrationBuilder.CreateIndex(
                name: "UQ_ChiTietPN_MaThuoc_MaPhieuNhap",
                table: "ChiTietPN",
                columns: new[] { "MaThuoc", "MaPhieuNhap" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPX_MaPhieuXuat",
                table: "ChiTietPX",
                column: "MaPhieuXuat");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPX_MaTonKho",
                table: "ChiTietPX",
                column: "MaTonKho");

            migrationBuilder.CreateIndex(
                name: "UQ_ChiTietPX_MaThuoc_MaPhieuXuat",
                table: "ChiTietPX",
                columns: new[] { "MaThuoc", "MaPhieuXuat" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_KhachHangMaKhachHang",
                table: "DonHang",
                column: "KhachHangMaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKhachHang",
                table: "DonHang",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaNhanVien",
                table: "DonHang",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_MaThuoc",
                table: "GioHang",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnh_MaThuoc",
                table: "HinhAnh",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_MaNguoiDung",
                table: "KhachHang",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSanPham_MaDanhMuc",
                table: "LoaiSanPham",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_Luong_MaNhanVien",
                table: "Luong",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaCaLamViec",
                table: "NhanVien",
                column: "MaCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaNguoiDung",
                table: "NhanVien",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_MaQuyen",
                table: "PhanQuyen",
                column: "MaQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_MaVaiTro",
                table: "PhanQuyen",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_MaNhanVien",
                table: "PhieuNhap",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "UQ_PhieuNhap_MaPhieuNhap",
                table: "PhieuNhap",
                column: "MaPhieuNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuXuat_MaNhanVien",
                table: "PhieuXuat",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "UQ_PhieuXuat_MaPhieuXuat",
                table: "PhieuXuat",
                column: "MaPhieuXuat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaoLuuVaPhucHoi_MaNhanVien",
                table: "SaoLuuVaPhucHoi",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_MaDonHang",
                table: "ThanhToan",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_MaLoaiSanPham",
                table: "Thuoc",
                column: "MaLoaiSanPham");

            migrationBuilder.CreateIndex(
                name: "UQ_Thuoc_MaThuoc",
                table: "Thuoc",
                column: "MaThuoc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TonKho_MaThuoc",
                table: "TonKho",
                column: "MaThuoc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChamCong");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "ChiTietPN");

            migrationBuilder.DropTable(
                name: "ChiTietPX");

            migrationBuilder.DropTable(
                name: "DonHangKhachHangViewModels");

            migrationBuilder.DropTable(
                name: "FAQ");

            migrationBuilder.DropTable(
                name: "GioHang");

            migrationBuilder.DropTable(
                name: "HinhAnh");

            migrationBuilder.DropTable(
                name: "Luong");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.DropTable(
                name: "SaoLuuVaPhucHoi");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "UpdateDonHangs");

            migrationBuilder.DropTable(
                name: "PhieuNhap");

            migrationBuilder.DropTable(
                name: "PhieuXuat");

            migrationBuilder.DropTable(
                name: "TonKho");

            migrationBuilder.DropTable(
                name: "QuyenTruyCap");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "LoaiSanPham");

            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "DanhMuc");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
