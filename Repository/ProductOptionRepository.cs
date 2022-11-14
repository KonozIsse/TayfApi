using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class ProductOptionRepository : RepositoryBase<ProductOption>, IProductOptionRepository
    {
        public ProductOptionRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<ProductOption>> GetAllOptions()
        => await FindAll(false).ToListAsync();
        public async Task<ProductOption> GetOptionId (int id, bool trackChanges)
        => await FindByCondition(x => x.Id == id, trackChanges).FirstOrDefaultAsync();
        public void CreateOption(ProductOption Option) => Create(Option);
        public void DeleteOption(ProductOption Option) => Delete(Option);
    }
}
