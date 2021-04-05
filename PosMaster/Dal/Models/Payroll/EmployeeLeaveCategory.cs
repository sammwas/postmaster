namespace PosMaster.Dal
{
    public class EmployeeLeaveCategory : BaseEntity
    {
        public string Title { get; set; }
        public decimal MaxDays { get; set; }
        public string AllowedGender { get; set; }
        public string AllowedGenderStr => string.IsNullOrEmpty(AllowedGender) ? "--ALL--" : AllowedGender;
    }
}
