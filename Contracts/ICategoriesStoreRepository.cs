using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICategoriesStoreRepository
    {
        Task<IEnumerable<CategoriesStore>> GetCategoriesStoreId(int storeId);
        Task<IEnumerable<CategoriesStore>> GetStoresCategoryId(int categoryId);
    }
}
