using System.Text.RegularExpressions;

namespace QuanLyCaoOc.Services
{
    /// <summary>Ham kiem tra dinh dang dung chung.</summary>
    public static class KiemTra
    {
        public static bool LaChuoiSo(string s, int min, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            return s.Length >= min && s.Length <= max && Regex.IsMatch(s, "^[0-9]+$");
        }

        /// <summary>Bien so VN: 51A-123.45, 59X1-234.56, 51G-678.90 ...</summary>
        public static bool LaBienSo(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return Regex.IsMatch(s.Trim().ToUpperInvariant(), @"^[0-9]{2}[A-Z]{1,2}[0-9]?-[0-9]{3}\.[0-9]{2}$");
        }
    }
}
