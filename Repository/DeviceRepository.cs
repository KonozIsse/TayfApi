using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Repository
{
    public class DeviceRepository : RepositoryBase<Device>, IDeviceRepository
    {
        public DeviceRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Device> CheckDevice(string token , bool trackChanges)
        => await FindByCondition(r => r.DeviceToken == token , trackChanges).FirstOrDefaultAsync();
        public async Task<Device> GetDeviceUser( int deviceId, int userId ,bool trackChanges)
       => await FindByCondition(c => c.Id == deviceId && c.UserId == userId, trackChanges).SingleOrDefaultAsync();
        public async Task<List<Device>> GetDevicesUserId (int userId, bool trackChanges)
        => await FindByCondition(r => r.UserId == userId, trackChanges).ToListAsync();
        public string GetTokenUser (int userId)
         =>  FindByCondition(r => r.UserId == userId, false).OrderByDescending(r => r.Id).FirstOrDefault().DeviceToken;
        public void AddDevice(Device device) => Create(device);
        public void DeleteDevice(Device device) => Delete(device);
    }
}
