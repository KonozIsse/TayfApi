using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAddressRepository
    {
        Task<Address> GetAddress(int id, bool trackChanges);
        Task<List<Address>> GetAllAddressesByCustomerId(int customerId);
        Task<Address> GetAddressIdByCustomerId(int id, int customerId , bool trackChanges);
        Task<Address> GetAddressCustomer(int customerId);
        void AddAddress(Address address);
        void DeleteAddress(Address address);
    }
}
