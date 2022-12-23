using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductOptionValueRepository
    {
        Task<ProductOptionValue> GetValueId(int id, bool trackChanges);
         Task<List<ProductOptionValue>> GetValuesOPtionId(int optionId);
        Task<ProductOptionValue> GetOptionValue(int optionId);
        Task<List<ProductOptionValue>> GetValues();
         void CreateValue(ProductOptionValue value) ;
         void DeleteValue(ProductOptionValue value) ;
    }
}
