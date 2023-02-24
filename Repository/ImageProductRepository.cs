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
        public async Task<IEnumerable<ProductImage>> GetAllImagesProductId(int productId, bool trackChanges , bool isIncluded = false)
          {
             var cats = FindByCondition(c => c.ProductId == productId , trackChanges);
            if (isIncluded == true)
            {
                cats = cats.Include(c => c.Image);
            }
            return  await cats.ToListAsync();
        }
        public async Task<IEnumerable<ProductImage>> GetAllProductsImageId(int imageId, bool trackChanges)
        {
            var cats = FindByCondition(c => c.ImageId == imageId, trackChanges);
            return await cats.ToListAsync();
        }
        public async Task<ProductImage> GetImageProductId( int id, bool trackChanges)
            => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public void CreateImageProduct(ProductImage image) => Create(image);
        public void DeleteImageProduct(ProductImage image) => Delete(image);

        public async Task DeleteRowRange(List<int> Ids)
        {
            var result = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(result);
        }
        public void CreatProductCategoryRange(List<ProductImage> productCategory) => CreateRange(productCategory);
    }
}
