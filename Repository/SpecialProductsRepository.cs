using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.RequestFeatures;

namespace Repository
{
    public class SpecialProductsRepository : RepositoryBase<SpecialProducts>, ISpecialProductsRepository
    {
        public SpecialProductsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<SpecialProducts> GetSpecialProductId(int productId)
         => await FindByCondition(c => c.ProductId == productId && c.EndDate > DateTime.Now && c.IsStatus == Status.Active, false).FirstOrDefaultAsync();
        public async Task<SpecialProducts> CheckSpecialExists(int productId , bool trackChanges)
        => await FindByCondition(c => c.ProductId == productId , trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<SpecialProducts>> GetSpecialProductsProductId(int productId, bool trackChanges)
       => await FindByCondition(c => c.ProductId== productId, trackChanges).ToListAsync();
        public async Task<PagedList<SpecialProducts>> SpecialsPage(PostsParameters postsParameters, bool trackChanges)
        {
            var special = await FindByCondition(c => c.IsStatus == Status.Active && c.EndDate > DateTime.Now, trackChanges)
                .Include(i => i.Product).ThenInclude(r => r.Reviews).OrderByDescending(p => p.ProductId).ToListAsync();
            return PagedList<SpecialProducts>.ToPagedList(special, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public void AddSpecialProduct(SpecialProducts special) => Create(special);
        public void DeleteSpecialProduct(SpecialProducts special) => Delete(special);

        public void CreateListSpecialProducts(List<SpecialProducts> entity) => CreateRange(entity);
        public async Task DeleteListSpecialProducts(List<int> Ids)
        {
            var result = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(result);
        }
    }
}
