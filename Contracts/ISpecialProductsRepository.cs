using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISpecialProductsRepository
    {
        Task<SpecialProducts> GetSpecialProductId(int productId);
        Task<SpecialProducts> CheckSpecialExists(int productId, bool trackChanges);
        Task<IEnumerable<SpecialProducts>> GetSpecialProductsProductId(int productId, bool trackChanges);
        Task<PagedList<SpecialProducts>> SpecialsPage(PostsParameters postsParameters, bool trackChanges);
        void AddSpecialProduct(SpecialProducts special);
        void DeleteSpecialProduct(SpecialProducts special);
        Task DeleteListSpecialProducts(List<int> Ids);
        void CreateListSpecialProducts(List<SpecialProducts> entity);
        Task<SpecialProducts> GetSpecialId(int id, bool trackChanges);
    }
}
