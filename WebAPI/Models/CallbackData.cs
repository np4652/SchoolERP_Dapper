namespace WebAPI.Models
{
    public class CallbackData
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public string RequestIP { get; set; }
        public string RequestBrowser { get; set; }
        public string EntryDate { get; set; }
        public int APIID { get; set; }
        public bool? InActiveMode { get; set; }
    }
    public class CallBackHitLog
    {

        public string RequestedId { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string EntryOn { get; set; }
        public char? HookType { get; set; }
    }
}
