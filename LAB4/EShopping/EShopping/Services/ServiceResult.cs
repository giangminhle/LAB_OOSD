namespace EShopping.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }     // ví dụ MaKH / MaDH vừa tạo

        public static ServiceResult Ok(string msg, int id = 0)
        {
            return new ServiceResult { Success = true, Message = msg, Id = id };
        }
        public static ServiceResult Fail(string msg)
        {
            return new ServiceResult { Success = false, Message = msg };
        }
    }
}
