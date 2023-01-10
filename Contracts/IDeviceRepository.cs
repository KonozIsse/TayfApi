using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDeviceRepository
    {
        Task<Device> CheckDevice(string token, bool trackChanges);
        Task<List<Device>> GetDevicesUserId(int userId, bool trackChanges);
        Task<Device> GetDeviceUser(int deviceId, int userId, bool trackChanges);
        string GetTokenUser(int userId);
        Task<Device> GetDeviceUserId(int userId);
        void AddDevice(Device device);
        void DeleteDevice(Device device);
    }
}
