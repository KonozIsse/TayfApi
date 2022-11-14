using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CategoriesRepository : RepositoryBase<Category>, ICategoriesRepository
    {
        public CategoriesRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Category> GetCategoryById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();
        public async Task<IEnumerable<Category>> GetAllCategories(bool trackChanges)
        => await FindByCondition(c => c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> GetCategoriesWithMainCategories(bool trackChanges)
        => await FindByCondition(c => c.Id != 1 && c.MainCategoryId == 1 && c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> GetSubCategoriesByMainId(int id, bool trackChanges)
         => await FindByCondition(c => c.MainCategoryId == id && c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> GetMainCategories(bool trackChanges)
        => await FindByCondition(c => c.MainCategoryId == 1 && c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> SearchSubCategories(int categoryId, string search)
        {
            var query = FindByCondition(c => c.MainCategoryId == categoryId && c.IsStatus == Status.Active, false);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.CategoryName.Contains(search));
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Category>> SearchMainCategoriesStoreId(int storeId ,int categoryId, string search)
        {
            var query = FindByCondition(c => c.Id == categoryId || c.MainCategoryId == categoryId && c.IsStatus == Status.Active
           && c.Products.Any(x=>x.IsStatus == Status.Active) && c.CategoriesStores.Any(x=>x.VendorId== storeId), false);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.CategoryName.Contains(search));
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Category>> SearchMainCategories(string search)
        {
            var query = FindByCondition(c => c.MainCategoryId == 1 && c.IsStatus == Status.Active, false);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.CategoryName.Contains(search));
            }
            return await query.ToListAsync();
        }
        public void CreateMainCategory(Category category) => Create(category);
        public void CreateSubCategory(int mainId, Category subCategory)
        {
            subCategory.MainCategoryId = mainId;
            Create(subCategory);
        }
        public void DeleteCategory(Category category) => Delete(category);
        public async Task DeleteSupCategories(List<int> Ids)
        {
            var result = await FindByCondition(c => Ids.Contains(c.Id), true).ToListAsync();
            DeleteRange(result);
        }
        public async Task<Category> GetCategoryToPrductId (int productId)
        => await FindByCondition(c => c.Products.Any(x=>x.Id == productId), false).SingleOrDefaultAsync();
      

    }
}
