using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.Models.Enums;

namespace Repository
{
    public class ImageProductRepository : RepositoryBase<ProductImage>, IImageProductRepository
    {
        public ImageProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<ProductImage>> GetAllImagesProduct(int productId, bool trackChanges , bool isIncluded = false)
          {
             var cats = FindByCondition(c => c.ProductId == productId && c.Image.IsStatus == Status.Active, trackChanges);
            if (isIncluded == true)
            {
                cats = cats.Include(c => c.Image);
            }
            return  await cats.ToListAsync();
        }
        public async Task<ProductImage> GetImageProductId( int id, bool trackChanges)
            => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public void CreateImageProduct(ProductImage image) => Create(image);
        public void DeleteImageProduct(ProductImage image) => Delete(image);
    }
}
