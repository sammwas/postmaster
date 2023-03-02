using PosMaster.Dal.Interfaces;
using PosMaster.Extensions;
using System;

namespace PosMaster.Services
{
    public static class LicencingService
    {

        private const string KEYSEPERATOR = "&Q_ILIMO&";
        private const int LICENCECODE = 3745;
        public static ReturnData<AuthenticationLicence> GenerateLicence(Guid clientId, int months, int code, string clientName)
        {
            var res = new ReturnData<AuthenticationLicence>
            {
                Data = new AuthenticationLicence()
            };
            try
            {
                if (!code.Equals(LICENCECODE))
                {
                    res.Message = "Invalid Code";
                    return res;
                }

                var dateTime = DateTime.Now;
                var expires = dateTime.AddMonths(months);
                var expiresStr = expires.ToString("dd-MMM-yyyy");
                var hashed = Helpers.EncryptSha1($"{clientId}{clientName.ToLower()}{expiresStr}");
                var expHashed = Encrypter.Encrypt(expiresStr);
                res.Data.Licence = Helpers.Base64Encode($"{hashed}{KEYSEPERATOR}{expHashed}");
                res.Success = true;
                res.Message = "Generated";
                res.Data.RemainingDays = (expires - dateTime).TotalDays;
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                res.Message = "Error occured. Try again later";
                return res;
            }
        }

        public static ReturnData<AuthenticationLicence> VerifyLicence(string licence, Guid clientId, string clientName)
        {
            var res = new ReturnData<AuthenticationLicence>
            {
                Data = new AuthenticationLicence()
            };
            try
            {
                if (string.IsNullOrEmpty(licence))
                {
                    res.Message = "Licence is Empty";
                    return res;
                }

                var token = Helpers.Base64Decode(licence);
                var dateTime = DateTime.Now;
                var data = token.Split($"{KEYSEPERATOR}");
                if (!data.Length.Equals(2))
                {
                    res.Message = "Incorrect licence Structure Format";
                    return res;
                }

                var realExpires = Encrypter.Decrypt(data[1]);
                var vDate = DateTime.TryParse(realExpires, out var expiry);
                if (!vDate)
                {
                    res.Message = "Incorrect licence Time Format";
                    return res;
                }

                if (expiry < dateTime)
                {
                    res.Message = "Licence already Expired";
                    return res;
                }

                var realHashed = Helpers.EncryptSha1($"{clientId}{clientName.ToLower()}{realExpires}");
                res.Success = data[0].Equals(realHashed);
                res.Message = res.Success ? "Valid" : "Invalid";
                res.Data.RemainingDays = (expiry - dateTime).TotalDays;
                res.Data.Licence = licence;
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                res.Message = "Verify:: Error Occured";
                return res;
            }
        }
    }

}
