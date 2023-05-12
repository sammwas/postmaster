namespace PosMaster.Dal
{
    public class SmsSentMessage : BaseEntity
    {
        public string Message { get; set; }
        public string PhoneNumber { get; set; }
        public string ResponseData { get; set; }
        public bool IsSent { get; set; }
        public string ResponseMessage { get; set; }
        public string Stamp { get; set; }
        public string Timestamp => $"{DateCreated:dd-MMM-yyyy} {string.Format("{0:hh:mm tt}", DateCreated)}";

    }
}
