# -*- coding: utf-8 -*-
import io
p = 'gen_diagrams.py'
s = open(p, encoding='utf-8').read()
start = s.index('def usecase_overview():')
end = s.index('if __name__ == "__main__":')
new = r'''def usecase_overview():
    c = Canvas(2400, 1460, "3_UseCase_TongQuat",
               "SƠ ĐỒ USE CASE TỔNG QUÁT - HỆ THỐNG QUẢN LÝ NHÂN SỰ")
    c.text(30, 68, "Công ty Sơn Đông Á  |  Phân rã chi tiết quy trình tuyển dụng tại Câu 4", 18, False, C_GRAY)

    # system boundary
    c.d.rectangle([470 * S, 110 * S, 1960 * S, 1130 * S], outline=C_BLUE, width=2 * S)
    c.text(1215, 122, "HỆ THỐNG QUẢN LÝ NHÂN SỰ", 22, True, C_BLUE, "mt")

    # actors
    a_nv = c.actor(220, 290, "Nhân viên /\nỨng viên")
    a_tp = c.actor(220, 630, "Trưởng phòng,\nban, phân xưởng")
    a_cc = c.actor(220, 970, "Ban chấm công")

    a_hc = c.actor(2180, 330, "Phòng HCNS")
    a_gd = c.actor(2180, 680, "Giám đốc")
    a_bc = c.actor(2180, 1020, "Bộ phận\nkế toán - lương")

    # use cases: 3 columns
    x1, x2, x3 = 560, 1030, 1500
    w, h = 380, 86
    uc = {}

    def U(key, x, y, text, fill=C_GOLD_FILL):
        uc[key] = c.ellipse(x, y, w, h, text, fill=fill)
        return uc[key]

    U("tiepnhan", x1, 230, "Tiếp nhận hồ sơ\nthử việc")
    U("qdtv", x1, 340, "Lập quyết định\ntuyển dụng")
    U("phanCong", x1, 450, "Phân công thử việc\ntại phòng ban")
    U("danhGia", x1, 560, "Theo dõi, đánh giá\nkết quả thử việc")
    U("kyhd", x1, 670, "Ký hợp đồng\nlao động chính thức")
    U("baohiem", x1, 780, "Lập hồ sơ\nBHXH - BHYT")

    U("qdcv", x2, 230, "Lập quyết định\nchức vụ - phụ cấp")
    U("qdl", x2, 340, "Lập quyết định\nlương - phụ cấp")
    U("kh", x2, 450, "Lập kế hoạch\nlàm việc tuần")
    U("cc", x2, 560, "Chấm công\ntheo kế hoạch")
    U("pt", x2, 670, "Lập phiếu ghi nhận\nphụ trội")
    U("duyetpt", x2, 780, "Duyệt phiếu\nphụ trội")

    U("kt", x3, 230, "Lập quyết định\nkhen thưởng")
    U("pc", x3, 340, "Lập phiếu chi\nkhen thưởng")
    U("kl", x3, 450, "Lập quyết định\nkỷ luật")
    U("ptthu", x3, 560, "Lập phiếu thu\nnộp phạt")
    U("bl", x3, 670, "Tính và chốt\nbảng lương tháng")
    U("bc", x3, 780, "Báo cáo nhân sự,\nquỹ lương")

    def assoc(ax, ay, ex, ey, dashed=False):
        c.line([(ax, ay), (ex, ey)], arrow="none", dashed=dashed, color=C_BLUE, lw=2)

    # Nhan vien
    assoc(300, 332, 560, 273)
    assoc(300, 332, 560, 493)
    # Truong phong
    assoc(300, 672, 560, 603)
    assoc(300, 672, 1030, 493)
    assoc(300, 672, 1500, 273)
    assoc(300, 672, 1500, 493)
    # Ban cham cong
    assoc(300, 1012, 1030, 603)
    assoc(300, 1012, 1030, 713)
    assoc(300, 1012, 1030, 823)
    # HCNS
    assoc(2110, 372, 560, 273)
    assoc(2110, 372, 560, 383)
    assoc(2110, 372, 560, 713)
    assoc(2110, 372, 560, 823)
    assoc(2110, 372, 1030, 273)
    assoc(2110, 372, 1030, 383)
    assoc(2110, 372, 1500, 273)
    assoc(2110, 372, 1500, 340)
    assoc(2110, 372, 1500, 450)
    assoc(2110, 372, 1500, 560)
    # Giam doc
    assoc(2110, 722, 560, 383)
    assoc(2110, 722, 560, 713)
    assoc(2110, 722, 1030, 493)
    assoc(2110, 722, 1500, 273)
    assoc(2110, 722, 1500, 450)
    # Ke toan
    assoc(2110, 1062, 1500, 713)
    assoc(2110, 1062, 1500, 823)
    assoc(2110, 1062, 560, 823)

    # include / extend (cuc bo, tranh cat duong)
    c.line([(750, 316), (750, 340)], "«include»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(940, 756), (940, 780)], "«extend»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(1320, 700), (1320, 780)], "«include»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(1880, 316), (1880, 340)], "«extend»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(1880, 536), (1880, 560)], "«extend»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)

    # legend
    c.d.rectangle([20 * S, 1190 * S, 2360 * S, 1380 * S], outline=C_GOLD, width=2 * S, fill="#FFFFF8")
    c.text(40, 1205, "CHÚ THÍCH", 17, True, C_GOLD)
    c.text(40, 1235,
           "• Người que = Actor; hình elip = Use case; đường liền = quan hệ actor - use case.\n"
           "• «include»: use case đích là bắt buộc (ví dụ tính bảng lương phải dùng chấm công).\n"
           "• «extend»: use case đích chỉ xảy ra trong điều kiện nhất định (ví dụ chỉ hợp đồng dài hạn mới lập BHXH/BHYT;\n"
           "  quyết định khen thưởng chỉ sinh phiếu chi khi nguồn chi là PhiếuChi).\n"
           "• Hệ thống quản lý toàn bộ thực thể: nhân viên, hợp đồng, bảo hiểm, quyết định lương/chức vụ, kế hoạch tuần,\n"
           "  chấm công, phụ trội, khen thưởng, kỷ luật, phiếu chi/thu và bảng lương tháng.",
           15, False, C_GRAY, "lt")
    c.save()


'''
s = s[:start] + new + s[end:]
open(p, 'w', encoding='utf-8').write(s)
print('patched')
