using System;

namespace PosMaster.Services
{
    public class AuthenticationLicence
    {
        public bool IsValid => RemainingDays > 0;
        public string Licence { get; set; }
        public double RemainingDays { get; set; }
        public string ExpiryDate => DateTime.Now.AddDays(RemainingDays).ToString("dd-MMM-yyyy");
    }
}
