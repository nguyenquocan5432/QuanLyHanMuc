SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

USE master;
GO

IF DB_ID(N'QuanLyHanMuc') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyHanMuc SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyHanMuc;
END
GO

CREATE DATABASE QuanLyHanMuc COLLATE Vietnamese_100_CI_AS;
GO

USE QuanLyHanMuc;
GO

CREATE TABLE dbo.NguoiDung (
    MaNguoiDung        int IDENTITY(1,1) NOT NULL,
    TenDangNhap        varchar(50)   NOT NULL,
    MatKhauMaHoa       varchar(200)  NOT NULL,
    HoTen              nvarchar(100) NOT NULL,
    Email              varchar(100)  NULL,
    VaiTro             nvarchar(20)  NOT NULL,
    MaNguoiCapNganSach int           NULL,
    HoatDong           bit           NOT NULL CONSTRAINT DF_NguoiDung_HoatDong DEFAULT (1),
    NgayTao            datetime2(0)  NOT NULL CONSTRAINT DF_NguoiDung_NgayTao  DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_NguoiDung PRIMARY KEY (MaNguoiDung),
    CONSTRAINT UQ_NguoiDung_TenDangNhap UNIQUE (TenDangNhap),
    CONSTRAINT CK_NguoiDung_VaiTro
        CHECK (VaiTro IN (N'QuanTri', N'CapNganSach', N'SinhVien')),
    CONSTRAINT FK_NguoiDung_NguoiCap
        FOREIGN KEY (MaNguoiCapNganSach) REFERENCES dbo.NguoiDung (MaNguoiDung),
    CONSTRAINT CK_NguoiDung_KhongTuCap
        CHECK (MaNguoiCapNganSach <> MaNguoiDung),
    CONSTRAINT CK_NguoiDung_QuanTriKhongCoNguoiCap
        CHECK (VaiTro <> N'QuanTri' OR MaNguoiCapNganSach IS NULL)
);
GO

CREATE TABLE dbo.DanhMuc (
    MaDanhMuc  int IDENTITY(1,1) NOT NULL,
    TenDanhMuc nvarchar(100) NOT NULL,
    MoTa       nvarchar(255) NULL,
    HoatDong   bit NOT NULL CONSTRAINT DF_DanhMuc_HoatDong DEFAULT (1),

    CONSTRAINT PK_DanhMuc PRIMARY KEY (MaDanhMuc),
    CONSTRAINT UQ_DanhMuc_TenDanhMuc UNIQUE (TenDanhMuc)
);
GO

CREATE TABLE dbo.HanMuc (
    MaHanMuc      int IDENTITY(1,1) NOT NULL,
    MaSinhVien    int           NOT NULL,
    MaDanhMuc     int           NOT NULL,
    NgayBatDau    date          NOT NULL,
    NgayKetThuc   date          NOT NULL,
    SoTienHanMuc  decimal(18,0) NOT NULL,
    SoTienDaChi   decimal(18,0) NOT NULL CONSTRAINT DF_HanMuc_SoTienDaChi   DEFAULT (0),
    NguongCanhBao decimal(5,2)  NOT NULL CONSTRAINT DF_HanMuc_NguongCanhBao DEFAULT (80),
    LoaiHanMuc    nvarchar(10)  NOT NULL,
    TrangThai     nvarchar(20)  NOT NULL CONSTRAINT DF_HanMuc_TrangThai     DEFAULT (N'Hoạt động'),
    MaNguoiTao    int           NOT NULL,
    NgayTao       datetime2(0)  NOT NULL CONSTRAINT DF_HanMuc_NgayTao       DEFAULT (SYSDATETIME()),
    PhienBan      rowversion    NOT NULL,

    CONSTRAINT PK_HanMuc PRIMARY KEY (MaHanMuc),
    CONSTRAINT FK_HanMuc_SinhVien FOREIGN KEY (MaSinhVien) REFERENCES dbo.NguoiDung (MaNguoiDung),
    CONSTRAINT FK_HanMuc_DanhMuc  FOREIGN KEY (MaDanhMuc)  REFERENCES dbo.DanhMuc (MaDanhMuc),
    CONSTRAINT FK_HanMuc_NguoiTao FOREIGN KEY (MaNguoiTao) REFERENCES dbo.NguoiDung (MaNguoiDung),
    CONSTRAINT CK_HanMuc_Ky        CHECK (NgayKetThuc > NgayBatDau),
    CONSTRAINT CK_HanMuc_SoTien    CHECK (SoTienHanMuc > 0),
    CONSTRAINT CK_HanMuc_DaChi     CHECK (SoTienDaChi >= 0),
    CONSTRAINT CK_HanMuc_Nguong    CHECK (NguongCanhBao BETWEEN 1 AND 100),
    CONSTRAINT CK_HanMuc_Loai      CHECK (LoaiHanMuc IN (N'Cứng', N'Mềm')),
    CONSTRAINT CK_HanMuc_TrangThai
        CHECK (TrangThai IN (N'Hoạt động', N'Sắp vượt', N'Đã vượt', N'Đã khóa')),
    CONSTRAINT CK_HanMuc_CungKhongVuot
        CHECK (LoaiHanMuc <> N'Cứng' OR SoTienDaChi <= SoTienHanMuc)
);
GO

CREATE TABLE dbo.GiaoDich (
    MaGiaoDich   int IDENTITY(1,1) NOT NULL,
    MaHanMuc     int           NOT NULL,
    SoTien       decimal(18,0) NOT NULL,
    NgayGiaoDich date          NOT NULL,
    MoTa         nvarchar(255) NULL,
    TrangThai    nvarchar(20)  NOT NULL,
    NgayTao      datetime2(0)  NOT NULL CONSTRAINT DF_GiaoDich_NgayTao DEFAULT (SYSDATETIME()),
    LyDoHuy      nvarchar(255) NULL,
    MaNguoiHuy   int           NULL,
    NgayHuy      datetime2(0)  NULL,

    CONSTRAINT PK_GiaoDich PRIMARY KEY (MaGiaoDich),
    CONSTRAINT FK_GiaoDich_HanMuc   FOREIGN KEY (MaHanMuc)   REFERENCES dbo.HanMuc (MaHanMuc),
    CONSTRAINT FK_GiaoDich_NguoiHuy FOREIGN KEY (MaNguoiHuy) REFERENCES dbo.NguoiDung (MaNguoiDung),
    CONSTRAINT CK_GiaoDich_SoTien CHECK (SoTien > 0),
    CONSTRAINT CK_GiaoDich_TrangThai
        CHECK (TrangThai IN (N'Hợp lệ', N'Chờ duyệt', N'Từ chối', N'Đã hủy')),
    CONSTRAINT CK_GiaoDich_HuyPhaiCoLyDo
        CHECK (TrangThai <> N'Đã hủy'
               OR (LEN(LTRIM(RTRIM(LyDoHuy))) > 0
                   AND MaNguoiHuy IS NOT NULL
                   AND NgayHuy IS NOT NULL))
);
GO

CREATE TABLE dbo.YeuCauNgoaiLe (
    MaYeuCau     int IDENTITY(1,1) NOT NULL,
    MaGiaoDich   int           NOT NULL,
    MaNguoiDuyet int           NULL,
    TrangThai    nvarchar(20)  NOT NULL CONSTRAINT DF_YeuCauNgoaiLe_TrangThai  DEFAULT (N'Chờ duyệt'),
    GhiChu       nvarchar(255) NULL,
    NgayYeuCau   datetime2(0)  NOT NULL CONSTRAINT DF_YeuCauNgoaiLe_NgayYeuCau DEFAULT (SYSDATETIME()),
    NgayDuyet    datetime2(0)  NULL,

    CONSTRAINT PK_YeuCauNgoaiLe PRIMARY KEY (MaYeuCau),
    CONSTRAINT FK_YeuCauNgoaiLe_GiaoDich   FOREIGN KEY (MaGiaoDich)   REFERENCES dbo.GiaoDich (MaGiaoDich),
    CONSTRAINT FK_YeuCauNgoaiLe_NguoiDuyet FOREIGN KEY (MaNguoiDuyet) REFERENCES dbo.NguoiDung (MaNguoiDung),
    CONSTRAINT UQ_YeuCauNgoaiLe_MaGiaoDich UNIQUE (MaGiaoDich),
    CONSTRAINT CK_YeuCauNgoaiLe_TrangThai
        CHECK (TrangThai IN (N'Chờ duyệt', N'Đã duyệt', N'Từ chối')),
    CONSTRAINT CK_YeuCauNgoaiLe_NguoiDuyet
        CHECK ((TrangThai = N'Chờ duyệt'  AND MaNguoiDuyet IS NULL     AND NgayDuyet IS NULL)
            OR (TrangThai <> N'Chờ duyệt' AND MaNguoiDuyet IS NOT NULL AND NgayDuyet IS NOT NULL))
);
GO

CREATE TABLE dbo.NhatKyViPham (
    MaViPham      int IDENTITY(1,1) NOT NULL,
    MaHanMuc      int           NOT NULL,
    SoTienDinhChi decimal(18,0) NOT NULL,
    SoConLaiLucDo decimal(18,0) NOT NULL,
    KetQua        nvarchar(30)  NOT NULL,
    NgayXayRa     datetime2(0)  NOT NULL CONSTRAINT DF_NhatKyViPham_NgayXayRa DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_NhatKyViPham PRIMARY KEY (MaViPham),
    CONSTRAINT FK_NhatKyViPham_HanMuc FOREIGN KEY (MaHanMuc) REFERENCES dbo.HanMuc (MaHanMuc),
    CONSTRAINT CK_NhatKyViPham_SoTien CHECK (SoTienDinhChi > 0),
    CONSTRAINT CK_NhatKyViPham_KetQua CHECK (KetQua IN (N'Từ chối', N'Gửi chờ duyệt'))
);
GO

CREATE TABLE dbo.ThongBao (
    MaThongBao   int IDENTITY(1,1) NOT NULL,
    MaNguoiNhan  int           NOT NULL,
    MaHanMuc     int           NULL,
    LoaiThongBao nvarchar(30)  NOT NULL,
    NoiDung      nvarchar(500) NOT NULL,
    TinhTrangGui nvarchar(20)  NOT NULL CONSTRAINT DF_ThongBao_TinhTrangGui DEFAULT (N'Trong ứng dụng'),
    DaDoc        bit           NOT NULL CONSTRAINT DF_ThongBao_DaDoc        DEFAULT (0),
    NgayTao      datetime2(0)  NOT NULL CONSTRAINT DF_ThongBao_NgayTao      DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_ThongBao PRIMARY KEY (MaThongBao),
    CONSTRAINT FK_ThongBao_NguoiNhan FOREIGN KEY (MaNguoiNhan) REFERENCES dbo.NguoiDung (MaNguoiDung),
    CONSTRAINT FK_ThongBao_HanMuc    FOREIGN KEY (MaHanMuc)    REFERENCES dbo.HanMuc (MaHanMuc),
    CONSTRAINT CK_ThongBao_Loai
        CHECK (LoaiThongBao IN (N'Ngưỡng', N'Đã vượt', N'Yêu cầu duyệt', N'Kết quả duyệt')),
    CONSTRAINT CK_ThongBao_TinhTrangGui
        CHECK (TinhTrangGui IN (N'Đã gửi', N'Lỗi', N'Trong ứng dụng'))
);
GO

CREATE TABLE dbo.NhatKyHeThong (
    MaNhatKy    int IDENTITY(1,1) NOT NULL,
    MaNguoiDung int           NOT NULL,
    HanhDong    nvarchar(50)  NOT NULL,
    TenThucThe  varchar(50)   NOT NULL,
    MaThucThe   int           NOT NULL,
    GiaTriCu    nvarchar(max) NULL,
    GiaTriMoi   nvarchar(max) NULL,
    NgayGio     datetime2(0)  NOT NULL CONSTRAINT DF_NhatKyHeThong_NgayGio DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_NhatKyHeThong PRIMARY KEY (MaNhatKy),
    CONSTRAINT FK_NhatKyHeThong_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES dbo.NguoiDung (MaNguoiDung)
);
GO

CREATE INDEX IX_HanMuc_TimHieuLuc
    ON dbo.HanMuc (MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc)
    INCLUDE (TrangThai, SoTienHanMuc, SoTienDaChi);

CREATE INDEX IX_GiaoDich_LichSu
    ON dbo.GiaoDich (MaHanMuc, NgayGiaoDich)
    INCLUDE (SoTien, TrangThai);

CREATE INDEX IX_YeuCauNgoaiLe_ChoDuyet
    ON dbo.YeuCauNgoaiLe (NgayYeuCau)
    INCLUDE (MaGiaoDich)
    WHERE TrangThai = N'Chờ duyệt';

CREATE INDEX IX_ThongBao_ChuaDoc
    ON dbo.ThongBao (MaNguoiNhan, DaDoc);

CREATE INDEX IX_NhatKyViPham_BaoCao
    ON dbo.NhatKyViPham (MaHanMuc, NgayXayRa);
GO

CREATE OR ALTER FUNCTION dbo.fn_TinhTrangThai
(
    @SoTienDaChi   decimal(18,0),
    @SoTienHanMuc  decimal(18,0),
    @NguongCanhBao decimal(5,2)
)
RETURNS nvarchar(20)
AS
BEGIN
    RETURN CASE
        WHEN @SoTienDaChi >= @SoTienHanMuc                        THEN N'Đã vượt'
        WHEN @SoTienDaChi * 100 >= @SoTienHanMuc * @NguongCanhBao THEN N'Sắp vượt'
        ELSE N'Hoạt động'
    END;
END
GO

CREATE OR ALTER FUNCTION dbo.fn_CoQuyenQuanLy
(
    @MaNguoi    int,
    @MaSinhVien int
)
RETURNS bit
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM dbo.NguoiDung sv
        JOIN dbo.NguoiDung n ON n.MaNguoiDung = @MaNguoi AND n.HoatDong = 1
        WHERE sv.MaNguoiDung = @MaSinhVien
          AND (   (n.VaiTro = N'CapNganSach'
                   AND (sv.MaNguoiCapNganSach = n.MaNguoiDung OR sv.MaNguoiDung = n.MaNguoiDung))
               OR (sv.MaNguoiCapNganSach IS NULL AND sv.MaNguoiDung = n.MaNguoiDung)))
        RETURN 1;
    RETURN 0;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_HanMuc_ChongTrungKy
ON dbo.HanMuc
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT (UPDATE(MaSinhVien) OR UPDATE(MaDanhMuc) OR UPDATE(NgayBatDau)
            OR UPDATE(NgayKetThuc) OR UPDATE(TrangThai))
        RETURN;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.HanMuc h
          ON  h.MaSinhVien = i.MaSinhVien
          AND h.MaDanhMuc  = i.MaDanhMuc
          AND h.MaHanMuc  <> i.MaHanMuc
        WHERE i.TrangThai <> N'Đã khóa'
          AND h.TrangThai <> N'Đã khóa'
          AND i.NgayBatDau <= h.NgayKetThuc
          AND h.NgayBatDau <= i.NgayKetThuc)
        THROW 50001, N'Đã có hạn mức trùng kỳ cho sinh viên và danh mục này (BR01).', 1;
END
GO

CREATE OR ALTER TRIGGER dbo.TR_GiaoDich_BaoVe
ON dbo.GiaoDich
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM inserted)
        RETURN;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.HanMuc h ON h.MaHanMuc = i.MaHanMuc
        WHERE i.NgayGiaoDich < h.NgayBatDau
           OR i.NgayGiaoDich > h.NgayKetThuc)
        THROW 50002, N'Ngày giao dịch nằm ngoài kỳ của hạn mức.', 1;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.HanMuc h ON h.MaHanMuc = i.MaHanMuc
        LEFT JOIN deleted d ON d.MaGiaoDich = i.MaGiaoDich
        WHERE i.TrangThai = N'Hợp lệ'
          AND (d.MaGiaoDich IS NULL OR d.TrangThai <> N'Hợp lệ')
          AND h.TrangThai = N'Đã khóa')
        THROW 50003, N'Hạn mức đang bị khóa, không nhận giao dịch mới (BR06).', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.HanMuc h WITH (UPDLOCK, HOLDLOCK)
        WHERE h.LoaiHanMuc = N'Cứng'
          AND h.MaHanMuc IN (SELECT MaHanMuc FROM inserted)
          AND h.SoTienHanMuc < (SELECT SUM(g.SoTien)
                                FROM dbo.GiaoDich g
                                WHERE g.MaHanMuc = h.MaHanMuc
                                  AND g.TrangThai = N'Hợp lệ'))
        THROW 50004, N'Tổng giao dịch hợp lệ vượt hạn mức cứng (BR04).', 1;
END
GO

CREATE OR ALTER VIEW dbo.vw_TinhTrangHanMuc
AS
SELECT  h.MaHanMuc,
        h.MaSinhVien,
        sv.HoTen                                       AS SinhVien,
        h.MaDanhMuc,
        dm.TenDanhMuc,
        h.NgayBatDau,
        h.NgayKetThuc,
        h.LoaiHanMuc,
        h.SoTienHanMuc,
        h.SoTienDaChi,
        h.SoTienHanMuc - h.SoTienDaChi                 AS SoConLai,
        CAST(h.SoTienDaChi * 100.0 / h.SoTienHanMuc AS decimal(9,2)) AS TyLeSuDung,
        h.NguongCanhBao,
        h.TrangThai
FROM dbo.HanMuc h
JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien
JOIN dbo.DanhMuc  dm ON dm.MaDanhMuc   = h.MaDanhMuc;
GO

CREATE OR ALTER VIEW dbo.vw_DoiSoatHanMuc
AS
SELECT  h.MaHanMuc,
        h.SoTienDaChi,
        ISNULL(g.TongHopLe, 0)                 AS TongGiaoDichHopLe,
        h.SoTienDaChi - ISNULL(g.TongHopLe, 0) AS ChenhLech
FROM dbo.HanMuc h
LEFT JOIN (SELECT MaHanMuc, SUM(SoTien) AS TongHopLe
           FROM dbo.GiaoDich
           WHERE TrangThai = N'Hợp lệ'
           GROUP BY MaHanMuc) g
       ON g.MaHanMuc = h.MaHanMuc;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TaoHanMuc
    @MaNguoiTao    int,
    @MaSinhVien    int,
    @MaDanhMuc     int,
    @NgayBatDau    date,
    @NgayKetThuc   date,
    @SoTienHanMuc  decimal(18,0),
    @LoaiHanMuc    nvarchar(10),
    @NguongCanhBao decimal(5,2) = 80
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MaHanMuc int, @Json nvarchar(max);

    IF dbo.fn_CoQuyenQuanLy(@MaNguoiTao, @MaSinhVien) = 0
        THROW 50031, N'Bạn không có quyền thiết lập hạn mức cho sinh viên này (BR08).', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.DanhMuc WHERE MaDanhMuc = @MaDanhMuc AND HoatDong = 1)
        THROW 50032, N'Danh mục không tồn tại hoặc đã ngừng dùng.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.HanMuc (MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc,
                                SoTienHanMuc, LoaiHanMuc, NguongCanhBao, MaNguoiTao)
        VALUES (@MaSinhVien, @MaDanhMuc, @NgayBatDau, @NgayKetThuc,
                @SoTienHanMuc, @LoaiHanMuc, @NguongCanhBao, @MaNguoiTao);

        SET @MaHanMuc = SCOPE_IDENTITY();

        SET @Json = (SELECT MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc,
                            SoTienHanMuc, LoaiHanMuc, NguongCanhBao
                     FROM dbo.HanMuc WHERE MaHanMuc = @MaHanMuc
                     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriMoi)
        VALUES (@MaNguoiTao, N'Tạo hạn mức', 'HanMuc', @MaHanMuc, @Json);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaHanMuc AS MaHanMuc, N'Đã tạo hạn mức.' AS ThongDiep;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_KhoaMoKhoaHanMuc
    @MaHanMuc int,
    @MaNguoi  int,
    @Khoa     bit
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MaSinhVien int, @TrangThaiCu nvarchar(20), @TrangThaiMoi nvarchar(20),
            @SoTienHanMuc decimal(18,0), @SoTienDaChi decimal(18,0), @Nguong decimal(5,2);

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @MaSinhVien   = MaSinhVien,
               @TrangThaiCu  = TrangThai,
               @SoTienHanMuc = SoTienHanMuc,
               @SoTienDaChi  = SoTienDaChi,
               @Nguong       = NguongCanhBao
        FROM dbo.HanMuc WITH (UPDLOCK, HOLDLOCK)
        WHERE MaHanMuc = @MaHanMuc;

        IF @MaSinhVien IS NULL
            THROW 50033, N'Không tìm thấy hạn mức.', 1;

        IF dbo.fn_CoQuyenQuanLy(@MaNguoi, @MaSinhVien) = 0
            THROW 50034, N'Bạn không có quyền khóa hoặc mở khóa hạn mức này (BR08).', 1;

        SET @TrangThaiMoi = CASE WHEN @Khoa = 1 THEN N'Đã khóa'
                                 ELSE dbo.fn_TinhTrangThai(@SoTienDaChi, @SoTienHanMuc, @Nguong) END;

        UPDATE dbo.HanMuc SET TrangThai = @TrangThaiMoi WHERE MaHanMuc = @MaHanMuc;

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriCu, GiaTriMoi)
        VALUES (@MaNguoi,
                CASE WHEN @Khoa = 1 THEN N'Khóa hạn mức' ELSE N'Mở khóa hạn mức' END,
                'HanMuc', @MaHanMuc,
                CONCAT(N'{"TrangThai":"', @TrangThaiCu, N'"}'),
                CONCAT(N'{"TrangThai":"', @TrangThaiMoi, N'"}'));

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaHanMuc AS MaHanMuc, @TrangThaiMoi AS TrangThai;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GhiNhanGiaoDich
    @MaSinhVien   int,
    @MaDanhMuc    int,
    @SoTien       decimal(18,0),
    @NgayGiaoDich date          = NULL,
    @MoTa         nvarchar(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @KetQua nvarchar(30), @ThongDiep nvarchar(255), @MaGiaoDich int,
            @MaHanMuc int, @SoTienHanMuc decimal(18,0), @SoTienDaChi decimal(18,0),
            @Nguong decimal(5,2), @Loai nvarchar(10),
            @TrangThaiCu nvarchar(20), @TrangThaiMoi nvarchar(20),
            @TongDuKien decimal(18,0), @TyLe decimal(9,2), @SoConLai decimal(18,0),
            @MaNguoiCap int, @TenDanhMuc nvarchar(100), @Json nvarchar(max);

    SET @NgayGiaoDich = ISNULL(@NgayGiaoDich, CAST(SYSDATETIME() AS date));

    IF @SoTien IS NULL OR @SoTien <= 0
    BEGIN
        SET @KetQua = N'Từ chối';
        SET @ThongDiep = N'Số tiền phải lớn hơn 0 (BR07).';
        GOTO KetThuc;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE MaNguoiDung = @MaSinhVien AND HoatDong = 1)
    BEGIN
        SET @KetQua = N'Từ chối';
        SET @ThongDiep = N'Tài khoản không tồn tại hoặc đã ngừng hoạt động.';
        GOTO KetThuc;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT TOP (1)
               @MaHanMuc     = h.MaHanMuc,
               @SoTienHanMuc = h.SoTienHanMuc,
               @SoTienDaChi  = h.SoTienDaChi,
               @Nguong       = h.NguongCanhBao,
               @Loai         = h.LoaiHanMuc,
               @TrangThaiCu  = h.TrangThai
        FROM dbo.HanMuc h WITH (UPDLOCK, HOLDLOCK)
        WHERE h.MaSinhVien = @MaSinhVien
          AND h.MaDanhMuc  = @MaDanhMuc
          AND @NgayGiaoDich BETWEEN h.NgayBatDau AND h.NgayKetThuc
        ORDER BY CASE WHEN h.TrangThai = N'Đã khóa' THEN 1 ELSE 0 END;

        SELECT @TenDanhMuc = TenDanhMuc FROM dbo.DanhMuc WHERE MaDanhMuc = @MaDanhMuc;

        IF @MaHanMuc IS NULL
        BEGIN
            SET @KetQua = N'Từ chối';
            SET @ThongDiep = N'Chưa có hạn mức cho danh mục này vào ngày giao dịch.';
        END
        ELSE IF @TrangThaiCu = N'Đã khóa'
        BEGIN
            SET @KetQua = N'Từ chối';
            SET @ThongDiep = N'Hạn mức đang bị khóa, chưa thể ghi giao dịch (BR06).';
        END
        ELSE
        BEGIN
            SET @TongDuKien = @SoTienDaChi + @SoTien;
            SET @TyLe = CAST(@TongDuKien * 100.0 / @SoTienHanMuc AS decimal(9,2));

            IF @TongDuKien > @SoTienHanMuc AND @Loai = N'Cứng'
            BEGIN
                SET @SoConLai = @SoTienHanMuc - @SoTienDaChi;

                INSERT INTO dbo.NhatKyViPham (MaHanMuc, SoTienDinhChi, SoConLaiLucDo, KetQua)
                VALUES (@MaHanMuc, @SoTien, @SoConLai, N'Từ chối');

                SET @KetQua = N'Từ chối';
                SET @ThongDiep = CONCAT(N'Giao dịch vượt hạn mức còn lại (còn ',
                                        FORMAT(@SoConLai, 'N0', 'vi-VN'), N' đ). Không thể ghi nhận.');
            END
            ELSE IF @TongDuKien > @SoTienHanMuc
            BEGIN
                INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, MoTa, TrangThai)
                VALUES (@MaHanMuc, @SoTien, @NgayGiaoDich, @MoTa, N'Chờ duyệt');
                SET @MaGiaoDich = SCOPE_IDENTITY();

                INSERT INTO dbo.YeuCauNgoaiLe (MaGiaoDich) VALUES (@MaGiaoDich);

                SET @SoConLai = @SoTienHanMuc - @SoTienDaChi;
                INSERT INTO dbo.NhatKyViPham (MaHanMuc, SoTienDinhChi, SoConLaiLucDo, KetQua)
                VALUES (@MaHanMuc, @SoTien, @SoConLai, N'Gửi chờ duyệt');

                SELECT @MaNguoiCap = ISNULL(MaNguoiCapNganSach, MaNguoiDung)
                FROM dbo.NguoiDung WHERE MaNguoiDung = @MaSinhVien;

                INSERT INTO dbo.ThongBao (MaNguoiNhan, MaHanMuc, LoaiThongBao, NoiDung)
                VALUES (@MaNguoiCap, @MaHanMuc, N'Yêu cầu duyệt',
                        CONCAT(N'Có giao dịch ', FORMAT(@SoTien, 'N0', 'vi-VN'), N' đ (', @TenDanhMuc,
                               N') vượt hạn mức, đang chờ duyệt.'));

                SET @KetQua = N'Chờ duyệt';
                SET @ThongDiep = N'Giao dịch vượt hạn mức, đã chuyển người cấp ngân sách duyệt.';
            END
            ELSE
            BEGIN
                INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, MoTa, TrangThai)
                VALUES (@MaHanMuc, @SoTien, @NgayGiaoDich, @MoTa, N'Hợp lệ');
                SET @MaGiaoDich = SCOPE_IDENTITY();

                SET @TrangThaiMoi = dbo.fn_TinhTrangThai(@TongDuKien, @SoTienHanMuc, @Nguong);

                UPDATE dbo.HanMuc
                SET SoTienDaChi = @TongDuKien,
                    TrangThai   = @TrangThaiMoi
                WHERE MaHanMuc = @MaHanMuc;

                IF @TrangThaiMoi <> @TrangThaiCu AND @TrangThaiMoi IN (N'Sắp vượt', N'Đã vượt')
                    INSERT INTO dbo.ThongBao (MaNguoiNhan, MaHanMuc, LoaiThongBao, NoiDung)
                    VALUES (@MaSinhVien, @MaHanMuc,
                            CASE WHEN @TrangThaiMoi = N'Đã vượt' THEN N'Đã vượt' ELSE N'Ngưỡng' END,
                            CONCAT(@TenDanhMuc, N': đã dùng ', FORMAT(@TyLe, '0.##'), N'% hạn mức.'));

                SET @SoConLai = @SoTienHanMuc - @TongDuKien;
                SET @KetQua = CASE WHEN @TongDuKien = @SoTienHanMuc THEN N'Chấp nhận đúng mức'
                                   WHEN @TyLe >= @Nguong            THEN N'Chấp nhận kèm cảnh báo'
                                   ELSE N'Chấp nhận' END;
                SET @ThongDiep = CASE
                    WHEN @TongDuKien = @SoTienHanMuc THEN N'Đã ghi nhận. Giao dịch này dùng hết hạn mức.'
                    ELSE CONCAT(N'Đã ghi nhận. Đã dùng ', FORMAT(@TyLe, '0.##'), N'% hạn mức, còn lại ',
                                FORMAT(@SoConLai, 'N0', 'vi-VN'), N' đ.') END;

                SET @Json = (SELECT MaHanMuc, SoTien, NgayGiaoDich, TrangThai
                             FROM dbo.GiaoDich WHERE MaGiaoDich = @MaGiaoDich
                             FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
                INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriMoi)
                VALUES (@MaSinhVien, N'Ghi giao dịch', 'GiaoDich', @MaGiaoDich, @Json);
            END
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

KetThuc:
    SELECT @KetQua     AS KetQua,
           @MaGiaoDich AS MaGiaoDich,
           @TyLe       AS TyLeDuKien,
           @SoConLai   AS SoConLai,
           @ThongDiep  AS ThongDiep;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DuyetNgoaiLe
    @MaYeuCau     int,
    @MaNguoiDuyet int,
    @DongY        bit,
    @GhiChu       nvarchar(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MaGiaoDich int, @TrangThaiYC nvarchar(20), @MaHanMuc int, @SoTien decimal(18,0),
            @MaSinhVien int, @TrangThaiHM nvarchar(20), @SoTienHanMuc decimal(18,0),
            @SoTienDaChi decimal(18,0), @Nguong decimal(5,2), @TrangThaiMoi nvarchar(20),
            @NoiDung nvarchar(500);

    IF @DongY = 0 AND LEN(LTRIM(RTRIM(ISNULL(@GhiChu, N'')))) = 0
        THROW 50011, N'Từ chối phải ghi lý do.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @MaGiaoDich = MaGiaoDich, @TrangThaiYC = TrangThai
        FROM dbo.YeuCauNgoaiLe WITH (UPDLOCK, HOLDLOCK)
        WHERE MaYeuCau = @MaYeuCau;

        IF @MaGiaoDich IS NULL
            THROW 50012, N'Không tìm thấy yêu cầu duyệt.', 1;
        IF @TrangThaiYC <> N'Chờ duyệt'
            THROW 50013, N'Yêu cầu này đã được xử lý trước đó.', 1;

        SELECT @MaHanMuc = MaHanMuc, @SoTien = SoTien
        FROM dbo.GiaoDich WITH (UPDLOCK, HOLDLOCK)
        WHERE MaGiaoDich = @MaGiaoDich;

        SELECT @MaSinhVien   = MaSinhVien,
               @TrangThaiHM  = TrangThai,
               @SoTienHanMuc = SoTienHanMuc,
               @SoTienDaChi  = SoTienDaChi,
               @Nguong       = NguongCanhBao
        FROM dbo.HanMuc WITH (UPDLOCK, HOLDLOCK)
        WHERE MaHanMuc = @MaHanMuc;

        IF dbo.fn_CoQuyenQuanLy(@MaNguoiDuyet, @MaSinhVien) = 0
            THROW 50014, N'Bạn không có quyền duyệt yêu cầu này (BR08).', 1;

        IF @DongY = 1
        BEGIN
            IF @TrangThaiHM = N'Đã khóa'
                THROW 50015, N'Hạn mức đang bị khóa, không thể duyệt (BR06).', 1;

            UPDATE dbo.GiaoDich SET TrangThai = N'Hợp lệ' WHERE MaGiaoDich = @MaGiaoDich;

            SET @SoTienDaChi = @SoTienDaChi + @SoTien;
            SET @TrangThaiMoi = dbo.fn_TinhTrangThai(@SoTienDaChi, @SoTienHanMuc, @Nguong);

            UPDATE dbo.HanMuc
            SET SoTienDaChi = @SoTienDaChi,
                TrangThai   = @TrangThaiMoi
            WHERE MaHanMuc = @MaHanMuc;

            UPDATE dbo.YeuCauNgoaiLe
            SET TrangThai = N'Đã duyệt', MaNguoiDuyet = @MaNguoiDuyet,
                NgayDuyet = SYSDATETIME(), GhiChu = @GhiChu
            WHERE MaYeuCau = @MaYeuCau;

            SET @NoiDung = CONCAT(N'Giao dịch ', FORMAT(@SoTien, 'N0', 'vi-VN'), N' đ đã được duyệt.');
        END
        ELSE
        BEGIN
            UPDATE dbo.GiaoDich SET TrangThai = N'Từ chối' WHERE MaGiaoDich = @MaGiaoDich;

            UPDATE dbo.YeuCauNgoaiLe
            SET TrangThai = N'Từ chối', MaNguoiDuyet = @MaNguoiDuyet,
                NgayDuyet = SYSDATETIME(), GhiChu = @GhiChu
            WHERE MaYeuCau = @MaYeuCau;

            SET @NoiDung = LEFT(CONCAT(N'Giao dịch ', FORMAT(@SoTien, 'N0', 'vi-VN'),
                                       N' đ bị từ chối. Lý do: ', @GhiChu), 500);
        END

        INSERT INTO dbo.ThongBao (MaNguoiNhan, MaHanMuc, LoaiThongBao, NoiDung)
        VALUES (@MaSinhVien, @MaHanMuc, N'Kết quả duyệt', @NoiDung);

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriCu, GiaTriMoi)
        VALUES (@MaNguoiDuyet,
                CASE WHEN @DongY = 1 THEN N'Duyệt ngoại lệ' ELSE N'Từ chối ngoại lệ' END,
                'GiaoDich', @MaGiaoDich,
                N'{"TrangThai":"Chờ duyệt"}',
                CASE WHEN @DongY = 1 THEN N'{"TrangThai":"Hợp lệ"}' ELSE N'{"TrangThai":"Từ chối"}' END);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaYeuCau AS MaYeuCau,
           CASE WHEN @DongY = 1 THEN N'Đã duyệt' ELSE N'Từ chối' END AS KetQua,
           @NoiDung AS ThongDiep;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_HuyGiaoDich
    @MaGiaoDich int,
    @MaNguoiHuy int,
    @LyDo       nvarchar(255)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MaHanMuc int, @SoTien decimal(18,0), @TrangThaiGD nvarchar(20),
            @MaSinhVien int, @TrangThaiHM nvarchar(20), @SoTienHanMuc decimal(18,0),
            @SoTienDaChi decimal(18,0), @Nguong decimal(5,2), @TrangThaiMoi nvarchar(20);

    IF LEN(LTRIM(RTRIM(ISNULL(@LyDo, N'')))) = 0
        THROW 50021, N'Hủy giao dịch phải ghi lý do (BR07).', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @MaHanMuc = MaHanMuc, @SoTien = SoTien, @TrangThaiGD = TrangThai
        FROM dbo.GiaoDich WITH (UPDLOCK, HOLDLOCK)
        WHERE MaGiaoDich = @MaGiaoDich;

        IF @MaHanMuc IS NULL
            THROW 50022, N'Không tìm thấy giao dịch.', 1;
        IF @TrangThaiGD NOT IN (N'Hợp lệ', N'Chờ duyệt')
            THROW 50023, N'Chỉ hủy được giao dịch Hợp lệ hoặc Chờ duyệt.', 1;

        SELECT @MaSinhVien   = MaSinhVien,
               @TrangThaiHM  = TrangThai,
               @SoTienHanMuc = SoTienHanMuc,
               @SoTienDaChi  = SoTienDaChi,
               @Nguong       = NguongCanhBao
        FROM dbo.HanMuc WITH (UPDLOCK, HOLDLOCK)
        WHERE MaHanMuc = @MaHanMuc;

        IF dbo.fn_CoQuyenQuanLy(@MaNguoiHuy, @MaSinhVien) = 0
            THROW 50024, N'Bạn không có quyền hủy giao dịch này (BR08).', 1;

        UPDATE dbo.GiaoDich
        SET TrangThai = N'Đã hủy', LyDoHuy = @LyDo,
            MaNguoiHuy = @MaNguoiHuy, NgayHuy = SYSDATETIME()
        WHERE MaGiaoDich = @MaGiaoDich;

        IF @TrangThaiGD = N'Hợp lệ'
        BEGIN
            SET @SoTienDaChi = @SoTienDaChi - @SoTien;
            SET @TrangThaiMoi = CASE WHEN @TrangThaiHM = N'Đã khóa' THEN N'Đã khóa'
                                     ELSE dbo.fn_TinhTrangThai(@SoTienDaChi, @SoTienHanMuc, @Nguong) END;

            UPDATE dbo.HanMuc
            SET SoTienDaChi = @SoTienDaChi,
                TrangThai   = @TrangThaiMoi
            WHERE MaHanMuc = @MaHanMuc;
        END
        ELSE
        BEGIN
            UPDATE dbo.YeuCauNgoaiLe
            SET TrangThai = N'Từ chối', MaNguoiDuyet = @MaNguoiHuy, NgayDuyet = SYSDATETIME(),
                GhiChu = LEFT(CONCAT(N'Giao dịch đã bị hủy: ', @LyDo), 255)
            WHERE MaGiaoDich = @MaGiaoDich AND TrangThai = N'Chờ duyệt';
        END

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriCu, GiaTriMoi)
        VALUES (@MaNguoiHuy, N'Hủy giao dịch', 'GiaoDich', @MaGiaoDich,
                CONCAT(N'{"TrangThai":"', @TrangThaiGD, N'","SoTien":', @SoTien, N'}'),
                N'{"TrangThai":"Đã hủy"}');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaGiaoDich AS MaGiaoDich, N'Đã hủy' AS KetQua, @SoTienDaChi AS SoTienDaChiMoi;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DieuChinhHanMuc
    @MaHanMuc      int,
    @MaNguoi       int,
    @SoTienHanMuc  decimal(18,0),
    @LoaiHanMuc    nvarchar(10),
    @NguongCanhBao decimal(5,2),
    @PhienBan      binary(8)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @MaSinhVien int, @TrangThaiCu nvarchar(20), @SoTienDaChi decimal(18,0),
            @PhienBanHienTai binary(8), @TrangThaiMoi nvarchar(20),
            @GiaTriCu nvarchar(max), @GiaTriMoi nvarchar(max), @Msg nvarchar(255);

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @MaSinhVien      = MaSinhVien,
               @TrangThaiCu     = TrangThai,
               @SoTienDaChi     = SoTienDaChi,
               @PhienBanHienTai = PhienBan
        FROM dbo.HanMuc WITH (UPDLOCK, HOLDLOCK)
        WHERE MaHanMuc = @MaHanMuc;

        IF @MaSinhVien IS NULL
            THROW 50033, N'Không tìm thấy hạn mức.', 1;

        IF dbo.fn_CoQuyenQuanLy(@MaNguoi, @MaSinhVien) = 0
            THROW 50036, N'Bạn không có quyền điều chỉnh hạn mức này (BR08).', 1;

        IF @PhienBanHienTai <> @PhienBan
            THROW 50035, N'Hạn mức vừa bị người khác sửa. Hãy đóng form, tải lại danh sách rồi sửa lại.', 1;

        IF @LoaiHanMuc = N'Cứng' AND @SoTienHanMuc < @SoTienDaChi
        BEGIN
            SET @Msg = CONCAT(N'Hạn mức cứng không được nhỏ hơn số đã chi (',
                              FORMAT(@SoTienDaChi, 'N0', 'vi-VN'), N' đ).');
            THROW 50037, @Msg, 1;
        END

        SET @GiaTriCu = (SELECT SoTienHanMuc, LoaiHanMuc, NguongCanhBao, TrangThai
                         FROM dbo.HanMuc WHERE MaHanMuc = @MaHanMuc
                         FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        SET @TrangThaiMoi = CASE WHEN @TrangThaiCu = N'Đã khóa' THEN N'Đã khóa'
                                 ELSE dbo.fn_TinhTrangThai(@SoTienDaChi, @SoTienHanMuc, @NguongCanhBao) END;

        UPDATE dbo.HanMuc
        SET SoTienHanMuc  = @SoTienHanMuc,
            LoaiHanMuc    = @LoaiHanMuc,
            NguongCanhBao = @NguongCanhBao,
            TrangThai     = @TrangThaiMoi
        WHERE MaHanMuc = @MaHanMuc;

        SET @GiaTriMoi = (SELECT SoTienHanMuc, LoaiHanMuc, NguongCanhBao, TrangThai
                          FROM dbo.HanMuc WHERE MaHanMuc = @MaHanMuc
                          FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriCu, GiaTriMoi)
        VALUES (@MaNguoi, N'Điều chỉnh hạn mức', 'HanMuc', @MaHanMuc, @GiaTriCu, @GiaTriMoi);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaHanMuc AS MaHanMuc, @TrangThaiMoi AS TrangThai;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_LuuDanhMuc
    @MaNguoi    int,
    @MaDanhMuc  int = NULL,
    @TenDanhMuc nvarchar(100),
    @MoTa       nvarchar(255) = NULL,
    @HoatDong   bit = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @GiaTriCu nvarchar(max), @GiaTriMoi nvarchar(max);

    IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung
                   WHERE MaNguoiDung = @MaNguoi AND VaiTro = N'QuanTri' AND HoatDong = 1)
        THROW 50041, N'Chỉ quản trị viên được thực hiện thao tác này.', 1;

    IF LEN(LTRIM(RTRIM(ISNULL(@TenDanhMuc, N'')))) = 0
        THROW 50042, N'Tên danh mục không được để trống.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @MaDanhMuc IS NULL
        BEGIN
            INSERT INTO dbo.DanhMuc (TenDanhMuc, MoTa, HoatDong)
            VALUES (LTRIM(RTRIM(@TenDanhMuc)), @MoTa, @HoatDong);
            SET @MaDanhMuc = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.DanhMuc WITH (UPDLOCK, HOLDLOCK) WHERE MaDanhMuc = @MaDanhMuc)
                THROW 50043, N'Không tìm thấy danh mục.', 1;

            SET @GiaTriCu = (SELECT TenDanhMuc, MoTa, HoatDong FROM dbo.DanhMuc
                             WHERE MaDanhMuc = @MaDanhMuc FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

            UPDATE dbo.DanhMuc
            SET TenDanhMuc = LTRIM(RTRIM(@TenDanhMuc)), MoTa = @MoTa, HoatDong = @HoatDong
            WHERE MaDanhMuc = @MaDanhMuc;
        END

        SET @GiaTriMoi = (SELECT TenDanhMuc, MoTa, HoatDong FROM dbo.DanhMuc
                          WHERE MaDanhMuc = @MaDanhMuc FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriCu, GiaTriMoi)
        VALUES (@MaNguoi, CASE WHEN @GiaTriCu IS NULL THEN N'Thêm danh mục' ELSE N'Sửa danh mục' END,
                'DanhMuc', @MaDanhMuc, @GiaTriCu, @GiaTriMoi);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaDanhMuc AS MaDanhMuc;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_LuuNguoiDung
    @MaNguoi            int,
    @MaNguoiDung        int = NULL,
    @TenDangNhap        varchar(50),
    @MatKhauMaHoa       varchar(200) = NULL,
    @HoTen              nvarchar(100),
    @Email              varchar(100) = NULL,
    @VaiTro             nvarchar(20),
    @MaNguoiCapNganSach int = NULL,
    @HoatDong           bit = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @GiaTriCu nvarchar(max), @GiaTriMoi nvarchar(max), @VaiTroCu nvarchar(20);

    IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung
                   WHERE MaNguoiDung = @MaNguoi AND VaiTro = N'QuanTri' AND HoatDong = 1)
        THROW 50041, N'Chỉ quản trị viên được thực hiện thao tác này.', 1;

    IF @VaiTro <> N'SinhVien'
        SET @MaNguoiCapNganSach = NULL;

    IF @MaNguoiCapNganSach IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.NguoiDung
                       WHERE MaNguoiDung = @MaNguoiCapNganSach AND VaiTro = N'CapNganSach')
        THROW 50044, N'Người cấp ngân sách được chọn không hợp lệ.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @MaNguoiDung IS NULL
        BEGIN
            IF @MatKhauMaHoa IS NULL
                THROW 50045, N'Tài khoản mới phải có mật khẩu.', 1;

            INSERT INTO dbo.NguoiDung (TenDangNhap, MatKhauMaHoa, HoTen, Email, VaiTro, MaNguoiCapNganSach, HoatDong)
            VALUES (LTRIM(RTRIM(@TenDangNhap)), @MatKhauMaHoa, @HoTen, @Email, @VaiTro, @MaNguoiCapNganSach, @HoatDong);
            SET @MaNguoiDung = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            SELECT @VaiTroCu = VaiTro FROM dbo.NguoiDung WITH (UPDLOCK, HOLDLOCK) WHERE MaNguoiDung = @MaNguoiDung;
            IF @VaiTroCu IS NULL
                THROW 50043, N'Không tìm thấy người dùng.', 1;

            IF @MaNguoiDung = @MaNguoi AND (@HoatDong = 0 OR @VaiTro <> N'QuanTri')
                THROW 50046, N'Không thể tự khóa hoặc tự bỏ quyền quản trị của chính mình.', 1;

            IF @VaiTroCu = N'CapNganSach' AND @VaiTro <> N'CapNganSach'
               AND EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE MaNguoiCapNganSach = @MaNguoiDung)
                THROW 50047, N'Người này đang cấp ngân sách cho sinh viên khác, không đổi vai trò được.', 1;

            SET @GiaTriCu = (SELECT HoTen, Email, VaiTro, MaNguoiCapNganSach, HoatDong FROM dbo.NguoiDung
                             WHERE MaNguoiDung = @MaNguoiDung FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

            UPDATE dbo.NguoiDung
            SET HoTen = @HoTen, Email = @Email, VaiTro = @VaiTro,
                MaNguoiCapNganSach = @MaNguoiCapNganSach, HoatDong = @HoatDong
            WHERE MaNguoiDung = @MaNguoiDung;
        END

        SET @GiaTriMoi = (SELECT TenDangNhap, HoTen, Email, VaiTro, MaNguoiCapNganSach, HoatDong FROM dbo.NguoiDung
                          WHERE MaNguoiDung = @MaNguoiDung FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

        INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe, GiaTriCu, GiaTriMoi)
        VALUES (@MaNguoi, CASE WHEN @GiaTriCu IS NULL THEN N'Thêm người dùng' ELSE N'Sửa người dùng' END,
                'NguoiDung', @MaNguoiDung, @GiaTriCu, @GiaTriMoi);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @MaNguoiDung AS MaNguoiDung;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DatLaiMatKhau
    @MaNguoi      int,
    @MaNguoiDung  int,
    @MatKhauMaHoa varchar(200)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung
                   WHERE MaNguoiDung = @MaNguoi AND VaiTro = N'QuanTri' AND HoatDong = 1)
        THROW 50041, N'Chỉ quản trị viên được thực hiện thao tác này.', 1;

    BEGIN TRANSACTION;

    UPDATE dbo.NguoiDung SET MatKhauMaHoa = @MatKhauMaHoa WHERE MaNguoiDung = @MaNguoiDung;

    IF @@ROWCOUNT = 0
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50043, N'Không tìm thấy người dùng.', 1;
    END

    INSERT INTO dbo.NhatKyHeThong (MaNguoiDung, HanhDong, TenThucThe, MaThucThe)
    VALUES (@MaNguoi, N'Đặt lại mật khẩu', 'NguoiDung', @MaNguoiDung);

    COMMIT TRANSACTION;
END
GO

DECLARE @MatKhau varchar(200) = CONVERT(varchar(200), HASHBYTES('SHA2_256', '123456'), 2);

INSERT INTO dbo.NguoiDung (TenDangNhap, MatKhauMaHoa, HoTen, Email, VaiTro, MaNguoiCapNganSach) VALUES
('admin',     @MatKhau, N'Quản trị viên',  NULL, N'QuanTri',     NULL),
('phuhuynh1', @MatKhau, N'Lê Văn Hùng',    NULL, N'CapNganSach', NULL),
('sinhvien1', @MatKhau, N'Lê Minh Khoa',   NULL, N'SinhVien',    2),
('sinhvien2', @MatKhau, N'Trần Thu Hà',    NULL, N'SinhVien',    2),
('tuquanly1', @MatKhau, N'Nguyễn An Bình', NULL, N'SinhVien',    NULL);

INSERT INTO dbo.DanhMuc (TenDanhMuc, MoTa) VALUES
(N'Mua sắm trực tuyến', N'Sàn thương mại điện tử, mua online'),
(N'Ăn uống',            N'Ăn ngoài, gọi đồ ăn'),
(N'Đi lại',             N'Xe buýt, xe công nghệ, xăng'),
(N'Học tập',            N'Sách, khóa học, tài liệu');

DECLARE @Dau  date = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
DECLARE @Cuoi date = EOMONTH(GETDATE());

INSERT INTO dbo.HanMuc (MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc, SoTienHanMuc, LoaiHanMuc, MaNguoiTao) VALUES
(3, 1, @Dau, @Cuoi, 2000000, N'Cứng', 2),
(3, 2, @Dau, @Cuoi, 1500000, N'Mềm',  2),
(4, 3, @Dau, @Cuoi,  500000, N'Cứng', 2),
(3, 3, @Dau, @Cuoi,  300000, N'Mềm',  2),
(5, 2, @Dau, @Cuoi, 1000000, N'Mềm',  5);

INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, MoTa, TrangThai) VALUES
(1, 500000, @Dau,                  N'Áo khoác',        N'Hợp lệ'),
(1, 600000, DATEADD(DAY, 1, @Dau), N'Tai nghe',        N'Hợp lệ'),
(1, 300000, DATEADD(DAY, 2, @Dau), N'Sách tham khảo',  N'Hợp lệ'),
(2, 700000, @Dau,                  N'Tiền ăn tuần 1',  N'Hợp lệ'),
(2, 680000, DATEADD(DAY, 7, @Dau), N'Tiền ăn tuần 2',  N'Hợp lệ');

UPDATE h
SET SoTienDaChi = x.Tong,
    TrangThai   = dbo.fn_TinhTrangThai(x.Tong, h.SoTienHanMuc, h.NguongCanhBao)
FROM dbo.HanMuc h
JOIN (SELECT MaHanMuc, SUM(SoTien) AS Tong
      FROM dbo.GiaoDich
      WHERE TrangThai = N'Hợp lệ'
      GROUP BY MaHanMuc) x
  ON x.MaHanMuc = h.MaHanMuc;

UPDATE dbo.HanMuc SET TrangThai = N'Đã khóa' WHERE MaHanMuc = 4;

INSERT INTO dbo.ThongBao (MaNguoiNhan, MaHanMuc, LoaiThongBao, NoiDung) VALUES
(3, 2, N'Ngưỡng', N'Ăn uống: đã dùng 92% hạn mức.');
GO

SELECT * FROM dbo.vw_TinhTrangHanMuc ORDER BY MaHanMuc;
SELECT * FROM dbo.vw_DoiSoatHanMuc   ORDER BY MaHanMuc;
GO

SET NOEXEC ON;
GO
SELECT * FROM dbo.vw_TinhTrangHanMuc ORDER BY MaHanMuc;
GO

INSERT INTO dbo.NguoiDung (TenDangNhap, MatKhauMaHoa, HoTen, VaiTro)
VALUES ('gv1', 'x', N'Giáo viên', N'GiaoVien');
GO

INSERT INTO dbo.NguoiDung (TenDangNhap, MatKhauMaHoa, HoTen, VaiTro)
VALUES ('sinhvien1', 'x', N'Trùng tên', N'SinhVien');
GO

DELETE FROM dbo.DanhMuc WHERE MaDanhMuc = 1;
GO

UPDATE dbo.HanMuc SET SoTienDaChi = 2000001 WHERE MaHanMuc = 1;
GO

INSERT INTO dbo.HanMuc (MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc, SoTienHanMuc, LoaiHanMuc, MaNguoiTao)
VALUES (3, 4, '2026-12-31', '2026-12-01', 500000, N'Cứng', 2);
GO

UPDATE dbo.GiaoDich SET TrangThai = N'Đã hủy' WHERE MaGiaoDich = 1;
GO

INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, TrangThai)
VALUES (1, 0, CAST(GETDATE() AS date), N'Hợp lệ');
GO

INSERT INTO dbo.HanMuc (MaSinhVien, MaDanhMuc, NgayBatDau, NgayKetThuc, SoTienHanMuc, LoaiHanMuc, MaNguoiTao)
VALUES (3, 1, CAST(GETDATE() AS date), DATEADD(DAY, 30, CAST(GETDATE() AS date)), 1000000, N'Cứng', 2);
GO

INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, TrangThai)
VALUES (1, 10000, DATEADD(DAY, 1, EOMONTH(GETDATE())), N'Hợp lệ');
GO

INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, TrangThai)
VALUES (4, 10000, CAST(GETDATE() AS date), N'Hợp lệ');
GO

INSERT INTO dbo.GiaoDich (MaHanMuc, SoTien, NgayGiaoDich, TrangThai)
VALUES (1, 700000, CAST(GETDATE() AS date), N'Hợp lệ');
GO

SELECT * FROM dbo.vw_DoiSoatHanMuc ORDER BY MaHanMuc;
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 1, @SoTien = 700000, @MoTa = N'Giày thể thao';
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 1, @SoTien = 100000, @MoTa = N'Ốp điện thoại';
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 1, @SoTien = 100000, @MoTa = N'Mua sách, tài liệu';
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 1, @SoTien = 400000, @MoTa = N'Balo';
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 1, @SoTien = 1000;
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 1, @SoTien = 0;
EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 4, @SoTien = 50000;
GO

SELECT * FROM dbo.vw_TinhTrangHanMuc WHERE MaHanMuc = 1;
SELECT * FROM dbo.GiaoDich     WHERE MaHanMuc = 1 ORDER BY MaGiaoDich;
SELECT * FROM dbo.NhatKyViPham WHERE MaHanMuc = 1 ORDER BY MaViPham;
SELECT * FROM dbo.ThongBao     WHERE MaHanMuc = 1 ORDER BY MaThongBao;
SELECT * FROM dbo.vw_DoiSoatHanMuc WHERE MaHanMuc = 1;
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 2, @SoTien = 250000, @MoTa = N'Liên hoan lớp';
GO

SELECT yc.MaYeuCau, sv.HoTen, dm.TenDanhMuc, gd.SoTien,
       h.SoTienHanMuc - h.SoTienDaChi                 AS ConLaiTruocGiaoDich,
       gd.SoTien - (h.SoTienHanMuc - h.SoTienDaChi)   AS VuotBaoNhieu,
       yc.NgayYeuCau
FROM dbo.YeuCauNgoaiLe yc
JOIN dbo.GiaoDich  gd ON gd.MaGiaoDich = yc.MaGiaoDich
JOIN dbo.HanMuc    h  ON h.MaHanMuc    = gd.MaHanMuc
JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien
JOIN dbo.DanhMuc   dm ON dm.MaDanhMuc  = h.MaDanhMuc
WHERE yc.TrangThai = N'Chờ duyệt'
ORDER BY yc.NgayYeuCau;
SELECT * FROM dbo.ThongBao WHERE LoaiThongBao = N'Yêu cầu duyệt';
GO

DECLARE @YC int = (SELECT MIN(MaYeuCau) FROM dbo.YeuCauNgoaiLe WHERE TrangThai = N'Chờ duyệt');
EXEC dbo.sp_DuyetNgoaiLe @MaYeuCau = @YC, @MaNguoiDuyet = 3, @DongY = 1;
GO

DECLARE @YC int = (SELECT MIN(MaYeuCau) FROM dbo.YeuCauNgoaiLe WHERE TrangThai = N'Chờ duyệt');
EXEC dbo.sp_DuyetNgoaiLe @MaYeuCau = @YC, @MaNguoiDuyet = 2, @DongY = 1, @GhiChu = N'Đồng ý, liên hoan cuối kỳ';
SELECT * FROM dbo.vw_TinhTrangHanMuc WHERE MaHanMuc = 2;
GO

EXEC dbo.sp_DuyetNgoaiLe @MaYeuCau = 1, @MaNguoiDuyet = 2, @DongY = 1;
GO

EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 3, @MaDanhMuc = 2, @SoTien = 90000, @MoTa = N'Trà sữa';
DECLARE @YC int = (SELECT MIN(MaYeuCau) FROM dbo.YeuCauNgoaiLe WHERE TrangThai = N'Chờ duyệt');
EXEC dbo.sp_DuyetNgoaiLe @MaYeuCau = @YC, @MaNguoiDuyet = 2, @DongY = 0;
GO

DECLARE @YC int = (SELECT MIN(MaYeuCau) FROM dbo.YeuCauNgoaiLe WHERE TrangThai = N'Chờ duyệt');
EXEC dbo.sp_DuyetNgoaiLe @MaYeuCau = @YC, @MaNguoiDuyet = 2, @DongY = 0, @GhiChu = N'Tháng này đã vượt rồi';
GO

EXEC dbo.sp_HuyGiaoDich @MaGiaoDich = 1, @MaNguoiHuy = 2, @LyDo = N'';
GO

DECLARE @GD int = (SELECT MAX(MaGiaoDich) FROM dbo.GiaoDich
                   WHERE MaHanMuc = 1 AND SoTien = 400000 AND TrangThai = N'Hợp lệ');
EXEC dbo.sp_HuyGiaoDich @MaGiaoDich = @GD, @MaNguoiHuy = 2, @LyDo = N'Nhập nhầm, đã trả hàng';
SELECT * FROM dbo.vw_TinhTrangHanMuc WHERE MaHanMuc = 1;
SELECT * FROM dbo.vw_DoiSoatHanMuc   WHERE MaHanMuc = 1;
GO

DECLARE @GD int = (SELECT MAX(MaGiaoDich) FROM dbo.GiaoDich WHERE TrangThai = N'Đã hủy');
EXEC dbo.sp_HuyGiaoDich @MaGiaoDich = @GD, @MaNguoiHuy = 2, @LyDo = N'Thử lại';
GO

EXEC dbo.sp_KhoaMoKhoaHanMuc @MaHanMuc = 3, @MaNguoi = 4, @Khoa = 1;
GO

EXEC dbo.sp_KhoaMoKhoaHanMuc @MaHanMuc = 3, @MaNguoi = 2, @Khoa = 1;
EXEC dbo.sp_GhiNhanGiaoDich @MaSinhVien = 4, @MaDanhMuc = 3, @SoTien = 50000;
EXEC dbo.sp_KhoaMoKhoaHanMuc @MaHanMuc = 3, @MaNguoi = 2, @Khoa = 0;
GO

DECLARE @Dau date = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
EXEC dbo.sp_TaoHanMuc @MaNguoiTao = 2, @MaSinhVien = 3, @MaDanhMuc = 4,
     @NgayBatDau = @Dau, @NgayKetThuc = '2099-12-31',
     @SoTienHanMuc = 800000, @LoaiHanMuc = N'Cứng', @NguongCanhBao = 75;
GO

DECLARE @Dau date = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
EXEC dbo.sp_TaoHanMuc @MaNguoiTao = 5, @MaSinhVien = 5, @MaDanhMuc = 3,
     @NgayBatDau = @Dau, @NgayKetThuc = '2099-12-31', @SoTienHanMuc = 300000, @LoaiHanMuc = N'Mềm';
EXEC dbo.sp_TaoHanMuc @MaNguoiTao = 3, @MaSinhVien = 3, @MaDanhMuc = 3,
     @NgayBatDau = @Dau, @NgayKetThuc = '2099-12-31', @SoTienHanMuc = 300000, @LoaiHanMuc = N'Mềm';
GO

DECLARE @PB binary(8) = (SELECT PhienBan FROM dbo.HanMuc WHERE MaHanMuc = 3);
EXEC dbo.sp_DieuChinhHanMuc @MaHanMuc = 3, @MaNguoi = 2, @SoTienHanMuc = 600000,
     @LoaiHanMuc = N'Cứng', @NguongCanhBao = 80, @PhienBan = @PB;
SELECT HanhDong, GiaTriCu, GiaTriMoi FROM dbo.NhatKyHeThong WHERE HanhDong = N'Điều chỉnh hạn mức';
GO

DECLARE @PB binary(8) = (SELECT PhienBan FROM dbo.HanMuc WHERE MaHanMuc = 1);
EXEC dbo.sp_DieuChinhHanMuc @MaHanMuc = 1, @MaNguoi = 2, @SoTienHanMuc = 1000000,
     @LoaiHanMuc = N'Cứng', @NguongCanhBao = 80, @PhienBan = @PB;
GO

DECLARE @PB binary(8) = (SELECT PhienBan FROM dbo.HanMuc WHERE MaHanMuc = 3);
UPDATE dbo.HanMuc SET NguongCanhBao = 85 WHERE MaHanMuc = 3;
EXEC dbo.sp_DieuChinhHanMuc @MaHanMuc = 3, @MaNguoi = 2, @SoTienHanMuc = 700000,
     @LoaiHanMuc = N'Cứng', @NguongCanhBao = 80, @PhienBan = @PB;
GO

EXEC dbo.sp_LuuDanhMuc @MaNguoi = 1, @TenDanhMuc = N'Giải trí', @MoTa = N'Phim, game, du lịch';
EXEC dbo.sp_LuuDanhMuc @MaNguoi = 3, @TenDanhMuc = N'Thử';
GO

EXEC dbo.sp_LuuNguoiDung @MaNguoi = 1, @MaNguoiDung = 1, @TenDangNhap = 'admin',
     @HoTen = N'Quản trị viên', @VaiTro = N'QuanTri', @HoatDong = 0;
GO

SELECT nk.NgayGio, nd.HoTen, nk.HanhDong, nk.TenThucThe, nk.MaThucThe, nk.GiaTriCu, nk.GiaTriMoi
FROM dbo.NhatKyHeThong nk
JOIN dbo.NguoiDung nd ON nd.MaNguoiDung = nk.MaNguoiDung
ORDER BY nk.MaNhatKy;
GO

BEGIN TRANSACTION;
SELECT * FROM dbo.HanMuc WITH (UPDLOCK, HOLDLOCK) WHERE MaHanMuc = 3;
GO

SELECT session_id, blocking_session_id, wait_type, wait_time
FROM sys.dm_exec_requests
WHERE blocking_session_id <> 0;
GO

COMMIT TRANSACTION;
GO

DECLARE @PhienBanLucDoc binary(8) = (SELECT PhienBan FROM dbo.HanMuc WHERE MaHanMuc = 5);

UPDATE dbo.HanMuc SET NguongCanhBao = 90 WHERE MaHanMuc = 5;

UPDATE dbo.HanMuc SET NguongCanhBao = 70
WHERE MaHanMuc = 5 AND PhienBan = @PhienBanLucDoc;

SELECT @@ROWCOUNT AS SoDongNguoiBCapNhat;
SELECT MaHanMuc, NguongCanhBao, PhienBan FROM dbo.HanMuc WHERE MaHanMuc = 5;
GO

SELECT MaHanMuc, TrangThai, SoTienHanMuc, SoTienDaChi
FROM dbo.HanMuc
WHERE MaSinhVien = 3 AND MaDanhMuc = 1
  AND CAST(GETDATE() AS date) BETWEEN NgayBatDau AND NgayKetThuc;
GO

SELECT t.name AS Bang, i.name AS ChiMuc, i.type_desc, i.is_unique, i.has_filter, i.filter_definition
FROM sys.indexes i
JOIN sys.tables  t ON t.object_id = i.object_id
WHERE i.name IS NOT NULL
ORDER BY t.name, i.index_id;
GO

SELECT dm.TenDanhMuc,
       h.SoTienHanMuc,
       SUM(CASE WHEN gd.TrangThai = N'Hợp lệ' THEN gd.SoTien ELSE 0 END) AS DaChi,
       COUNT(CASE WHEN gd.TrangThai = N'Hợp lệ' THEN 1 END)               AS SoGiaoDich,
       h.TrangThai
FROM dbo.HanMuc h
JOIN dbo.DanhMuc dm ON dm.MaDanhMuc = h.MaDanhMuc
LEFT JOIN dbo.GiaoDich gd ON gd.MaHanMuc = h.MaHanMuc
WHERE h.MaSinhVien = 3
  AND CAST(GETDATE() AS date) BETWEEN h.NgayBatDau AND h.NgayKetThuc
GROUP BY dm.TenDanhMuc, h.SoTienHanMuc, h.TrangThai
ORDER BY DaChi DESC;
GO

SELECT sv.HoTen, dm.TenDanhMuc, vp.SoTienDinhChi, vp.SoConLaiLucDo, vp.KetQua, vp.NgayXayRa
FROM dbo.NhatKyViPham vp
JOIN dbo.HanMuc    h  ON h.MaHanMuc    = vp.MaHanMuc
JOIN dbo.NguoiDung sv ON sv.MaNguoiDung = h.MaSinhVien
JOIN dbo.DanhMuc   dm ON dm.MaDanhMuc  = h.MaDanhMuc
ORDER BY vp.NgayXayRa DESC;
GO

SELECT * FROM dbo.vw_DoiSoatHanMuc ORDER BY MaHanMuc;
GO

SET NOEXEC OFF;
GO
