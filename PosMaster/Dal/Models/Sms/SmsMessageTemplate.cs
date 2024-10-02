namespace PosMaster.Dal
{
    public class SmsMessageTemplate : BaseEntity
    {
        public string Variables { get; set; }
        public string Template { get; set; }
        public MessageOption Option { get; set; }
        public enum MessageOption
        {
            BalanceReminder,
            Purchase
        }
    }
}
