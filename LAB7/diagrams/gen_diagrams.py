# -*- coding: utf-8 -*-
"""
Sinh so do LAB7: xuat dong thoi file .drawio (nguon) va .png (de chen bao cao).
Khong phu thuoc draw.io CLI; PNG duoc ve bang Pillow theo cung toa do.
"""
import os
import math
import html
from PIL import Image, ImageDraw, ImageFont

BASE = os.path.dirname(os.path.abspath(__file__))
S = 2  # supersampling

FONT_REG = r"C:\Windows\Fonts\arial.ttf"
FONT_BOLD = r"C:\Windows\Fonts\arialbd.ttf"

C_BLUE = "#2F5597"
C_BLUE_FILL = "#DCE9F7"
C_GOLD = "#BF9000"
C_GOLD_FILL = "#FFF2CC"
C_GREEN = "#548235"
C_GREEN_FILL = "#E2EFDA"
C_GRAY = "#595959"
C_GRAY_FILL = "#F2F2F2"
C_RED = "#C00000"
C_RED_FILL = "#FBE4E4"
C_ORANGE_FILL = "#FCE4D6"
C_PURPLE = "#7030A0"
C_PURPLE_FILL = "#E9DFF5"
C_WHITE = "#FFFFFF"
C_BLACK = "#000000"


def esc(s):
    return html.escape(s, quote=True).replace("\n", "<br>")


class Canvas:
    def __init__(self, w, h, name, title=""):
        self.w, self.h, self.name, self.title = w, h, name, title
        self.img = Image.new("RGB", (w * S, h * S), "white")
        self.d = ImageDraw.Draw(self.img)
        self.cells = []
        self.n = 0

    # ---------- helpers ----------
    def _id(self, p="n"):
        self.n += 1
        return "%s%d" % (p, self.n)

    def _font(self, size, bold=False):
        return ImageFont.truetype(FONT_BOLD if bold else FONT_REG, int(size * S))

    def _wrap(self, text, font, maxw):
        out = []
        for raw in str(text).split("\n"):
            if raw == "":
                out.append("")
                continue
            words = raw.split(" ")
            line = ""
            for wd in words:
                cand = (line + " " + wd).strip()
                if font.getlength(cand) <= maxw * S or not line:
                    line = cand
                else:
                    out.append(line)
                    line = wd
            out.append(line)
        return out

    def text(self, x, y, s, size=20, bold=False, color=C_BLACK, anchor="lt",
             maxw=None, line_gap=1.18):
        f = self._font(size, bold)
        lines = self._wrap(s, f, maxw) if maxw else str(s).split("\n")
        lh = size * line_gap
        total = lh * len(lines)
        yy = y
        if anchor[1] == "m":
            yy = y - total / 2
        elif anchor[1] == "b":
            yy = y - total
        for ln in lines:
            xx = x
            if anchor[0] == "m":
                xx = x - f.getlength(ln) / (2 * S)
            elif anchor[0] == "r":
                xx = x - f.getlength(ln) / S
            self.d.text((xx * S, yy * S), ln, font=f, fill=color)
            yy += lh
        return total

    def _html(self, s):
        return esc(s).replace("&lt;br&gt;", "<br>")

    def vertex(self, vid, x, y, w, h, value, style, parent="1"):
        raw = value.strip().startswith("<")
        v = value.replace('"', '&quot;') if raw else html.escape(value, quote=True)
        self.cells.append(
            '<mxCell id="%s" value="%s" style="%s" vertex="1" parent="%s">'
            '<mxGeometry x="%.1f" y="%.1f" width="%.1f" height="%.1f" as="geometry"/>'
            "</mxCell>" % (vid, v, style, parent, x, y, w, h)
        )

    def rect(self, x, y, w, h, text="", fill=C_WHITE, stroke=C_BLUE, size=20,
             bold=False, rounded=False, dashed=False, color=C_BLACK, lw=2,
             align="center", valign="middle", padding=6, style_extra=""):
        dash = "dashed=1;" if dashed else ""
        self.d.rounded_rectangle([x * S, y * S, (x + w) * S, (y + h) * S],
                                 radius=(10 * S if rounded else 0), fill=fill,
                                 outline=stroke, width=max(1, int(lw * S)))
        maxw = w - 2 * padding
        if text:
            f = self._font(size, bold)
            lines = self._wrap(text, f, maxw)
            th = len(lines) * size * 1.18
            ty = y + (h - th) / 2 if valign == "middle" else y + padding
            for i, ln in enumerate(lines):
                if align == "center":
                    tx = x + w / 2
                    an = "mt"
                elif align == "right":
                    tx = x + w - padding
                    an = "rt"
                else:
                    tx = x + padding
                    an = "lt"
                self.text(tx, ty + i * size * 1.18, ln, size, bold, color, an)
        vid = self._id()
        st = "rounded=%d;whiteSpace=wrap;html=1;fillColor=%s;strokeColor=%s;fontFamily=Arial;fontSize=%d;%s%s" % (
            1 if rounded else 0, fill, stroke, size, dash, style_extra)
        self.vertex(vid, x, y, w, h, text, st)
        return vid

    def ellipse(self, x, y, w, h, text, fill=C_GOLD_FILL, stroke=C_GOLD, size=19,
                bold=False, dashed=False):
        dash = "dashed=1;" if dashed else ""
        self.d.ellipse([x * S, y * S, (x + w) * S, (y + h) * S], fill=fill,
                       outline=stroke, width=2 * S)
        f = self._font(size, bold)
        lines = self._wrap(text, f, w - 24)
        th = len(lines) * size * 1.16
        ty = y + (h - th) / 2
        for i, ln in enumerate(lines):
            self.text(x + w / 2, ty + i * size * 1.16, ln, size, bold, C_BLACK, "mt")
        vid = self._id("uc")
        st = "ellipse;whiteSpace=wrap;html=1;fillColor=%s;strokeColor=%s;fontFamily=Arial;fontSize=%d;%s" % (
            fill, stroke, size, dash)
        self.vertex(vid, x, y, w, h, text, st)
        return vid

    def diamond(self, x, y, w, h, text, fill=C_GREEN_FILL, stroke=C_GREEN, size=18):
        pts = [(x + w / 2, y), (x + w, y + h / 2), (x + w / 2, y + h), (x, y + h / 2)]
        self.d.polygon([(px * S, py * S) for px, py in pts], fill=fill,
                       outline=stroke, width=2 * S)
        f = self._font(size)
        lines = self._wrap(text, f, w - 20)
        th = len(lines) * size * 1.16
        ty = y + (h - th) / 2
        for i, ln in enumerate(lines):
            self.text(x + w / 2, ty + i * size * 1.16, ln, size, False, C_BLACK, "mt")
        vid = self._id("dec")
        st = "rhombus;whiteSpace=wrap;html=1;fillColor=%s;strokeColor=%s;fontFamily=Arial;fontSize=%d;" % (
            fill, stroke, size)
        self.vertex(vid, x, y, w, h, text, st)
        return vid

    def actor(self, cx, top, label, size=20, color=C_BLUE):
        # stick figure, height 90
        head_r = 13
        self.d.ellipse([(cx - head_r) * S, top * S, (cx + head_r) * S, (top + 2 * head_r) * S],
                       outline=color, width=2 * S)
        self.d.line([cx * S, (top + 26) * S, cx * S, (top + 62) * S], fill=color, width=2 * S)
        self.d.line([(cx - 20) * S, (top + 40) * S, (cx + 20) * S, (top + 40) * S], fill=color, width=2 * S)
        self.d.line([(cx - 17) * S, (top + 84) * S, cx * S, (top + 62) * S, (cx + 17) * S, (top + 84) * S],
                    fill=color, width=2 * S)
        self.text(cx, top + 92, label, size, True, C_BLACK, "mt", maxw=180)
        vid = self._id("act")
        self.vertex(vid, cx - 30, top, 60, 92 + 30, label,
                    "shape=umlActor;whiteSpace=wrap;html=1;verticalLabelPosition=bottom;verticalAlign=top;fontFamily=Arial;fontSize=%d;" % size)
        return vid

    def line(self, pts, label="", dashed=False, arrow="end", color=C_BLACK, lw=2,
             size=16, label_pos=0.5, source=None, target=None, start_mult="",
             end_mult="", arrow_start=None, waypoints=None, bold=False):
        pts = [(float(a), float(b)) for a, b in pts]
        if waypoints:
            pts = [pts[0]] + [(float(a), float(b)) for a, b in waypoints] + [pts[-1]]
        for i in range(len(pts) - 1):
            p, q = pts[i], pts[i + 1]
            if dashed:
                # ve net dut
                self._dash_line(p, q, color, lw)
            else:
                self.d.line([p[0] * S, p[1] * S, q[0] * S, q[1] * S], fill=color, width=lw * S)
        if arrow in ("end", "both"):
            self._arrowhead(pts[-2], pts[-1], color, filled=True)
        if arrow == "open":
            self._arrowhead(pts[-2], pts[-1], color, filled=False)
        if arrow == "both" or arrow_start == "start":
            self._arrowhead(pts[1], pts[0], color, filled=True)
        if label:
            self._edge_label(pts, label, label_pos, size, color)
        if start_mult:
            self._near_end_label(pts, start_mult, True, size)
        if end_mult:
            self._near_end_label(pts, end_mult, False, size)
        eid = self._id("e")
        st = "endArrow=%s;html=1;rounded=0;strokeColor=%s;strokeWidth=%d;fontFamily=Arial;fontSize=%d;%s" % (
            {"end": "block", "both": "block", "open": "open", "none": "none"}[arrow],
            color, lw, size, "dashed=1;" if dashed else "")
        if arrow == "none":
            st = "endArrow=none;html=1;rounded=0;strokeColor=%s;strokeWidth=%d;%s" % (
                color, lw, "dashed=1;" if dashed else "")
        src = ' source="%s"' % source if source else ""
        tgt = ' target="%s"' % target if target else ""
        way = ""
        if waypoints:
            way = '<Array as="points">%s</Array>' % "".join(
                '<mxPoint x="%.1f" y="%.1f"/>' % (a, b) for a, b in waypoints)
        self.cells.append(
            '<mxCell id="%s" value="%s" style="%s" edge="1" parent="1"%s%s>'
            '<mxGeometry relative="1" as="geometry">%s</mxGeometry></mxCell>' % (
                eid, html.escape(label, quote=True), st, src, tgt, way))
        # multiplicity labels as edge children (draw.io)
        for txt, rel in ((start_mult, -0.82), (end_mult, 0.82)):
            if txt:
                lid = self._id("el")
                self.cells.append(
                    '<mxCell id="%s" value="%s" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontFamily=Arial;fontSize=%d;" vertex="1" connectable="0" parent="%s">'
                    '<mxGeometry x="%.2f" relative="1" as="geometry"><mxPoint as="offset"/></mxGeometry></mxCell>' % (
                        lid, html.escape(txt, quote=True), size, eid, rel))
        return eid

    def _dash_line(self, p, q, color, lw, dash=9, gap=7):
        dx, dy = q[0] - p[0], q[1] - p[1]
        L = math.hypot(dx, dy)
        if L == 0:
            return
        ux, uy = dx / L, dy / L
        t = 0
        while t < L:
            e = min(t + dash, L)
            self.d.line([(p[0] + ux * t) * S, (p[1] + uy * t) * S,
                         (p[0] + ux * e) * S, (p[1] + uy * e) * S], fill=color, width=lw * S)
            t = e + gap

    def _arrowhead(self, p, q, color, filled=True, size=12):
        ang = math.atan2(q[1] - p[1], q[0] - p[0])
        a = size
        pts = [(q[0], q[1]),
               (q[0] - a * math.cos(ang - 0.38), q[1] - a * math.sin(ang - 0.38)),
               (q[0] - a * math.cos(ang + 0.38), q[1] - a * math.sin(ang + 0.38))]
        self.d.polygon([(x * S, y * S) for x, y in pts],
                       fill=color if filled else "white", outline=color, width=2 * S)

    def _edge_label(self, pts, label, pos, size, color):
        # diem tren polyline theo do dai
        segs = []
        total = 0
        for i in range(len(pts) - 1):
            L = math.hypot(pts[i + 1][0] - pts[i][0], pts[i + 1][1] - pts[i][1])
            segs.append((total, total + L, pts[i], pts[i + 1]))
            total += L
        if total == 0:
            return
        t = total * pos
        for a, b, p, q in segs:
            if a <= t <= b:
                r = (t - a) / max(1e-6, b - a)
                x, y = p[0] + (q[0] - p[0]) * r, p[1] + (q[1] - p[1]) * r
                f = self._font(size)
                tw = f.getlength(label) / S + 8
                th = size + 6
                self.d.rectangle([(x - tw / 2) * S, (y - th / 2) * S,
                                  (x + tw / 2) * S, (y + th / 2) * S], fill="white")
                self.text(x, y, label, size, False, color, "mm")
                return

    def _near_end_label(self, pts, text, at_start, size):
        if at_start:
            p, q = pts[0], pts[1]
        else:
            p, q = pts[-1], pts[-2]
        L = math.hypot(q[0] - p[0], q[1] - p[1])
        if L == 0:
            return
        ux, uy = (q[0] - p[0]) / L, (q[1] - p[1]) / L
        x, y = p[0] + ux * 24, p[1] + uy * 24
        # offset vuong goc, hai dau lech hai phia de khong de len nhau
        side = -1 if at_start else 1
        ox, oy = -uy * 16 * side, ux * 16 * side
        f = self._font(size)
        tw = f.getlength(text) / S + 6
        self.d.rectangle([(x + ox - tw / 2) * S, (y + oy - size / 2 - 2) * S,
                          (x + ox + tw / 2) * S, (y + oy + size / 2 + 2) * S], fill="white")
        self.text(x + ox, y + oy, text, size, False, C_BLACK, "mm")

    def classbox(self, x, y, w, title, attrs, methods, size=18, header=C_BLUE_FILL,
                 stroke=C_BLUE, key_lines=None, dashed=False):
        lh = size * 1.32
        head_h = size * 1.9
        h = head_h + len(attrs) * lh + 8 + len(methods) * lh + 8
        self.d.rectangle([x * S, y * S, (x + w) * S, (y + h) * S], fill=C_WHITE,
                         outline=stroke, width=2 * S)
        self.d.rectangle([x * S, y * S, (x + w) * S, (y + head_h) * S], fill=header,
                         outline=stroke, width=2 * S)
        self.text(x + w / 2, y + head_h / 2, title, size + 1, True, C_BLACK, "mm")
        yy = y + head_h + 3
        for a in attrs:
            col = C_RED if ("PK" in a or "FK" in a) else C_BLACK
            self.text(x + 7, yy, a, size, False, col, "lt")
            yy += lh
        self.d.line([x * S, yy * S, (x + w) * S, yy * S], fill=stroke, width=1 * S)
        yy += 4
        for m in methods:
            self.text(x + 7, yy, m, size, False, C_BLUE, "lt")
            yy += lh
        # drawio: HTML compartments
        parts = ['<div style="text-align:center"><b>%s</b></div><hr>' % esc(title)]
        parts.append("<div style='text-align:left'>%s</div>" % "<br>".join(esc(a) for a in attrs))
        parts.append("<hr><div style='text-align:left'>%s</div>" % "<br>".join(esc(m) for m in methods))
        val = "".join(parts)
        vid = self._id("cls")
        st = "rounded=0;whiteSpace=wrap;html=1;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;spacingTop=2;fillColor=#FFFFFF;strokeColor=%s;fontFamily=Arial;fontSize=%d;%s" % (
            stroke, size, "dashed=1;" if dashed else "")
        self.vertex(vid, x, y, w, h, val, st)
        return vid, h

    def tablebox(self, x, y, w, title, cols, size=16, header=C_BLUE_FILL,
                 stroke=C_BLUE):
        lh = size * 1.3
        head_h = size * 1.9
        h = head_h + len(cols) * lh + 6
        self.d.rectangle([x * S, y * S, (x + w) * S, (y + h) * S], fill=C_WHITE,
                         outline=stroke, width=2 * S)
        self.d.rectangle([x * S, y * S, (x + w) * S, (y + head_h) * S], fill=header,
                         outline=stroke, width=2 * S)
        self.text(x + w / 2, y + head_h / 2, title, size + 1, True, C_BLACK, "mm")
        yy = y + head_h + 2
        for c in cols:
            col = C_RED if ("PK" in c) else (C_PURPLE if "FK" in c else C_BLACK)
            self.text(x + 6, yy, c, size, False, col, "lt")
            yy += lh
        val = '<div style="text-align:center"><b>%s</b></div><hr><div style="text-align:left">%s</div>' % (
            esc(title), "<br>".join(esc(c) for c in cols))
        vid = self._id("tbl")
        st = "rounded=0;whiteSpace=wrap;html=1;align=left;verticalAlign=top;spacingLeft=4;fillColor=#FFFFFF;strokeColor=%s;fontFamily=Arial;fontSize=%d;" % (
            stroke, size)
        self.vertex(vid, x, y, w, h, val, st)
        return vid, h

    def swimlane(self, x, y, w, h, title, fill=C_GRAY_FILL, stroke=C_GRAY, size=20,
                 horizontal=False):
        self.d.rectangle([x * S, y * S, (x + w) * S, (y + h) * S], fill="white",
                         outline=stroke, width=2 * S)
        if horizontal:
            self.d.rectangle([x * S, y * S, (x + 34) * S, (y + h) * S], fill=fill,
                             outline=stroke, width=2 * S)
            self.text(x + 17, y + h / 2, title, size, True, C_BLACK, "mm")
        else:
            self.d.rectangle([x * S, y * S, (x + w) * S, (y + 34) * S], fill=fill,
                             outline=stroke, width=2 * S)
            self.text(x + w / 2, y + 17, title, size, True, C_BLACK, "mm")
        vid = self._id("lane")
        st = "swimlane;horizontal=%d;startSize=34;html=1;fillColor=%s;strokeColor=%s;fontFamily=Arial;fontSize=%d;" % (
            1 if horizontal else 0, fill, stroke, size)
        self.vertex(vid, x, y, w, h, title, st)
        return vid

    def style_note(self, x, y, w, h, text, size=15):
        self.d.rectangle([x * S, y * S, (x + w) * S, (y + h) * S], fill="#FFFFF0",
                         outline="#BF9000", width=1 * S)
        self.d.polygon([(x + w - 18) * S, y * S, (x + w) * S, y * S, (x + w) * S, (y + 18) * S],
                       fill="#FFF2CC", outline="#BF9000")
        self.text(x + 8, y + 8, text, size, False, C_GRAY, "lt", maxw=w - 16)
        vid = self._id("note")
        self.vertex(vid, x, y, w, h, text,
                    "shape=note;whiteSpace=wrap;html=1;fillColor=#FFFFF0;strokeColor=#BF9000;fontFamily=Arial;fontSize=%d;align=left;verticalAlign=top;spacingLeft=6;" % size)
        return vid

    def title_block(self, subtitle=""):
        if not self.title:
            return
        self.text(30, 24, self.title, 30, True, C_BLUE, "lt")
        if subtitle:
            self.text(30, 64, subtitle, 18, False, C_GRAY, "lt")

    def save(self):
        os.makedirs(BASE, exist_ok=True)
        png = os.path.join(BASE, self.name + ".png")
        self.img.resize((self.w, self.h), Image.LANCZOS).save(png, "PNG")
        drawio = os.path.join(BASE, self.name + ".drawio")
        body = "\n".join(self.cells)
        xml = ('<mxfile host="app.diagrams.net" agent="LAB7 generator" version="24.7.17">\n'
               '  <diagram id="%s" name="%s">\n'
               '    <mxGraphModel dx="1600" dy="1000" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="%d" pageHeight="%d" math="0" shadow="0">\n'
               '      <root>\n        <mxCell id="0"/>\n        <mxCell id="1" parent="0"/>\n%s\n      </root>\n'
               '    </mxGraphModel>\n  </diagram>\n</mxfile>\n') % (
            self.name, html.escape(self.title or self.name, quote=True), self.w, self.h, body)
        with open(drawio, "w", encoding="utf-8") as f:
            f.write(xml)
        print("Saved", png, "and", drawio)


# =====================================================================
# 3. USE CASE TONG QUAT
# =====================================================================
def usecase_overview():
    c = Canvas(2400, 1460, "3_UseCase_TongQuat",
               "SƠ ĐỒ USE CASE TỔNG QUÁT - HỆ THỐNG QUẢN LÝ NHÂN SỰ")
    c.text(30, 16, c.title, 30, True, C_BLUE)
    c.text(30, 60, "Công ty Sơn Đông Á  |  Phân rã chi tiết quy trình tuyển dụng tại Câu 4", 18, False, C_GRAY)

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
    c.line([(750, 756), (750, 780)], "«extend»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(1220, 700), (1220, 780)], "«include»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(1690, 316), (1690, 340)], "«extend»", dashed=True, arrow="open",
           color=C_PURPLE, lw=2, size=14)
    c.line([(1690, 536), (1690, 560)], "«extend»", dashed=True, arrow="open",
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
        # Chi hien ban so nguon khi khac 1 (mac dinh 1 da ghi trong chu thich)
        c.line([pa, pb], label=label, dashed=dashed, source=ids[a], target=ids[b],
               start_mult=(m1 if m1 not in ("1", "") else ""), end_mult=m2,
               waypoints=wps, **dict(A, color=color))

    # cot 1: chuoi doc
    E("PB", "NV", (360, 347), (360, 400), "1", "0..*", "quản lý")
    E("NV", "QDTD", (360, 705), (360, 800), "1", "0..*", "được tuyển")
    E("QDTD", "HD", (360, 1105), (360, 1200), "0..1", "0..1", "làm cơ sở")
    E("HD", "BH", (360, 1505), (360, 1600), "1", "0..*", "đóng")
    # QDCV: qua kenh phai cot 1
    E("NV", "QDCV", (820, 600), (820, 2100), "1", "0..*", "",
      wps=[(850, 600), (850, 2100)])
    E("PB", "QDCV", (820, 250), (820, 2050), "1", "0..*", "",
      wps=[(875, 250), (875, 2050)])

    # NV -> cot 2 (fan trong khe 820-910)
    E("NV", "QDL", (820, 470), (910, 300), "1", "0..*", "")
    E("NV", "KHT", (820, 540), (910, 560), "1", "0..*", "")
    E("NV", "CTKH", (820, 610), (910, 880), "1", "0..*", "",
      wps=[(870, 610), (870, 880)])
    E("NV", "CC", (820, 660), (910, 1250), "1", "0..*", "",
      wps=[(885, 660), (885, 1250)])
    E("NV", "PPT", (820, 690), (910, 1680), "1", "0..*", "",
      wps=[(895, 690), (895, 1680)])
    E("NV", "CTPT", (820, 730), (910, 2040), "1", "0..*", "",
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
      wps=[(390, 92), (2560, 92), (2560, 1100)])

    # (ChiTietBangLuong.MaNV -> NhanVien da ghi trong chu thich de tranh duong cheo dai)

    # -------- notes --------
    c.style_note(30, 2390, 1680, 210,
                 "CHÚ THÍCH\n"
                 "• Đầu nguồn không ghi bản số = 1; đầu đích ghi 0..*, 1..*, 0..1 tương ứng.\n"
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


if __name__ == "__main__":
    usecase_overview()
