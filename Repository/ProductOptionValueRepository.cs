using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ProductOptionValueRepository : RepositoryBase<ProductOptionValue>, IProductOptionValueRepository
    {
        public ProductOptionValueRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<ProductOptionValue> GetValueId(int id, bool trackChanges)
        => await FindByCondition(x => x.Id == id, trackChanges).SingleOrDefaultAsync();
        public async Task<ProductOptionValue> GetOptionValue(int optionId)
       => await FindByCondition(x => x.Id == optionId, false).FirstOrDefaultAsync();

        public async Task<List<ProductOptionValue>> GetValuesOPtionId(int optionId)
        => await FindByCondition(x => x.OptionId == optionId, false).Include(c=>c.ProductOption).ToListAsync();
        public async Task<List<ProductOptionValue>> GetValues()
       => await FindAll(false).ToListAsync();
        public void CreateValue(ProductOptionValue value) => Create(value);
        public void DeleteValue(ProductOptionValue value) => Delete(value);
    }
}
