namespace QuanLyNhanSu.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }

        public static ServiceResult Ok(string message, int id = 0, string code = null)
        {
            return new ServiceResult { Success = true, Message = message, Id = id, Code = code };
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }
    }
}
