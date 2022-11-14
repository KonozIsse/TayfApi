using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IProductOptionRepository
    {
        Task<List<ProductOption>> GetAllOptions();
        Task<ProductOption> GetOptionId(int id, bool trackChanges);
        void CreateOption(ProductOption Option);
        void DeleteOption(ProductOption Option);
    }
}
