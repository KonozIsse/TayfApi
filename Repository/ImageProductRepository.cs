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
    public class ImageProductRepository : RepositoryBase<ProductImage>, IImageProductRepository
    {
        public ImageProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<ProductImage>> GetImagesProduct(int productId, bool trackChanges)
            => await FindByCondition(c => c.ProductId == productId, trackChanges).ToListAsync();
        public void CreateImageProduct(ProductImage image) => Create(image);
        public void DeleteImageProduct(ProductImage image) => Delete(image);
    }
}
