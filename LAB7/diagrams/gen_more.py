# -*- coding: utf-8 -*-
"""Sinh cac so do 4-8."""
import math
from gen_diagrams import (Canvas, C_BLUE, C_BLUE_FILL, C_GOLD, C_GOLD_FILL,
                          C_GREEN, C_GREEN_FILL, C_GRAY, C_GRAY_FILL,
                          C_RED, C_RED_FILL, C_ORANGE_FILL, C_PURPLE,
                          C_PURPLE_FILL, C_WHITE, C_BLACK, S)


# =====================================================================
# 4. USE CASE PHAN RA TUYEN DUNG -> HOP DONG
# =====================================================================
def usecase_recruitment():
    c = Canvas(2300, 1480, "4_UseCase_PhanRa_TuyenDung",
               "USE CASE PHÂN RÃ: TỪ TIẾP NHẬN THỬ VIỆC ĐẾN HỢP ĐỒNG CHÍNH THỨC")
    c.text(30, 16, c.title, 29, True, C_BLUE)
    c.text(30, 58, "Câu 4 - Quy trình nghiệp vụ và các use case con", 18, False, C_GRAY)

    c.d.rectangle([220 * S, 90 * S, 2000 * S, 1400 * S], outline=C_BLUE, width=2 * S)
    c.text(1110, 102, "PHÂN RÃ USE CASE TUYỂN DỤNG", 20, True, C_BLUE, "mt")

    # actor
    a_uv = c.actor(110, 330, "Ứng viên /\nNhân viên")
    a_tp = c.actor(110, 760, "Trưởng phòng,\nban")
    a_hc = c.actor(2150, 380, "Phòng HCNS")
    a_gd = c.actor(2150, 800, "Giám đốc")

    main = c.ellipse(790, 130, 640, 100,
                     "Quy trình tiếp nhận thử việc\nvà ký hợp đồng chính thức",
                     fill="#FFE699", stroke=C_GOLD, size=20, bold=True)

    x1, x2, x3 = 280, 760, 1240
    w, h = 400, 96
    uc = {}

    def U(key, x, y, text, fill=C_GOLD_FILL):
        uc[key] = c.ellipse(x, y, w, h, text, fill=fill, size=18)

    U("tn", x1, 310, "1. Tiếp nhận hồ sơ\nứng viên")
    U("qdtv", x2, 310, "2. Lập QĐ tuyển dụng\nvà thử việc")
    U("kyhd", x3, 310, "7. Ký hợp đồng\nlao động chính thức")
    U("pc", x1, 560, "3. Phân công thử việc\ntại phòng ban")
    U("dg", x2, 560, "4. Theo dõi, đánh giá\nkết quả thử việc")
    U("qdcv", x3, 560, "5. Lập QĐ chức vụ\nvà phụ cấp")
    U("qdl", x1, 810, "6. Lập QĐ lương,\nphụ cấp lương/độc hại")
    U("bh", x2, 810, "8. Lập hồ sơ\nBHXH - BHYT")
    U("capnhat", x3, 810, "9. Cập nhật trạng thái\nĐang làm, lưu hồ sơ")

    # main -> sub use cases (include)
    for key in ("tn", "qdtv", "kyhd", "pc", "dg", "qdcv", "qdl", "bh", "capnhat"):
        e = uc[key]
        ex = e[0] if isinstance(e, tuple) else None
    # lay tam elip tu toa do da biet
    targets = {
        "tn": (x1 + w / 2, 310), "qdtv": (x2 + w / 2, 310), "kyhd": (x3 + w / 2, 310),
        "pc": (x1 + w / 2, 560), "dg": (x2 + w / 2, 560), "qdcv": (x3 + w / 2, 560),
        "qdl": (x1 + w / 2, 810), "bh": (x2 + w / 2, 810), "capnhat": (x3 + w / 2, 810),
    }
    for i, key in enumerate(("tn", "qdtv", "kyhd", "pc", "dg", "qdcv", "qdl", "bh", "capnhat")):
        tx, ty = targets[key]
        c.line([(1110, 230), (tx, ty)], "«include»" if key in ("tn", "qdtv", "kyhd") else "",
               dashed=True, arrow="open", color=C_PURPLE, lw=1.6, size=13)

    # extend: BHXH/BHYT chi voi hop dong dai han
    c.line([(960, 858), (1440, 406)], "«extend»", dashed=True, arrow="open",
           color=C_RED, lw=2, size=14)

    # actor associations
    def assoc(ax, ay, ex, ey):
        c.line([(ax, ay), (ex, ey)], arrow="none", color=C_BLUE, lw=2)

    assoc(190, 372, 280, 358)
    assoc(190, 802, 280, 608)
    assoc(190, 802, 760, 608)
    assoc(2070, 422, 1640, 358)
    assoc(2070, 422, 1640, 608)
    assoc(2070, 422, 760, 858)
    assoc(2070, 842, 1640, 358)
    assoc(2070, 842, 280, 858)

    c.style_note(240, 1010, 1700, 300,
                 "ĐIỀU KIỆN VÀ KẾT QUẢ\n"
                 "• Bước 2: hồ sơ phải có CCCD 12 số, đủ 18 tuổi, thời gian thử việc 1–60 ngày;\n"
                 "  mỗi nhân viên chỉ có một quyết định thử việc đang hiệu lực.\n"
                 "• Bước 3–4: trưởng phòng phân công ca làm việc và theo dõi, đánh giá trong thời gian thử việc.\n"
                 "• Bước 5–6: quyết định chức vụ và quyết định lương có hiệu lực kể từ ngày ký hợp đồng chính thức.\n"
                 "• Bước 7: hợp đồng chỉ được ký khi đã hết thử việc; có 2 loại Dài hạn / Ngắn hạn.\n"
                 "• Bước 8 (extend): chỉ hợp đồng dài hạn mới bắt buộc lập BHXH (8% NV/17,5% CT) và BHYT (1,5% NV/3% CT).\n"
                 "• Bước 9: cập nhật NhanVien.TrangThai = Đang làm và QuyetDinhTuyenDung.TrangThai = Hoàn thành.")
    c.save()


# =====================================================================
# 5. ACTIVITY DIAGRAM
# =====================================================================
def activity_recruitment():
    c = Canvas(1800, 1520, "5_Activity_TuyenDung",
               "ACTIVITY DIAGRAM: QUY TRÌNH THỬ VIỆC ĐẾN HỢP ĐỒNG CHÍNH THỨC")
    c.text(30, 16, c.title, 27, True, C_BLUE)
    c.text(30, 56, "Câu 5 - Swimlane theo vai trò thực hiện", 18, False, C_GRAY)

    lanes = [
        (40, 380, "Ứng viên / Nhân viên", C_BLUE_FILL),
        (420, 420, "Phòng HCNS", C_GOLD_FILL),
        (840, 400, "Trưởng phòng ban", C_GREEN_FILL),
        (1240, 520, "Giám đốc", C_PURPLE_FILL),
    ]
    for x, w, t, f in lanes:
        c.swimlane(x, 90, w, 1390, t, fill=f, stroke=C_GRAY, size=18)

    def start(cx, cy, r=16):
        c.d.ellipse([(cx - r) * S, (cy - r) * S, (cx + r) * S, (cy + r) * S],
                    fill=C_GREEN, outline=C_GREEN, width=2 * S)

    def end(cx, cy, r=18):
        c.d.ellipse([(cx - r) * S, (cy - r) * S, (cx + r) * S, (cy + r) * S],
                    fill=C_RED, outline=C_RED, width=2 * S)
        c.d.ellipse([(cx - r + 4) * S, (cy - r + 4) * S, (cx + r - 4) * S, (cy + r - 4) * S],
                    outline="white", width=2 * S)

    start(220, 160)
    c.text(220, 180, "Bắt đầu", 15, False, C_GRAY, "mt")

    A1 = (100, 210, 240, 62)
    A2 = (480, 210, 300, 62)
    D1 = (540, 315, 180, 96)
    A3 = (100, 330, 240, 74)
    A4 = (480, 470, 300, 62)
    A5 = (1300, 470, 300, 62)
    A6 = (100, 580, 240, 62)
    A7 = (900, 580, 300, 62)
    D2 = (930, 700, 200, 96)
    A8 = (480, 760, 300, 62)
    A9 = (480, 900, 300, 74)
    A10 = (1300, 900, 300, 62)
    D3 = (1340, 1010, 200, 96)
    A11 = (480, 1060, 300, 62)
    A12 = (480, 1210, 300, 62)

    def act(box, text):
        x, y, w, h = box
        return c.rect(x, y, w, h, text, fill=C_GREEN_FILL, stroke=C_GREEN,
                      rounded=True, size=18)

    act(A1, "Nộp hồ sơ ứng viên")
    act(A2, "Tiếp nhận, kiểm tra hồ sơ")
    c.diamond(*D1, "Hồ sơ hợp lệ?")
    act(A3, "Thông báo bổ sung\nhồ sơ")
    act(A4, "Lập QĐ tuyển dụng\nvà thử việc")
    act(A5, "Ký QĐ tuyển dụng")
    act(A6, "Bắt đầu thử việc")
    act(A7, "Phân công ca, theo dõi\nthử việc")
    c.diamond(*D2, "Đạt yêu cầu\nthử việc?")
    act(A8, "Chấm dứt thử việc")
    act(A9, "Lập QĐ chức vụ\nvà QĐ lương")
    act(A10, "Ký hợp đồng\nchính thức")
    c.diamond(*D3, "Loại hợp đồng?")
    act(A11, "Lập hồ sơ\nBHXH - BHYT")
    act(A12, "Cập nhật trạng thái\nĐang làm, lưu hồ sơ")
    end(630, 1330)

    def arw(pts, label="", wps=None, color=C_BLACK, dashed=False):
        c.line(pts, label=label, waypoints=wps, arrow="end", color=color,
               lw=2, size=16, dashed=dashed)

    arw([(220, 176), (220, 210)])
    arw([(340, 241), (480, 241)])
    arw([(630, 272), (630, 315)])
    arw([(540, 363), (340, 363)], "Không")
    arw([(220, 330), (220, 272)], "Bổ sung")
    arw([(630, 411), (630, 470)], "Có")
    arw([(780, 501), (1300, 501)])
    arw([(1450, 532), (1450, 560), (340, 560), (340, 580)])
    arw([(340, 611), (900, 611)])
    arw([(1050, 642), (1050, 700)])
    arw([(930, 748), (780, 748)], "Không")
    arw([(1030, 796), (1030, 840), (630, 840), (630, 900)], "Có")
    arw([(780, 937), (1300, 937)])
    arw([(1450, 962), (1450, 1010)])
    arw([(1340, 1058), (780, 1058)], "Dài hạn")
    arw([(1440, 1106), (1440, 1240), (780, 1240)], "Ngắn hạn")
    arw([(630, 1122), (630, 1210)])
    arw([(630, 1272), (630, 1312)])
    c.text(630, 1352, "Kết thúc", 15, False, C_GRAY, "mt")

    c.style_note(1060, 1180, 660, 180,
                 "Điều kiện:\n"
                 "• Hồ sơ hợp lệ: CCCD 12 số, đủ 18 tuổi,\n"
                 "  thời gian thử việc 1–60 ngày.\n"
                 "• Đạt thử việc: có đánh giá của trưởng phòng.\n"
                 "• Dài hạn: bắt buộc BHXH/BHYT; ngắn hạn: không.\n"
                 "• Mọi bước ghi đều lưu vết quyết định.")
    c.save()


# =====================================================================
# 6. SEQUENCE DIAGRAM - LAP QUYET DINH KHEN THUONG
# =====================================================================
def sequence_reward():
    c = Canvas(2200, 1560, "6_Sequence_KhenThuong",
               "SEQUENCE DIAGRAM: LẬP QUYẾT ĐỊNH KHEN THƯỞNG")
    c.text(30, 16, c.title, 29, True, C_BLUE)
    c.text(30, 58, "Câu 6 - Đối tượng: cá nhân hoặc tập thể phòng ban, đơn vị", 18, False, C_GRAY)

    parts = [
        ("HCNS", 160, "actor"),
        ("FrmKhenThuong\n<<UI>>", 500, "ui"),
        ("KhenThuongService\n<<Service>>", 860, "svc"),
        ("QuyetDinhKhenThuong\n<<Entity>>", 1220, "ent"),
        ("PhieuChiKhenThuong\n<<Entity>>", 1580, "ent2"),
        ("SQL Server\n<<Data>>", 1940, "db"),
    ]
    top = 110
    bottom = 1450
    for name, x, kind in parts:
        if kind == "actor":
            c.actor(x, 120, "Nhân viên\nHCNS", 20)
            c.line([(x, 250), (x, bottom)], arrow="none", dashed=True, color=C_GRAY, lw=1.5)
        else:
            c.rect(x - 150, top, 300, 64, name, fill=C_BLUE_FILL if kind in ("ui", "svc") else C_GOLD_FILL,
                   stroke=C_BLUE if kind in ("ui", "svc") else C_GOLD, size=16, bold=True)
            c.line([(x, 174), (x, bottom)], arrow="none", dashed=True, color=C_GRAY, lw=1.5)

    # activation bars
    for x, y1, y2 in ((860, 330, 1010), (1220, 410, 560), (1580, 690, 830), (1940, 500, 850)):
        c.rect(x - 9, y1, 18, y2 - y1, "", fill="#DDEBF7", stroke=C_BLUE, lw=1)

    def msg(y, a, b, text, dashed=False, color=C_BLACK):
        c.line([(a, y), (b, y)], text, dashed=dashed, arrow="open" if dashed else "end",
               color=color, lw=2, size=15, label_pos=0.5)

    msg(300, 160, 500, "1. Mở form khen thưởng")
    msg(360, 500, 860, "2. LapKhenThuong(input)")
    # self message
    c.line([(860, 400), (930, 400), (930, 450), (869, 450)], "3. ValidateKhenThuong()",
           arrow="end", color=C_BLACK, lw=2, size=15, label_pos=0.55)
    msg(500, 860, 1220, "4. KiemTraDoiTuong(doiTuong, MaNV, MaPB)")
    msg(545, 1220, 860, "5. hợp lệ / lỗi", dashed=True, color=C_BLUE)
    msg(610, 860, 1940, "6. INSERT QuyetDinhKhenThuong")
    msg(655, 1940, 860, "7. MaQDKhenThuong", dashed=True, color=C_BLUE)

    # alt frame
    c.d.rectangle([1300 * S, 690 * S, 2160 * S, 860 * S], outline=C_PURPLE, width=2 * S)
    c.d.polygon([1300 * S, 690 * S, 1440 * S, 690 * S, 1440 * S, 718 * S,
                 1420 * S, 728 * S, 1300 * S, 728 * S], fill=C_PURPLE_FILL, outline=C_PURPLE)
    c.text(1305, 696, "alt [Nguồn chi = PhieuChi]", 14, True, C_PURPLE)
    msg(760, 860, 1580, "8. INSERT PhieuChiKhenThuong")
    msg(805, 1580, 860, "9. MaPhieuChi", dashed=True, color=C_BLUE)

    msg(920, 860, 500, "10. ServiceResult", dashed=True, color=C_BLUE)
    msg(985, 500, 160, "11. Thông báo + làm mới lưới", dashed=True, color=C_BLUE)

    c.style_note(60, 1060, 700, 270,
                 "ALT / ĐIỀU KIỆN\n"
                 "• DoiTuong = CaNhan: bắt buộc MaNV, MaPB = NULL.\n"
                 "• DoiTuong = TapThe: bắt buộc MaPB, MaNV = NULL;\n"
                 "  số tiền được chia đều cho nhân viên đang làm của phòng ban.\n"
                 "• NguonChiTra = PhieuChi: sinh phiếu chi ngay (nhánh alt).\n"
                 "• NguonChiTra = Luong: không sinh phiếu chi; khoản thưởng\n"
                 "  được cộng vào bảng lương tháng tương ứng.")
    c.save()


# =====================================================================
# 7. COMPONENT / KIEN TRUC
# =====================================================================
def component_arch():
    c = Canvas(1900, 1240, "7_Component_KienTruc",
               "SƠ ĐỒ THÀNH PHẦN - KIẾN TRÚC UI → SERVICE → DATA")
    c.text(30, 16, c.title, 29, True, C_BLUE)
    c.text(30, 58, "Ứng dụng WinForms .NET Framework 4.7.2 | SQL Server", 18, False, C_GRAY)

    def band(y, h, title, fill, stroke):
        c.d.rectangle([40 * S, y * S, 1860 * S, (y + h) * S], fill=fill,
                      outline=stroke, width=2 * S)
        c.text(55, y + 10, title, 19, True, stroke)

    band(100, 210, "PRESENTATION LAYER (WinForms)", "#EAF1FB", C_BLUE)
    band(360, 190, "SERVICE LAYER (nghiệp vụ + kiểm tra dữ liệu)", "#FFF8E5", C_GOLD)
    band(600, 160, "DATA LAYER (ADO.NET)", "#EAF5E9", C_GREEN)
    band(810, 180, "DATABASE (SQL Server - QuanLyNhanSu)", "#F3EAF9", C_PURPLE)

    def comp(x, y, w, text, fill=C_WHITE, stroke=C_BLUE, size=15, h=46):
        c.rect(x, y, w, h, text, fill=fill, stroke=stroke, size=size)

    ui = ["FrmMain", "FrmTiepNhan", "FrmHopDong", "FrmQuyetDinh", "FrmKeHoach",
          "FrmChamCong", "FrmPhuTroi", "FrmKhenThuong", "FrmLuong", "FrmDanhSach"]
    for i, t in enumerate(ui):
        comp(70 + (i % 5) * 350, 145 + (i // 5) * 78, 310, t, C_WHITE, C_BLUE)

    svc = ["TuyenDungService", "HopDongService", "QuyetDinhService", "KeHoachService",
           "ChamCongService", "PhuTroiService", "KhenThuongService", "LuongService"]
    for i, t in enumerate(svc):
        comp(70 + (i % 4) * 440, 400 + (i // 4) * 70, 380, t, C_WHITE, C_GOLD)

    comp(70, 650, 520, "Db (Connection, Query, Execute, Transaction)", C_WHITE, C_GREEN)
    comp(650, 650, 420, "Models / DTO (Domain.cs)", C_WHITE, C_GREEN)
    comp(1130, 650, 380, "ServiceResult", C_WHITE, C_GREEN)

    comp(70, 870, 820, "18 bảng: NhanVien, HopDong, BangLuong, ...", C_WHITE, C_PURPLE)
    comp(950, 870, 380, "3 stored procedure", C_WHITE, C_PURPLE)
    comp(1390, 870, 400, "2 trigger bảo vệ nghiệp vụ", C_WHITE, C_PURPLE)

    # arrows
    c.line([(950, 310), (950, 400)], "gọi phương thức", arrow="end", color=C_BLUE,
           lw=2, size=16)
    c.line([(950, 540), (950, 650)], "mở kết nối / truy vấn", arrow="end", color=C_GOLD,
           lw=2, size=16)
    c.line([(950, 760), (950, 870)], "ADO.NET / T-SQL", arrow="end", color=C_GREEN,
           lw=2, size=16)

    c.style_note(70, 1070, 1760, 110,
                 "Quy ước: UI chỉ hiển thị và thu thập dữ liệu; Service chứa toàn bộ quy tắc nghiệp vụ và kiểm tra;\n"
                 "Data chỉ thực thi SQL có tham số. Không có câu SQL hoặc công thức tính lương trong form.")
    c.save()


# =====================================================================
# 8. ERD
# =====================================================================
ERD_TABLES = {
    "PB": ("PhongBan", 60, 110, 760, ["MaPB (PK)", "TenPB", "DienThoai", "TrangThai"]),
    "NV": ("NhanVien", 60, 480, 760, ["MaNV (PK)", "MaNVCode", "HoTen", "CCCD", "MaPB (FK)", "TrangThai"]),
    "QDTD": ("QuyetDinhTuyenDung", 60, 900, 760, ["MaQDTD (PK)", "SoQD", "MaNV (FK)", "MaPB (FK)", "NgayBatDau", "NgayKetThuc"]),
    "HD": ("HopDongLaoDong", 60, 1320, 760, ["MaHD (PK)", "SoHD", "MaNV (FK)", "MaQDTD (FK)", "LoaiHD", "NgayHieuLuc"]),
    "BH": ("BaoHiem", 60, 1740, 760, ["MaBH (PK)", "MaNV (FK)", "MaHD (FK)", "LoaiBH", "MucDong", "TyLeNhanVien"]),
    "QDCV": ("QuyetDinhChucVu", 60, 2160, 760, ["MaQDChucVu (PK)", "MaNV (FK)", "MaPB (FK)", "ChucVu", "PhuCapChucVu"]),

    "QDL": ("QuyetDinhLuong", 910, 110, 760, ["MaQDLuong (PK)", "MaNV (FK)", "MucLuongCoBan", "PhuCapLuong", "PhuCapDocHai"]),
    "KHT": ("KeHoachTuan", 910, 480, 760, ["MaKH (PK)", "MaPB (FK)", "TuanSo", "Nam", "TrangThai"]),
    "CTKH": ("ChiTietKeHoach", 910, 850, 760, ["MaCTKH (PK)", "MaKH (FK)", "MaNV (FK)", "Ngay", "CaLam"]),
    "CC": ("ChamCong", 910, 1220, 760, ["MaChamCong (PK)", "MaNV (FK)", "MaCTKH (FK)", "Ngay", "TrangThai"]),
    "PPT": ("PhieuPhuTroi", 910, 1590, 760, ["MaPhieuPT (PK)", "SoPhieu", "NgayPhieu", "HinhThuc", "TrangThai"]),
    "CTPT": ("ChiTietPhuTroi", 910, 1930, 760, ["MaCTPT (PK)", "MaPhieuPT (FK)", "MaNV (FK)", "SoGio", "HeSo"]),

    "KT": ("QuyetDinhKhenThuong", 1760, 110, 760, ["MaQDKhenThuong (PK)", "SoQD", "MaNV (FK)", "MaPB (FK)", "SoTien"]),
    "PC": ("PhieuChiKhenThuong", 1760, 480, 760, ["MaPhieuChi (PK)", "SoPhieu", "MaQDKhenThuong (FK)", "NgayChi", "SoTien"]),
    "KL": ("QuyetDinhKyLuat", 1760, 850, 760, ["MaQDKyLuat (PK)", "SoQD", "MaNV (FK)", "MaPB (FK)", "SoTienPhat"]),
    "PT": ("PhieuThuNopPhat", 1760, 1220, 760, ["MaPhieuThu (PK)", "SoPhieu", "MaQDKyLuat (FK)", "NgayThu", "SoTien"]),
    "BL": ("BangLuong", 1760, 1590, 760, ["MaBangLuong (PK)", "Thang", "Nam", "TrangThai", "TongChiPhi"]),
    "CTBL": ("ChiTietBangLuong", 1760, 1930, 760, ["MaCTBL (PK)", "MaBangLuong (FK)", "MaNV (FK)", "SoNgayCong", "ThucLinh"]),
}


def erd():
    c = Canvas(2600, 2640, "8_ERD",
               "SƠ ĐỒ ERD - CSDL QUANLYNHANSU (18 BẢNG)")
    c.text(30, 16, c.title, 30, True, C_BLUE)
    c.text(30, 60, "Khóa chính (PK) màu đỏ, khóa ngoại (FK) màu tím; quan hệ 1 - N", 18, False, C_GRAY)

    ids = {}
    for k, (t, x, y, w, cols) in ERD_TABLES.items():
        vid, h = c.tablebox(x, y, w, t, cols, size=20, header=C_BLUE_FILL, stroke=C_BLUE)
        ids[k] = vid

    def rel(a, b, pa, pb, m1, m2, label="", wps=None):
        c.line([pa, pb], label=label, arrow="none", source=ids[a], target=ids[b],
               start_mult=m1 if m1 != "1" else "", end_mult=m2, waypoints=wps,
               color=C_BLUE, lw=2, size=16)

    rel("PB", "NV", (360, 320), (360, 480), "1", "N", "quản lý")
    rel("NV", "QDTD", (360, 700), (360, 900), "1", "N", "được tuyển")
    rel("QDTD", "HD", (360, 1100), (360, 1320), "1", "0..1", "cơ sở")
    rel("HD", "BH", (360, 1500), (360, 1740), "1", "N", "đóng")
    rel("NV", "QDCV", (820, 600), (820, 2280), "1", "N", "",
        wps=[(860, 600), (860, 2280)])
    rel("PB", "QDCV", (820, 250), (820, 2200), "1", "N", "",
        wps=[(890, 250), (890, 2200)])

    rel("NV", "QDL", (820, 530), (910, 300), "1", "N", "")
    rel("NV", "KHT", (820, 580), (910, 560), "1", "N", "")
    rel("NV", "CTKH", (820, 640), (910, 890), "1", "N", "",
        wps=[(880, 640), (880, 890)])
    rel("NV", "CC", (820, 680), (910, 1260), "1", "N", "",
        wps=[(895, 680), (895, 1260)])
    rel("NV", "CTPT", (820, 720), (910, 1970), "1", "N", "",
        wps=[(905, 720), (905, 1970)])

    rel("KHT", "CTKH", (1290, 750), (1290, 850), "1", "N", "gồm")
    rel("CTKH", "CC", (1290, 1090), (1290, 1220), "1", "0..1", "")
    rel("PPT", "CTPT", (1290, 1840), (1290, 1930), "1", "N", "gồm")

    rel("KT", "PC", (2140, 420), (2140, 480), "1", "0..1", "chi")
    rel("KL", "PT", (2140, 1160), (2140, 1220), "1", "0..1", "thu")
    rel("BL", "CTBL", (2140, 1840), (2140, 1930), "1", "N", "gồm")

    rel("PB", "KT", (340, 110), (2140, 110), "1", "N", "khen thưởng",
        wps=[(340, 80), (2140, 80)])
    rel("PB", "KL", (390, 110), (2520, 1000), "1", "N", "kỷ luật",
        wps=[(390, 96), (2560, 96), (2560, 1000)])

    c.style_note(30, 2490, 2540, 120,
                 "GHI CHÚ: 18 bảng, 3 stored procedure (sp_TiepNhanThuViec, sp_KyHopDongChinhThuc, sp_ChotBangLuong)\n"
                 "và 2 trigger (TR_BaoHiem_ChiDaiHan, TR_ChiTietBangLuong_Khoa). Quan hệ nhiều-nhiều không phát sinh\n"
                 "vì mỗi bảng chi tiết đã có khóa ngoại trực tiếp; các bảng con đều xóa theo cha (ON DELETE CASCADE)\n"
                 "ở ChiTietKeHoach, ChiTietPhuTroi, ChiTietBangLuong.")
    c.save()


if __name__ == "__main__":
    usecase_recruitment()
    activity_recruitment()
    sequence_reward()
    component_arch()
    erd()
