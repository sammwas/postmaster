using PosMaster.Dal;
using System;

namespace PosMaster.Services
{
    public class UserCookieData
    {
        public UserCookieData()
        {

        }

        public UserCookieData(User user)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            FullName = user.FullName;
            ClientId = user.ClientId;
            InstanceId = user.InstanceId;
            Role = user.Role;
            ImagePath = user.ImagePath;
            DateCreated = user.DateCreated;
            EmailAddress = user.Email;
            Gender = user.Gender;
        }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string ImagePath { get; set; }
        public Guid ClientId { get; set; }
        public string ClientCode { get; set; }
        public string CurrencyShort { get; set; }
        public string ClientName { get; set; }
        public string ClientLogoPath { get; set; }
        public Guid InstanceId { get; set; }
        public string InstanceCode { get; set; }
        public string InstanceName { get; set; }
        public DateTime DateCreated { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public bool ShowCardPos { get; set; }
    }
}
