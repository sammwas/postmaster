using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IClientInstanceInterface
    {
        Task<ReturnData<ClientInstance>> EditAsync(ClientInstanceViewModel model);
        Task<ReturnData<List<ClientInstance>>> AllAsync();
        Task<ReturnData<List<ClientInstance>>> ByClientIdAsync(Guid clientId);
        Task<ReturnData<ClientInstance>> ByIdAsync(Guid id);
    }

    public class ClientInstanceImplementation : IClientInstanceInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ClientInstanceImplementation> _logger;
        public ClientInstanceImplementation(DatabaseContext context, ILogger<ClientInstanceImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReturnData<List<ClientInstance>>> AllAsync()
        {
            var result = new ReturnData<List<ClientInstance>> { Data = new List<ClientInstance>() };
            var tag = nameof(AllAsync);
            _logger.LogInformation($"{tag} get all client instances");
            try
            {
                var data = await _context.ClientInstances
                    .Include(c => c.Client)
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} client instances");
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

        public async Task<ReturnData<List<ClientInstance>>> ByClientIdAsync(Guid clientId)
        {
            var result = new ReturnData<List<ClientInstance>> { Data = new List<ClientInstance>() };
            var tag = nameof(ByClientIdAsync);
            _logger.LogInformation($"{tag} get client instances for client- {clientId}");
            try
            {
                var instances = await _context.ClientInstances
                    .Include(c => c.Client)
                    .Where(c => c.ClientId.Equals(clientId))
                    .ToListAsync();
                result.Success = instances.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = instances;
                _logger.LogInformation($"{tag} found {instances.Count} client instance for client {clientId} {result.Message}");
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

        public async Task<ReturnData<ClientInstance>> ByIdAsync(Guid id)
        {
            var result = new ReturnData<ClientInstance> { Data = new ClientInstance() };
            var tag = nameof(ByIdAsync);
            _logger.LogInformation($"{tag} get client instance by id - {id}");
            try
            {
                var client = await _context.ClientInstances.Include(c => c.Client)
                    .FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = client != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = client;
                _logger.LogInformation($"{tag} client instance {id} {result.Message}");
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

        public async Task<ReturnData<ClientInstance>> EditAsync(ClientInstanceViewModel model)
        {
            var result = new ReturnData<ClientInstance> { Data = new ClientInstance() };
            var tag = nameof(EditAsync);
            _logger.LogInformation($"{tag} edit client instance");
            try
            {
                var hasOpenTime = DateTime.TryParse(model.OpeningTime, out var oTime);
                var hasCloseTime = DateTime.TryParse(model.ClosingTime, out var cTime);
                if (model.IsEditMode)
                {
                    var dbInstance = await _context.ClientInstances
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbInstance == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbInstance.Latitude = model.Latitude;
                    dbInstance.Longitude = model.Longitude;
                    dbInstance.Name = model.Name;
                    dbInstance.Code = model.Code;
                    dbInstance.PostalAddress = model.PostalAddress;
                    dbInstance.EmailAddress = model.EmailAddress;
                    dbInstance.Town = model.Town;
                    dbInstance.Location = model.Location;
                    dbInstance.PrimaryTelephone = model.PrimaryTelephone;
                    dbInstance.SecondaryTelephone = model.SecondaryTelephone;
                    dbInstance.OpeningTime = hasOpenTime ? oTime : DateTime.Now;
                    dbInstance.ClosingTime = hasCloseTime ? cTime : DateTime.Now;
                    dbInstance.LastModifiedBy = model.Personnel;
                    dbInstance.DateLastModified = DateTime.Now;
                    dbInstance.Notes = model.Notes;
                    dbInstance.Status = model.Status;
                    dbInstance.PinNo = model.PinNo;
                    dbInstance.ReceiptFooterNotes = model.ReceiptNotes;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbInstance;
                    _logger.LogInformation($"{tag} updated {dbInstance.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var instance = new ClientInstance
                {
                    OpeningTime = hasOpenTime ? oTime : DateTime.Now,
                    ClosingTime = hasCloseTime ? cTime : DateTime.Now,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Name = model.Name,
                    PostalAddress = model.PostalAddress,
                    Town = model.Town,
                    Location = model.Location,
                    PrimaryTelephone = model.PrimaryTelephone,
                    SecondaryTelephone = model.SecondaryTelephone,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Code = model.Code,
                    EmailAddress = model.EmailAddress,
                    PinNo = model.PinNo,
                    ReceiptFooterNotes = model.ReceiptNotes
                };
                instance.InstanceId = instance.Id;
                _context.ClientInstances.Add(instance);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = instance;
                _logger.LogInformation($"{tag} added {instance.Name}  {instance.Id} : {result.Message}");
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
