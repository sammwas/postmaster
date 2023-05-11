using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IUserInterface
    {
        Task<ReturnData<string>> AddLoginLogAsync(UserLoginLog log);
        Task<ReturnData<string>> LogOutAsync(string personnel);
        Task<ReturnData<List<UserViewModel>>> AllAsync();
        Task<ReturnData<List<UserViewModel>>> ByClientIdAsync(Guid clientId, Guid? instanceId = null);
        Task<ReturnData<UserViewModel>> UpdateAsync(UserViewModel model);
        Task<ReturnData<string>> ConfirmEmailAsync(string userId, string personnel);
        Task<ReturnData<string>> UpdateImageAsync(UploadImageViewModel model);
        Task<ReturnData<bool>> PhoneIdNumberExistsAsync(string number, bool isPhone);
    }
    public class UserInterface : IUserInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<UserInterface> _logger;
        private readonly UserManager<User> _userManager;
        public UserInterface(DatabaseContext context, ILogger<UserInterface> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<ReturnData<string>> AddLoginLogAsync(UserLoginLog log)
        {
            var result = new ReturnData<string>();
            var tag = nameof(AddLoginLogAsync);
            _logger.LogInformation($"{tag} add login log for {log.UserName}");
            try
            {
                if (!string.IsNullOrEmpty(log.UserRole))
                {
                    if (log.UserRole.Equals(Role.SuperAdmin))
                    {
                        result.Success = true;
                        result.Message = "SuperAdmin";
                        return result;
                    }
                }
                await _context.UserLoginLogs.AddAsync(log);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<List<UserViewModel>>> AllAsync()
        {
            var result = new ReturnData<List<UserViewModel>> { Data = new List<UserViewModel>() };
            var tag = nameof(AllAsync);
            _logger.LogInformation($"{tag} get all users");
            try
            {
                var data = await _context.Users
                    .Select(u => new UserViewModel(u))
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} users");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<List<UserViewModel>>> ByClientIdAsync(Guid clientId, Guid? instanceId = null)
        {
            var result = new ReturnData<List<UserViewModel>> { Data = new List<UserViewModel>() };
            var tag = nameof(ByClientIdAsync);
            _logger.LogInformation($"{tag} get all client {clientId} and instance {instanceId} users");
            try
            {
                var dataQry = _context.Users
                    .Where(c => !c.Email.Equals(Constants.SuperAdminEmail) && c.ClientId.Equals(clientId))
                    .AsQueryable();
                if (instanceId.HasValue)
                    dataQry = dataQry.Where(u => u.InstanceId.Equals(instanceId));
                var data = await dataQry.Select(u => new UserViewModel(u))
                  .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} users");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<string>> ConfirmEmailAsync(string userId, string personnel)
        {
            var result = new ReturnData<string>();
            var tag = nameof(ConfirmEmailAsync);
            _logger.LogInformation($"{tag} confirm user email {userId}");
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
                if (user == null)
                {
                    result.Message = "Not Found";
                    _logger.LogInformation($"{tag} confirm email for {userId} {result.Message}");
                    return result;
                }
                user.EmailConfirmed = true;
                user.DateLastModified = DateTime.Now;
                user.LastModifiedBy = personnel;
                await _context.SaveChangesAsync();
                result.Data = userId;
                result.Success = true;
                result.Message = "Confirmed";
                _logger.LogInformation($"{tag} confirm email for {userId} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<string>> LogOutAsync(string personnel)
        {
            var result = new ReturnData<string>();
            var tag = nameof(LogOutAsync);
            _logger.LogInformation($"{tag} user {personnel} logout");
            try
            {
                var loginRec = await _context.UserLoginLogs
                                   .OrderByDescending(l => l.DateCreated)
                                   .FirstOrDefaultAsync(l => l.Personnel.Equals(personnel));
                if (loginRec != null)
                {
                    loginRec.DateLastModified = DateTime.Now;
                    loginRec.LastModifiedBy = personnel;
                    loginRec.LogoutTime = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = "Logged out";
                _logger.LogInformation($"{tag} user account log out {personnel} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<bool>> PhoneIdNumberExistsAsync(string number, bool isPhone)
        {

            var result = new ReturnData<bool>();
            var tag = nameof(PhoneIdNumberExistsAsync);
            var option = isPhone ? "Phone number" : "National ID number";
            _logger.LogInformation($"{tag} check if {option} {number} exists");
            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    result.Message = $"{option} is required";
                    return result;
                }
                number = number.Trim();
                var exists = isPhone ? await _context.Users.AnyAsync(u => u.PhoneNumber.Equals(number))
                 : await _context.Users.AnyAsync(u => u.IdNumber.Equals(number));
                result.Success = true;
                result.Message = exists ? $"{option} Exists" : $"{option} Does not exist";
                result.Data = exists;
                _logger.LogInformation($"{tag} {option} {number} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<UserViewModel>> UpdateAsync(UserViewModel model)
        {
            var result = new ReturnData<UserViewModel> { Data = new UserViewModel() };
            var tag = nameof(UpdateAsync);
            _logger.LogInformation($"{tag} update user {model.UserId} details");
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(model.UserId));
                if (user == null)
                {
                    result.Message = "Not Found";
                    _logger.LogInformation($"{tag} update details for {model.UserId} {result.Message}");
                    return result;
                }
                user.Title = model.Title;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.Gender = model.Gender;
                user.IdNumber = model.IdNumber;
                user.PhoneNumber = model.PhoneNumber;
                user.EmailConfirmed = model.EmailConfirmed;
                user.InstanceId = model.InstanceId;
                user.MaritalStatus = model.MaritalStatus;
                user.Status = model.Status;
                user.DateLastModified = DateTime.Now;
                user.LastModifiedBy = model.Personnel;
                if (model.IsNewImage)
                    user.ImagePath = model.ImagePath;
                if (!user.Role.Equals(model.Role))
                    await _userManager.AddToRoleAsync(user, model.Role);
                if (!user.Email.ToLower().Equals(model.EmailAddress.ToLower()))
                {
                    var any = _context.Users.Any(u => u.Email.ToLower().Equals(model.EmailAddress.ToLower()));
                    if (any)
                    {
                        result.Message = $"{model.EmailAddress} already exists";
                        _logger.LogInformation($"{tag} update details for {model.UserId} {result.Message}");
                        return result;
                    }
                    user.Email = model.EmailAddress;
                    user.NormalizedEmail = model.EmailAddress.ToUpper();
                }
                await _context.SaveChangesAsync();
                result.Data = new UserViewModel(user);
                result.Success = true;
                result.Message = "Updated";
                _logger.LogInformation($"{tag} update user details {model.UserId} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<string>> UpdateImageAsync(UploadImageViewModel model)
        {
            var result = new ReturnData<string>();
            var tag = nameof(UpdateImageAsync);
            _logger.LogInformation($"{tag} update user {model.UserId} profile image");
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(model.UserId));
                if (user == null)
                {
                    result.Message = "Not Found";
                    _logger.LogInformation($"{tag} update image for {model.UserId} {result.Message}");
                    return result;
                }
                var prevImage = user.ImagePath;
                user.ImagePath = model.CurrentImage;
                user.DateLastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Data = prevImage;
                result.Success = true;
                result.Message = "Updated";
                _logger.LogInformation($"{tag} update image for {model.UserId} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }
    }
}
