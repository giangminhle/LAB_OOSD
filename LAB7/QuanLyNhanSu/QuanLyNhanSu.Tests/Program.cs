using System;
using System.Text;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.Tests
{
    /// <summary>
    /// Console runner cho toan bo kiem thu cua LAB7.
    /// Cach dung:
    ///   QuanLyNhanSu.Tests.exe            -> chay offline + CSDL
    ///   QuanLyNhanSu.Tests.exe offline    -> chi chay kiem thu quy tac
    ///   QuanLyNhanSu.Tests.exe db         -> chi chay kiem thu tich hop CSDL
    /// Ma thoat: 0 = tat ca PASS, 1 = co case FAIL.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string mode = args.Length > 0 ? args[0].Trim().ToLowerInvariant() : "all";
            if (mode != "offline" && mode != "db" && mode != "all")
            {
                Console.WriteLine("Tham so khong hop le: " + mode);
                Console.WriteLine("Dung: QuanLyNhanSu.Tests.exe [offline|db|all]");
                return 2;
            }

            Console.WriteLine("=== LAB7 - KIEM THU HE THONG QUAN LY NHAN SU ===");
            Console.WriteLine("Thoi diem: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Console.WriteLine();

            int fail = 0;

            if (mode == "all" || mode == "offline")
            {
                Console.WriteLine("--- PHAN 1: KIEM THU QUY TAC NGHIEP VU (OFFLINE) ---");
                string kq = RuleTests.RunAll();
                Console.Write(kq);
                fail += DemFail(kq);
                Console.WriteLine();
            }

            if (mode == "all" || mode == "db")
            {
                Console.WriteLine("--- PHAN 2: KIEM THU TICH HOP CSDL ---");
                string msg;
                if (!Db.TestConnection(out msg))
                {
                    Console.WriteLine("FAIL DB00_KetNoi - " + msg);
                    Console.WriteLine("Goi y: chay SQL\\01_CreateTables.sql va SQL\\02_SeedData.sql truoc.");
                    fail++;
                }
                else
                {
                    Console.WriteLine("INFO " + msg);
                    string kq = DbTests.RunAll();
                    Console.Write(kq);
                    fail += DemFail(kq);
                }
                Console.WriteLine();
            }

            Console.WriteLine(fail == 0
                ? "=== KET LUAN: TAT CA KIEM THU PASS ==="
                : "=== KET LUAN: CO " + fail + " KIEM THU FAIL ===");
            return fail == 0 ? 0 : 1;
        }

        private static int DemFail(string text)
        {
            int n = 0, i = 0;
            while ((i = text.IndexOf("FAIL ", i, StringComparison.Ordinal)) >= 0)
            {
                n++;
                i += 5;
            }
            return n;
        }
    }
}
