namespace PosMaster.Dal
{
    public enum EntityStatus
    {
        Inactive,
        Active,
        Closed
    }

    public enum DataSource
    {
        Web,
        Mobile
    }

    public enum SmsProvider
    {
        AfricasTalking,
        Cosmere
    }

    public enum AlertLevel
    {
        Success,
        Warning,
        Error
    }

    public enum Document
    {
        Receipt,
        Grn,
        Po,
        Invoice,
        Customer,
        Leave,
        Order,
        Voucher
    }

    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum GlUserType
    {
        Customer,
        Supplier,
        Payee
    }
    public enum BusinessShortCodeType
    {
        Till,
        Paybill
    }
}
