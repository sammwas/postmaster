using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IHumanResourceInterface
    {
        Task<ReturnData<Bank>> EditBankAsync(BankViewModel model);
        Task<ReturnData<Bank>> BankByIdAsync(Guid id);
        Task<ReturnData<List<Bank>>> BanksAsync(Guid clientId);
        Task<ReturnData<EmployeeLeaveCategory>> EditLeaveCategoryAsync(EmployeeLeaveCategoryViewModel model);
        Task<ReturnData<EmployeeLeaveCategory>> LeaveCategoryByIdAsync(Guid id);
        Task<ReturnData<List<EmployeeLeaveCategory>>> LeaveCategoriesAsync(Guid clientId);
        Task<ReturnData<EmployeeSalary>> EditEmployeeSalaryAsync(EmployeeSalaryViewModel model);
        Task<ReturnData<EmployeeSalary>> EmployeeSalaryByUserIdAsync(string userId);
        Task<ReturnData<List<EmployeeSalary>>> EmployeeSalariesAsync(Guid clientId, Guid? instanceId);
        Task<ReturnData<EmployeeKin>> EditEmployeeKinAsync(EmployeeKinViewModel model);
        Task<ReturnData<EmployeeKin>> EmployeeKinByUserIdAsync(string userId);
        Task<ReturnData<List<EmployeeLeaveCategory>>> EmployeeLeaveBalancesAsync(Guid clientId, string userId, string gender, Guid? categoryId);
        Task<ReturnData<EmployeeLeaveApplication>> EditLeaveApplicationAsync(LeaveApplicationViewModel model);
        Task<ReturnData<List<EmployeeLeaveApplication>>> LeaveApplicationsAsync(Guid? clientId, Guid? instanceId, string userId = "", string dtFrom = "",
            string dtTo = "", string search = "");
        Task<ReturnData<EmployeeLeaveApplication>> LeaveApplicationByIdAsync(Guid id);
        Task<ReturnData<Guid>> ApproveLeaveApplicationAsync(ApproveLeaveViewModel model);
        Task<ReturnData<List<MonthlyPayViewModel>>> MonthlyPaymentsAsync(int month, int year, Guid clientId, Guid? instanceId);
        Task<ReturnData<decimal>> ApproveMonthPaymentAsync(ApproveMonthlyPaymentViewModel model);
    }

    public class HumanResourceImplementation : IHumanResourceInterface
    {
        private readonly DatabaseContext _context;
        private readonly IProductInterface _productInterface;
        private readonly ILogger<HumanResourceImplementation> _logger;
        public HumanResourceImplementation(DatabaseContext context, ILogger<HumanResourceImplementation> logger, IProductInterface productInterface)
        {
            _context = context;
            _logger = logger;
            _productInterface = productInterface;
        }

        public async Task<ReturnData<Guid>> ApproveLeaveApplicationAsync(ApproveLeaveViewModel model)
        {
            var result = new ReturnData<Guid> { Data = model.Id };
            var tag = nameof(ApproveLeaveApplicationAsync);
            _logger.LogInformation($"{tag} approve leave application {model.Id}");
            try
            {
                var application = await _context.EmployeeLeaveApplications
                    .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                if (application == null)
                {
                    result.Message = "Application not Found";
                    _logger.LogInformation($"{tag} {result.Message}");
                    return result;
                }
                application.LastModifiedBy = model.Personnel;
                application.DateLastModified = DateTime.Now;
                application.ApplicationStatus = model.Status;
                application.Comments = model.Comment;
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = $"Leave {application.Code} {model.Status}";
                _logger.LogInformation($"{tag} {result.Message}");
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

        public async Task<ReturnData<decimal>> ApproveMonthPaymentAsync(ApproveMonthlyPaymentViewModel model)
        {
            var result = new ReturnData<decimal>();
            var tag = nameof(ApproveMonthPaymentAsync);
            _logger.LogInformation($"{tag} approve monthly payments for clientId {model.ClientId}, instanceId {model.InstanceId}, month {model.Month} and year {model.Year}");
            try
            {
                var sPaymentsQry = _context.EmployeeSalaries
                    .Where(s => s.ClientId.Equals(model.ClientId)).AsQueryable();
                if (model.InstanceId != null)
                    sPaymentsQry = sPaymentsQry.Where(p => p.InstanceId.Equals(model.InstanceId.Value));
                var sData = await sPaymentsQry.ToListAsync();
                if (!sData.Any())
                {
                    result.Message = "No records found";
                    _logger.LogInformation($"{tag} not approved: {result.Message}");
                    return result;
                }
                decimal spentAmount = 0;
                foreach (var salary in sData)
                {
                    var approved = _context.EmployeeMonthPayments
                        .FirstOrDefault(p => p.UserId.Equals(salary.UserId)
                        && p.Month.Equals(model.Month) && p.Year.Equals(model.Year));
                    if (approved == null)
                    {
                        var mPayment = new EmployeeMonthPayment
                        {
                            ClientId = salary.ClientId,
                            InstanceId = salary.InstanceId,
                            UserId = salary.UserId,
                            BasicPay = salary.BasicPay,
                            Allowance = salary.Allowance,
                            Deduction = salary.Deduction,
                            Month = model.Month,
                            Year = model.Year,
                            Personnel = model.Personnel,
                            Code = $"{salary.UserId}_{model.Month}{model.Year}"
                        };
                        _context.EmployeeMonthPayments.Add(mPayment);
                        spentAmount += mPayment.Netpay;
                    }
                    else
                    {
                        approved.BasicPay = salary.BasicPay;
                        approved.Allowance = salary.Allowance;
                        approved.Deduction = salary.Deduction;
                        approved.LastModifiedBy = model.Personnel;
                        approved.DateLastModified = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = "Approved";
                result.Data = spentAmount;
                _logger.LogInformation($"{tag} approved {sData.Count} monthly salaries of {result.Data}");
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

        public async Task<ReturnData<Bank>> BankByIdAsync(Guid id)
        {
            var result = new ReturnData<Bank> { Data = new Bank() };
            var tag = nameof(BankByIdAsync);
            _logger.LogInformation($"{tag} get bank by id - {id}");
            try
            {
                var bank = await _context.Banks
                    .FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = bank != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = bank;
                _logger.LogInformation($"{tag} bank {id} {result.Message}");
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

        public async Task<ReturnData<List<Bank>>> BanksAsync(Guid clientId)
        {
            var result = new ReturnData<List<Bank>> { Data = new List<Bank>() };
            var tag = nameof(BanksAsync);
            _logger.LogInformation($"{tag} get all client banks");
            try
            {
                var data = await _context.Banks
                    .Where(b => b.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} client banks");
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

        public async Task<ReturnData<Bank>> EditBankAsync(BankViewModel model)
        {
            var result = new ReturnData<Bank> { Data = new Bank() };
            var tag = nameof(EditBankAsync);
            _logger.LogInformation($"{tag} edit client bank");
            try
            {
                if (model.IsEditMode)
                {
                    var dbBank = await _context.Banks
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbBank == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbBank.Name = model.Name;
                    dbBank.Code = model.Code;
                    dbBank.EmailAddress = model.EmailAddress;
                    dbBank.PhoneNumber = model.PhoneNumber;
                    dbBank.Website = model.Website;
                    dbBank.ContactPerson = model.ContactPerson;
                    dbBank.LastModifiedBy = model.Personnel;
                    dbBank.DateLastModified = DateTime.Now;
                    dbBank.Notes = model.Notes;
                    dbBank.Status = model.Status;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbBank;
                    _logger.LogInformation($"{tag} updated {dbBank.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var bank = new Bank
                {
                    Name = model.Name,
                    Website = model.Website,
                    PhoneNumber = model.PhoneNumber,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Code = model.Code,
                    EmailAddress = model.EmailAddress,
                    ContactPerson = model.ContactPerson,
                    InstanceId = model.InstanceId
                };
                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = bank;
                _logger.LogInformation($"{tag} added {bank.Name}  {bank.Id} : {result.Message}");
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

        public async Task<ReturnData<EmployeeKin>> EditEmployeeKinAsync(EmployeeKinViewModel model)
        {
            var result = new ReturnData<EmployeeKin> { Data = new EmployeeKin() };
            var tag = nameof(EditEmployeeKinAsync);
            _logger.LogInformation($"{tag} edit user {model.UserId} next of kin");
            try
            {
                if (model.IsEditMode)
                {
                    var dbKin = await _context.EmployeeKins
                        .FirstOrDefaultAsync(c => c.UserId.Equals(model.UserId));
                    if (dbKin == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbKin.FirstName = model.FirstName;
                    dbKin.MiddleName = model.MiddleName;
                    dbKin.LastName = model.LastName;
                    dbKin.Code = model.Code;
                    dbKin.EmailAddress = model.EmailAddress;
                    dbKin.PhoneNumber = model.PhoneNumber;
                    dbKin.AltPhoneNumber = model.AltPhoneNumber;
                    dbKin.PostalAddress = model.PostalAddress;
                    dbKin.Town = model.Town;
                    dbKin.Title = model.Title;
                    dbKin.Gender = model.Gender;
                    dbKin.Relationship = model.Relationship;
                    dbKin.LastModifiedBy = model.Personnel;
                    dbKin.DateLastModified = DateTime.Now;
                    dbKin.Notes = model.Notes;
                    dbKin.Status = model.Status;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbKin;
                    _logger.LogInformation($"{tag} updated {dbKin.FirstName} {model.Id} : {result.Message}");
                    return result;
                }

                var kin = new EmployeeKin
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    AltPhoneNumber = model.AltPhoneNumber,
                    PhoneNumber = model.PhoneNumber,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Code = model.Code,
                    EmailAddress = model.EmailAddress,
                    Title = model.Title,
                    Relationship = model.Relationship,
                    Gender = model.Gender,
                    InstanceId = model.InstanceId,
                    PostalAddress = model.PostalAddress,
                    Town = model.Town,
                    UserId = model.UserId
                };
                _context.EmployeeKins.Add(kin);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = kin;
                _logger.LogInformation($"{tag} added {kin.FirstName}  {kin.Id} : {result.Message}");
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

        public async Task<ReturnData<EmployeeSalary>> EditEmployeeSalaryAsync(EmployeeSalaryViewModel model)
        {
            var result = new ReturnData<EmployeeSalary> { Data = new EmployeeSalary() };
            var tag = nameof(EditEmployeeSalaryAsync);
            _logger.LogInformation($"{tag} edit employee salary for {model.UserId}");
            try
            {

                var salary = await _context.EmployeeSalaries
                    .FirstOrDefaultAsync(c => c.UserId.Equals(model.UserId));
                if (salary == null)
                {
                    var empSalary = new EmployeeSalary
                    {
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        UserId = model.UserId,
                        Allowance = model.Allowance,
                        Deduction = model.Deduction,
                        BasicPay = model.BasicPay,
                        Bank = model.Bank,
                        BankAccount = model.BankAccount,
                        Personnel = model.Personnel,
                        Status = model.Status,
                        Notes = model.Notes
                    };
                    var salaryLog = new EmployeeSalaryLog
                    {
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        UserId = model.UserId,
                        Personnel = model.Personnel,
                        SalaryFrom = 0,
                        SalaryTo = empSalary.NetAmount
                    };
                    _context.EmployeeSalaryLogs.Add(salaryLog);
                    _context.EmployeeSalaries.Add(empSalary);
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Added";
                    result.Data = empSalary;
                    return result;
                }
                var prevNet = salary.NetAmount;
                salary.Code = model.Code;
                salary.Allowance = model.Allowance;
                salary.BasicPay = model.BasicPay;
                salary.Deduction = model.Deduction;
                salary.Bank = model.Bank;
                salary.BankAccount = model.BankAccount;
                salary.LastModifiedBy = model.Personnel;
                salary.DateLastModified = DateTime.Now;
                salary.Notes = model.Notes;
                salary.Status = model.Status;
                if (prevNet != salary.NetAmount)
                {
                    var salaryLog = new EmployeeSalaryLog
                    {
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        UserId = model.UserId,
                        Personnel = model.Personnel,
                        SalaryFrom = prevNet,
                        SalaryTo = salary.NetAmount
                    };
                    _context.EmployeeSalaryLogs.Add(salaryLog);
                }
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Updated";
                result.Data = salary;
                _logger.LogInformation($"{tag} updated {salary.Code} {model.Id} : {result.Message}");
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

        public async Task<ReturnData<EmployeeLeaveApplication>> EditLeaveApplicationAsync(LeaveApplicationViewModel model)
        {
            var result = new ReturnData<EmployeeLeaveApplication> { Data = new EmployeeLeaveApplication() };
            var tag = nameof(EditLeaveApplicationAsync);
            _logger.LogInformation($"{tag} edit leave application");
            try
            {
                var cId = Guid.Parse(model.EmployeeLeaveCategoryId);
                var balancesRes = await EmployeeLeaveBalancesAsync(model.ClientId, model.UserId, model.Gender, cId);
                if (!balancesRes.Success)
                {
                    result.Message = balancesRes.Message;
                    result.ErrorMessage = balancesRes.ErrorMessage;
                    _logger.LogInformation($"{tag} - {result.Message}");
                    return result;
                }

                var category = balancesRes.Data.FirstOrDefault(c => c.Id.Equals(cId));
                if (category == null)
                {
                    result.Message = "Provided category not found";
                    _logger.LogInformation($"{tag} - {result.Message}");
                    return result;

                }

                if (model.Days > category.MaxDays)
                {
                    result.Message = $"Max {category.MaxDays} days for {category.Title}";
                    _logger.LogInformation($"{tag} - {result.Message}");
                    return result;
                }

                var dtFrom = DateTime.Parse(model.DateFrom);
                var dtTo = DateTime.Parse(model.DateTo);
                if (dtFrom > dtTo)
                {
                    result.Message = $"Date to should be greater";
                    _logger.LogInformation($"{tag} - {result.Message}");
                    return result;
                }

                if (model.IsEditMode)
                {
                    var dbLeave = await _context.EmployeeLeaveApplications
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbLeave == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }

                    if (dbLeave.ApplicationStatus.Equals(ApplicationStatus.Pending))
                    {
                        result.Message = $"{dbLeave.Code} already {dbLeave.ApplicationStatus}";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbLeave.EmployeeLeaveCategory = category;
                    dbLeave.EmployeeLeaveCategoryId = cId;
                    dbLeave.Code = model.Code;
                    dbLeave.DateFrom = dtFrom;
                    dbLeave.DateTo = dtTo;
                    dbLeave.Days = model.Days;
                    dbLeave.LastModifiedBy = model.Personnel;
                    dbLeave.DateLastModified = DateTime.Now;
                    dbLeave.Notes = model.Notes;
                    dbLeave.Status = model.Status;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbLeave;
                    _logger.LogInformation($"{tag} updated {dbLeave.Code} {model.Id} : {result.Message}");
                    return result;
                }
                var anyPending = _context.EmployeeLeaveApplications.Any(a => a.UserId.Equals(model.UserId)
                 && a.EmployeeLeaveCategoryId.Equals(cId) && a.ApplicationStatus.Equals(ApplicationStatus.Pending));
                if (anyPending)
                {
                    result.Message = "Already has a Pending application";
                    _logger.LogInformation($"{tag} adding failed for user {model.UserId} : {result.Message}");
                    return result;
                }
                model.Code = _productInterface.DocumentRefNumber(Document.Leave, model.ClientId);
                var leaveApplication = new EmployeeLeaveApplication
                {
                    EmployeeLeaveCategory = category,
                    EmployeeLeaveCategoryId = cId,
                    DateTo = dtTo,
                    DateFrom = dtFrom,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Code = model.Code,
                    Days = model.Days,
                    UserId = model.UserId,
                    InstanceId = model.InstanceId
                };
                _context.EmployeeLeaveApplications.Add(leaveApplication);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = leaveApplication;
                _logger.LogInformation($"{tag} added {category.Title} for user {model.UserId} : {result.Message}");
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

        public async Task<ReturnData<EmployeeLeaveCategory>> EditLeaveCategoryAsync(EmployeeLeaveCategoryViewModel model)
        {
            var result = new ReturnData<EmployeeLeaveCategory> { Data = new EmployeeLeaveCategory() };
            var tag = nameof(EditLeaveCategoryAsync);
            _logger.LogInformation($"{tag} edit leave category");
            try
            {
                if (model.IsEditMode)
                {
                    var dbCategory = await _context.EmployeeLeaveCategories
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbCategory == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbCategory.Title = model.Title;
                    dbCategory.Code = model.Code;
                    dbCategory.MaxDays = model.MaxDays;
                    dbCategory.AllowedGender = model.FemaleOnly ? "Female" : "";
                    dbCategory.LastModifiedBy = model.Personnel;
                    dbCategory.DateLastModified = DateTime.Now;
                    dbCategory.Notes = model.Notes;
                    dbCategory.Status = model.Status;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbCategory;
                    _logger.LogInformation($"{tag} updated {dbCategory.Title} {model.Id} : {result.Message}");
                    return result;
                }

                var category = new EmployeeLeaveCategory
                {
                    Title = model.Title,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Code = model.Code,
                    MaxDays = model.MaxDays,
                    AllowedGender = model.FemaleOnly ? "Female" : "",
                    InstanceId = model.InstanceId
                };
                _context.EmployeeLeaveCategories.Add(category);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = category;
                _logger.LogInformation($"{tag} added {category.Title}  {category.Id} : {result.Message}");
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

        public async Task<ReturnData<EmployeeKin>> EmployeeKinByUserIdAsync(string userId)
        {
            var result = new ReturnData<EmployeeKin> { Data = new EmployeeKin() };
            var tag = nameof(EmployeeKinByUserIdAsync);
            _logger.LogInformation($"{tag} get employee {userId} kin");
            try
            {
                var salary = await _context.EmployeeKins.Include(u => u.User)
                    .FirstOrDefaultAsync(c => c.UserId.Equals(userId));
                result.Success = salary != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = salary;
                _logger.LogInformation($"{tag} kin for {userId} {result.Message}");
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

        public async Task<ReturnData<List<EmployeeLeaveCategory>>> EmployeeLeaveBalancesAsync(Guid clientId, string userId, string gender, Guid? categoryId)
        {
            var result = new ReturnData<List<EmployeeLeaveCategory>> { Data = new List<EmployeeLeaveCategory>() };
            var tag = nameof(EmployeeLeaveBalancesAsync);
            _logger.LogInformation($"{tag} get employee {userId} leave balances per category");
            try
            {
                var categoriesQry = _context.EmployeeLeaveCategories
                    .Where(c => c.ClientId.Equals(clientId))
                    .AsQueryable();
                if (categoryId.HasValue)
                    categoriesQry = categoriesQry.Where(c => c.Id.Equals(categoryId.Value));
                var categories = await categoriesQry.OrderByDescending(c => c.Title).ToListAsync();
                foreach (var category in categories)
                {
                    if (!string.IsNullOrEmpty(category.AllowedGender))
                    {
                        if (!category.AllowedGender.Equals(gender))
                            continue;
                    }
                    var days = _context.EmployeeLeaveApplications
                        .Where(a => a.EmployeeLeaveCategoryId.Equals(category.Id)
                        && a.ApplicationStatus.Equals(ApplicationStatus.Approved)
                        && a.UserId.Equals(userId) && a.DateCreated.Year.Equals(DateTime.Now.Year))
                        .Sum(a => a.Days);
                    category.MaxDays -= days;
                    if (category.MaxDays > 0)
                        result.Data.Add(category);
                }
                result.Success = result.Data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                _logger.LogInformation($"{tag} found {result.Data.Count} employee leave categories");
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

        public async Task<ReturnData<List<EmployeeSalary>>> EmployeeSalariesAsync(Guid clientId, Guid? instanceId)
        {
            var result = new ReturnData<List<EmployeeSalary>> { Data = new List<EmployeeSalary>() };
            var tag = nameof(EmployeeSalariesAsync);
            _logger.LogInformation($"{tag} get client {clientId} employee salaries {instanceId}");
            try
            {
                var dataQry = _context.EmployeeSalaries.Include(s => s.User)
                    .Where(b => b.ClientId.Equals(clientId)).AsQueryable();
                if (instanceId != null)
                    dataQry = dataQry.Where(d => d.InstanceId.Equals(instanceId.Value));
                var data = await dataQry.OrderByDescending(c => c.DateCreated)
                   .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} employee salary records");
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

        public async Task<ReturnData<EmployeeSalary>> EmployeeSalaryByUserIdAsync(string userId)
        {
            var result = new ReturnData<EmployeeSalary> { Data = new EmployeeSalary() };
            var tag = nameof(EmployeeSalaryByUserIdAsync);
            _logger.LogInformation($"{tag} get employee {userId} salary");
            try
            {
                var salary = await _context.EmployeeSalaries
                    .FirstOrDefaultAsync(c => c.UserId.Equals(userId));
                result.Success = salary != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = salary;
                _logger.LogInformation($"{tag} salary record for {userId} {result.Message}");
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

        public async Task<ReturnData<EmployeeLeaveApplication>> LeaveApplicationByIdAsync(Guid id)
        {
            var result = new ReturnData<EmployeeLeaveApplication> { Data = new EmployeeLeaveApplication() };
            var tag = nameof(LeaveApplicationByIdAsync);
            _logger.LogInformation($"{tag} get bank by id - {id}");
            try
            {
                var application = await _context.EmployeeLeaveApplications
                    .Include(a => a.EmployeeLeaveCategory)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id.Equals(id));
                result.Success = application != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = application;
                _logger.LogInformation($"{tag} leave application {id} {result.Message}");
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

        public async Task<ReturnData<List<EmployeeLeaveApplication>>> LeaveApplicationsAsync(Guid? clientId, Guid? instanceId, string userId = "", string dateFrom = "",
            string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<EmployeeLeaveApplication>> { Data = new List<EmployeeLeaveApplication>() };
            var tag = nameof(LeaveApplicationsAsync);
            _logger.LogInformation($"{tag} get leave applications: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.EmployeeLeaveApplications
                    .Include(l => l.User)
                    .Include(l => l.EmployeeLeaveCategory)
                    .AsQueryable();
                if (clientId != null)
                    dataQuery = dataQuery.Where(r => r.ClientId.Equals(clientId.Value));
                if (instanceId != null)
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Code.ToLower().Contains(search.ToLower()));
                var data = await dataQuery.OrderByDescending(r => r.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} leave applications");
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

        public async Task<ReturnData<List<EmployeeLeaveCategory>>> LeaveCategoriesAsync(Guid clientId)
        {
            var result = new ReturnData<List<EmployeeLeaveCategory>> { Data = new List<EmployeeLeaveCategory>() };
            var tag = nameof(LeaveCategoriesAsync);
            _logger.LogInformation($"{tag} get all client leave categories");
            try
            {
                var data = await _context.EmployeeLeaveCategories
                    .Where(b => b.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} client leave categories");
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

        public async Task<ReturnData<EmployeeLeaveCategory>> LeaveCategoryByIdAsync(Guid id)
        {
            var result = new ReturnData<EmployeeLeaveCategory> { Data = new EmployeeLeaveCategory() };
            var tag = nameof(LeaveCategoryByIdAsync);
            _logger.LogInformation($"{tag} get leave category by id - {id}");
            try
            {
                var category = await _context.EmployeeLeaveCategories
                    .FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = category != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = category;
                _logger.LogInformation($"{tag} leave category {id} {result.Message}");
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

        public async Task<ReturnData<List<MonthlyPayViewModel>>> MonthlyPaymentsAsync(int month, int year, Guid clientId, Guid? instanceId)
        {
            var result = new ReturnData<List<MonthlyPayViewModel>> { Data = new List<MonthlyPayViewModel>() };
            var tag = nameof(MonthlyPaymentsAsync);
            _logger.LogInformation($"{tag} get monthly payments for clientId {clientId}, instanceId {instanceId}, month {month} and year {year}");
            try
            {
                var approved = _context.EmployeeMonthPayments
                    .Any(p => p.Month.Equals(month) && p.Year.Equals(year) && p.ClientId.Equals(clientId));
                if (approved)
                {
                    var mPaymentsQry = _context.EmployeeMonthPayments
                        .Include(p => p.User)
                        .Where(p => p.Month.Equals(month) && p.Year.Equals(year) && p.ClientId.Equals(clientId));
                    if (instanceId != null)
                        mPaymentsQry = mPaymentsQry.Where(p => p.InstanceId.Equals(instanceId.Value));
                    var mData = await mPaymentsQry.OrderByDescending(p => p.BasicPay)
                        .Select(p => new MonthlyPayViewModel(p))
                        .ToListAsync();
                    result.Success = mData.Any();
                    result.Message = result.Success ? "Found" : "Not Found";
                    if (result.Success)
                        result.Data = mData;
                    _logger.LogInformation($"{tag} found {mData.Count} monthly payments");
                    return result;
                }

                var cMonth = DateTime.Now.Month;
                var cYear = DateTime.Now.Year;
                var cDays = DateTime.DaysInMonth(cYear, cMonth);
                var cMaxDate = new DateTime(cYear, cMonth, cDays);
                var iDays = DateTime.DaysInMonth(year, month);
                var iMaxDate = new DateTime(year, month, iDays);
                if (iMaxDate > cMaxDate)
                {
                    result.Message = $"Future months not allowed";
                    _logger.LogInformation($"{tag} {result.Message}");
                    return result;
                }

                var sPaymentsQry = _context.EmployeeSalaries
                        .Include(p => p.User)
                        .Where(p => p.ClientId.Equals(clientId)).AsQueryable();
                if (instanceId != null)
                    sPaymentsQry = sPaymentsQry.Where(p => p.InstanceId.Equals(instanceId.Value));
                var sData = await sPaymentsQry.OrderByDescending(p => p.BasicPay)
                    .Select(p => new MonthlyPayViewModel(p) { Month = month, Year = year })
                    .ToListAsync();
                result.Success = sData.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = sData;
                _logger.LogInformation($"{tag} found {sData.Count} monthly salaries");
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
