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
    public class ProductAttributRepository : RepositoryBase<ProductAttribut>, IProductAttributRepository
    {
        public ProductAttributRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<ProductAttribut> GetAttributeId (int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<ProductAttribut> GetAttributeIdProductId(int id, int productId)
          => await FindByCondition(c => c.Id == id && c.ProductId == productId, false).FirstOrDefaultAsync();
        public async Task<List<ProductAttribut>> GetAttributesProductId (int productId)
         =>await FindByCondition(c => c.ProductId == productId, false).Include(c=>c.Product)
            .Include(c=>c.ProductOption).Include(c=>c.ProductOptionValue).ToListAsync();
        public async Task<List<ProductAttribut>> GetAllAttributes()
        => await FindAll(false).Include(c => c.Product)
           .Include(c => c.ProductOption).Include(c => c.ProductOptionValue).ToListAsync();
        public async Task<List<ProductAttribut>> GetAttributesOptionId(int optionId)
         => await FindByCondition(c => c.OptionId == optionId, false).ToListAsync();
         public async Task<ProductAttribut> GetProductOptionValue(int productId, int optionId, int valueId)
         => await FindByCondition(c => c.ProductId == productId && c.OptionId == optionId && c.ValueId == valueId, false).FirstOrDefaultAsync();

         public void AddAttributesProduct(int productId, ProductAttribut option)
        {
            option.ProductId = productId;
            Create(option); 
        }
        public void DeleteAttributesProduct(ProductAttribut attributes) => Delete(attributes);

        
    }
    
}
