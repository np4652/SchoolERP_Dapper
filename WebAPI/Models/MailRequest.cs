namespace WebAPI.Models
{
    public class MailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; } = "amit.roundpay@outlook.com";
    }
}
