﻿namespace PosMaster.Dal
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public string PostalAddress { get; set; }
        public string Town { get; set; }
        public decimal CreditLimit { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string PinNo { get; set; }
        public bool ShowInvoiceBalance { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
