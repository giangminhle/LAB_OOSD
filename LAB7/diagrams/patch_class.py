# -*- coding: utf-8 -*-
"""Them 2 so do class diagram vao gen_diagrams.py"""
p = 'gen_diagrams.py'
s = open(p, encoding='utf-8').read()
anchor = 'if __name__ == "__main__":'
code = r'''
# =====================================================================
# 1-2. CLASS DIAGRAM (phan tich / thiet ke)
# =====================================================================
CLASSES = {
    # key: (title, x, y, w, attrs_analysis, attrs_design, methods)
    "PB": ("PhongBan", 60, 110, 760,
           ["- TenPB: string", "- DienThoai: string", "- TrangThai: bool"],
           ["- MaPB: int  {PK}", "- TenPB: string", "- TrangThai: bool"],
           ["+ themPhongBan(): bool", "+ khoaPhongBan(): bool"]),
    "NV": ("NhanVien", 60, 400, 760,
           ["- HoTen: string", "- GioiTinh: string", "- NgaySinh: Date", "- CCCD: string", "- TrangThai: string"],
           ["- MaNV: int  {PK}", "- HoTen: string", "- CCCD: string", "- MaPB: int  {FK}", "- TrangThai: string"],
           ["+ tiepNhanThuViec(): bool", "+ capNhatTrangThai(): void"]),
    "QDTD": ("QuyetDinhTuyenDung", 60, 800, 760,
             ["- SoQD: string", "- NgayBatDau: Date", "- NgayKetThuc: Date", "- MucLuongThuViec: decimal", "- TrangThai: string"],
             ["- MaQDTD: int  {PK}", "- SoQD: string", "- MaNV: int  {FK}", "- MaPB: int  {FK}", "- TrangThai: string"],
             ["+ lapQuyetDinh(): bool", "+ kiemTraThoiHan(): bool"]),
    "HD": ("HopDongLaoDong", 60, 1200, 760,
           ["- SoHD: string", "- LoaiHD: string", "- NgayHieuLuc: Date", "- NgayHetHan: Date?", "- MucLuong: decimal"],
           ["- MaHD: int  {PK}", "- SoHD: string", "- MaNV: int  {FK}", "- MaQDTD: int  {FK}", "- LoaiHD: string"],
           ["+ kyHopDong(): bool", "+ kiemTraLoaiHD(): bool"]),
    "BH": ("BaoHiem", 60, 1600, 760,
           ["- LoaiBH: string", "- SoSo: string", "- MucDong: decimal", "- TyLeNhanVien: decimal", "- TrangThai: string"],
           ["- MaBH: int  {PK}", "- MaNV: int  {FK}", "- MaHD: int  {FK}", "- LoaiBH: string", "- MucDong: decimal"],
           ["+ lapBaoHiem(): bool", "+ tinhKhauTru(): decimal"]),
    "QDCV": ("QuyetDinhChucVu", 60, 2000, 760,
             ["- ChucVu: string", "- PhuCapChucVu: decimal", "- NgayHieuLuc: Date", "- LyDo: string"],
             ["- MaQDCV: int  {PK}", "- MaNV: int  {FK}", "- MaPB: int  {FK}", "- ChucVu: string", "- NgayHieuLuc: Date"],
             ["+ lapQuyetDinh(): bool", "+ hieuLucTai(ngay): bool"]),

    "QDL": ("QuyetDinhLuong", 910, 110, 760,
            ["- MucLuongCoBan: decimal", "- PhuCapLuong: decimal", "- PhuCapDocHai: decimal", "- NgayHieuLuc: Date", "- LyDo: string"],
            ["- MaQDL: int  {PK}", "- MaNV: int  {FK}", "- MucLuongCoBan: decimal", "- PhuCapLuong: decimal", "- NgayHieuLuc: Date"],
            ["+ lapQuyetDinh(): bool", "+ hieuLucTai(ngay): bool"]),
    "KHT": ("KeHoachTuan", 910, 480, 760,
            ["- TuanSo: int", "- Nam: int", "- NgayLap: Date", "- NguoiLap: string", "- TrangThai: string"],
            ["- MaKH: int  {PK}", "- MaPB: int  {FK}", "- TuanSo: int", "- Nam: int", "- TrangThai: string"],
            ["+ lapKeHoach(): bool", "+ duyetKeHoach(): bool"]),
    "CTKH": ("ChiTietKeHoach", 910, 850, 760,
             ["- Ngay: Date", "- CaLam: string", "- LaPhuTroi: bool", "- GhiChu: string"],
             ["- MaCTKH: int  {PK}", "- MaKH: int  {FK}", "- MaNV: int  {FK}", "- Ngay: Date", "- CaLam: string"],
             ["+ themPhanCong(): bool", "+ kiemTraTrung(): bool"]),
    "CC": ("ChamCong", 910, 1220, 760,
           ["- Ngay: Date", "- CaLam: string", "- GioVao: Time", "- GioRa: Time", "- TrangThai: string"],
           ["- MaChamCong: int  {PK}", "- MaNV: int  {FK}", "- MaCTKH: int  {FK}", "- Ngay: Date", "- TrangThai: string"],
           ["+ chamCong(): bool", "+ tinhSoGioLam(): decimal"]),
    "PPT": ("PhieuPhuTroi", 910, 1625, 760,
            ["- SoPhieu: string", "- NgayPhieu: Date", "- HinhThuc: string", "- TrangThai: string", "- NguoiLap: string"],
            ["- MaPhieuPT: int  {PK}", "- SoPhieu: string", "- NgayPhieu: Date", "- HinhThuc: string", "- TrangThai: string"],
            ["+ lapPhieu(): bool", "+ duyetPhieu(): bool"]),
    "CTPT": ("ChiTietPhuTroi", 910, 1995, 760,
             ["- SoGio: decimal", "- HeSo: decimal", "- GhiChu: string"],
             ["- MaCTPT: int  {PK}", "- MaPhieuPT: int  {FK}", "- MaNV: int  {FK}", "- SoGio: decimal", "- HeSo: decimal"],
             ["+ themChiTiet(): bool", "+ tinhTien(): decimal"]),

    "KT": ("QuyetDinhKhenThuong", 1760, 110, 760,
           ["- SoQD: string", "- DoiTuong: string", "- LyDo: string", "- SoTien: decimal", "- NguonChiTra: string", "- TrangThai: string"],
           ["- MaQDKhenThuong: int {PK}", "- SoQD: string", "- MaNV: int  {FK}", "- MaPB: int  {FK}", "- SoTien: decimal", "- NguonChiTra: string"],
           ["+ lapQuyetDinh(): bool", "+ taoPhieuChi(): bool"]),
    "PC": ("PhieuChiKhenThuong", 1760, 560, 760,
           ["- SoPhieu: string", "- NgayChi: Date", "- SoTien: decimal", "- NguoiChi: string"],
           ["- MaPhieuChi: int  {PK}", "- SoPhieu: string", "- MaQDKhenThuong: int {FK}", "- NgayChi: Date", "- SoTien: decimal"],
           ["+ lapPhieuChi(): bool", "+ inPhieu(): void"]),
    "KL": ("QuyetDinhKyLuat", 1760, 930, 760,
           ["- SoQD: string", "- DoiTuong: string", "- LyDo: string", "- SoTienPhat: decimal", "- NguonThu: string", "- TrangThai: string"],
           ["- MaQDKyLuat: int  {PK}", "- SoQD: string", "- MaNV: int  {FK}", "- MaPB: int  {FK}", "- SoTienPhat: decimal", "- NguonThu: string"],
           ["+ lapQuyetDinh(): bool", "+ taoPhieuThu(): bool"]),
    "PT": ("PhieuThuNopPhat", 1760, 1380, 760,
           ["- SoPhieu: string", "- NgayThu: Date", "- SoTien: decimal", "- NguoiThu: string"],
           ["- MaPhieuThu: int  {PK}", "- SoPhieu: string", "- MaQDKyLuat: int  {FK}", "- NgayThu: Date", "- SoTien: decimal"],
           ["+ lapPhieuThu(): bool", "+ inPhieu(): void"]),
    "BL": ("BangLuong", 1760, 1750, 760,
           ["- Thang: int", "- Nam: int", "- NgayLap: Date", "- TrangThai: string", "- TongChiPhi: decimal"],
           ["- MaBangLuong: int {PK}", "- Thang: int", "- Nam: int", "- TrangThai: string", "- TongChiPhi: decimal"],
           ["+ tinhBangLuong(): bool", "+ chotBangLuong(): bool"]),
    "CTBL": ("ChiTietBangLuong", 1760, 2120, 760,
             ["- SoNgayCong: int", "- GioPhuTroi: decimal", "- ThucLinh: decimal", "- ... (15 truong luong)"],
             ["- MaCTBL: int  {PK}", "- MaBangLuong: int {FK}", "- MaNV: int  {FK}", "- ThucLinh: decimal"],
             ["+ tinhLuong(): decimal", "+ tinhThucLinh(): decimal"]),
}


def class_diagram(design=False):
    name = "2_ClassDiagram_Design" if design else "1_ClassDiagram_Analysis"
    title = ("SƠ ĐỒ LỚP - GIAI ĐOẠN THIẾT KẾ (có ID, khóa, phương thức)"
             if design else
             "SƠ ĐỒ LỚP - GIAI ĐOẠN PHÂN TÍCH (mô hình khái niệm)")
    c = Canvas(2600, 2640, name, title)
    c.text(30, 16, c.title, 30, True, C_BLUE if not design else C_PURPLE)
    c.text(30, 60, "Công ty Sơn Đông Á  |  18 lớp nghiệp vụ  |  Quan hệ 1 - 0..* thể hiện bản số",
           18, False, C_GRAY)

    ids = {}
    for key, (t, x, y, w, aa, ad, ms) in CLASSES.items():
        attrs = ad if design else aa
        vid, h = c.classbox(x, y, w, t, attrs, ms, size=26,
                            header=C_PURPLE_FILL if design else C_BLUE_FILL,
                            stroke=C_PURPLE if design else C_BLUE)
        ids[key] = vid

    # -------- associations --------
    A = dict(arrow="none", color=C_BLUE, lw=2, size=17)

    def E(a, b, pa, pb, m1, m2, label="", dashed=False, color=C_BLUE, wps=None):
        c.line([pa, pb], label=label, dashed=dashed, source=ids[a], target=ids[b],
               start_mult=m1, end_mult=m2, waypoints=wps, **dict(A, color=color))

    # cot 1: chuoi doc
    E("PB", "NV", (360, 347), (360, 400), "1", "0..*", "quản lý")
    E("NV", "QDTD", (360, 705), (360, 800), "1", "0..*", "được tuyển")
    E("QDTD", "HD", (360, 1105), (360, 1200), "0..1", "0..1", "làm cơ sở")
    E("HD", "BH", (360, 1505), (360, 1600), "1", "0..*", "đóng")
    # QDCV: qua kenh phai cot 1
    E("NV", "QDCV", (820, 600), (820, 2100), "1", "0..*", "bổ nhiệm",
      wps=[(850, 600), (850, 2100)])
    E("PB", "QDCV", (820, 250), (820, 2050), "1", "0..*", "",
      wps=[(875, 250), (875, 2050)])

    # NV -> cot 2 (fan trong khe 820-910)
    E("NV", "QDL", (820, 470), (910, 300), "1", "0..*", "điều chỉnh")
    E("NV", "KHT", (820, 540), (910, 560), "1", "0..*", "phân công")
    E("NV", "CTKH", (820, 610), (910, 880), "1", "0..*", "",
      wps=[(870, 610), (870, 880)])
    E("NV", "CC", (820, 660), (910, 1250), "1", "0..*", "chấm công",
      wps=[(885, 660), (885, 1250)])
    E("NV", "PPT", (820, 690), (910, 1680), "1", "0..*", "",
      wps=[(895, 690), (895, 1680)])
    E("NV", "CTPT", (820, 730), (910, 2040), "1", "0..*", "phụ trội",
      wps=[(905, 730), (905, 2040)])

    # cot 2: doc
    E("KHT", "CTKH", (1290, 785), (1290, 850), "1", "1..*", "gồm")
    E("CTKH", "CC", (1290, 1155), (1290, 1220), "0..1", "0..1", "theo dõi")
    E("PPT", "CTPT", (1290, 1930), (1290, 1995), "1", "1..*", "gồm")

    # cot 3: cap quyet dinh - phieu
    E("KT", "PC", (2140, 450), (2140, 560), "1", "0..1", "chi thưởng")
    E("KL", "PT", (2140, 1270), (2140, 1380), "1", "0..1", "nộp phạt")
    E("BL", "CTBL", (2140, 2055), (2140, 2120), "1", "1..*", "gồm")

    # PB -> khen thuong / ky luat (kenh tren)
    E("PB", "KT", (340, 110), (2140, 110), "1", "0..*", "khen thưởng",
      wps=[(340, 74), (2140, 74)])
    E("PB", "KL", (390, 110), (2520, 1100), "1", "0..*", "kỷ luật",
      wps=[(390, 88), (2560, 88), (2560, 1100)])

    # NV -> CTBL (kenh duoi) - the hien chi tiet luong cua nhan vien
    E("NV", "CTBL", (400, 705), (2520, 2260), "1", "0..*", "được tính",
      wps=[(400, 2560), (2560, 2560), (2560, 2260)])

    # -------- notes --------
    c.style_note(30, 2390, 1680, 210,
                 "CHÚ THÍCH\n"
                 "• Quan hệ không ghi bản số mặc định đọc theo chiều mũi nhọn: 1 phía nguồn - nhiều phía đích.\n"
                 "• Thành phần (composition): KeHoachTuan–ChiTietKeHoach, PhieuPhuTroi–ChiTietPhuTroi,\n"
                 "  BangLuong–ChiTietBangLuong (xóa cha thì xóa con).\n"
                 "• Các FK đã chuẩn hóa còn lại: ChiTietPhuTroi.MaNV, ChiTietBangLuong.MaNV,\n"
                 "  ChiTietKeHoach.MaNV → NhanVien; PhieuPhuTroi không lưu MaNV (chi tiết mới lưu).")
    c.text(1760, 2410, "Ghi chú thiết kế:", 18, True, C_PURPLE)
    c.text(1760, 2445,
           "• ID kỹ thuật: Ma + tên lớp (IDENTITY).\n"
           "• Mã nghiệp vụ: MaNVCode, SoQD, SoHD, SoPhieu.\n"
           "• FK đánh dấu {FK}; PK đánh dấu {PK}.\n"
           "• Kiểu tiền: DECIMAL(18,0), đơn vị VND.\n"
           "• Ngày: DATE; giờ: TIME(0).", 17, False, C_GRAY, "lt")
    c.save()


'''
s = s.replace(anchor, code + anchor)
open(p, 'w', encoding='utf-8').write(s)
print('inserted class diagram')
