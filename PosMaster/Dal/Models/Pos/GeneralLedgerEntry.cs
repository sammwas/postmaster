using System;

namespace PosMaster.Dal
{
    public class GeneralLedgerEntry : BaseEntity
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Document Document { get; set; }
        public string DocumentNumber { get; set; }
        public string VoteHead { get; set; }
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        public GlUserType UserType { get; set; }
    }
}
