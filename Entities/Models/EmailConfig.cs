namespace Entities.Models
{
    public class EmailConfig
    {
        public int Id { get; set; }
        public string EmailFrom { get; set; }
        public string Password { get; set; }
        public bool IsSSL { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public string UserId { get; set; }
    }

    public class EmailSettings : EmailConfig
    {
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
