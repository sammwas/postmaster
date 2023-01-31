namespace PosMaster.Dal
{
    public class TaxType : BaseEntity
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}