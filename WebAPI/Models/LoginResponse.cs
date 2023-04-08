using Data;

namespace WebAPI.Models
{
    public class LoginResponse:Response
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
