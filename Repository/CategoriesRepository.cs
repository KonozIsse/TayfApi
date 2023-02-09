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
        public async Task<IEnumerable<Category>> GetSubCategoriesByMainId(int id, bool trackChanges)
         => await FindByCondition(c => c.MainCategoryId == id && c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> GetSubCategoriesMainIDCP (int mainId)
        => await FindByCondition(c => c.MainCategoryId == mainId , false).OrderByDescending(x => x.CreatedAt).ToListAsync();
        public async Task<IEnumerable<Category>> GetSubActiveCategories(bool trackChanges)
      => await FindByCondition(c => c.MainCategoryId != 0 && c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> GetAllActiveMainCategories(bool trackChanges)
        => await FindByCondition(c => c.MainCategoryId == 0 && c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<IEnumerable<Category>> GetAllMainCategories()
        => await FindByCondition(c => c.MainCategoryId == 0 , false).ToListAsync();
        public async Task<IEnumerable<Category>> SearchSubCategories(int mainId , string search)
        {
             var query = FindByCondition(c => c.MainCategoryId == mainId && c.CategoryName.Contains(search), false);
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
        public async Task<IEnumerable<Category>> SearchMainCategoriesCP(string search)
        {
            var query = FindByCondition(c => c.MainCategoryId == 0 && c.CategoryName.Contains(search), false);
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
        public void CreateMainCategory(Category category) => Create(category);
        public void DeleteCategory(Category category) => Delete(category);
       
      

    }
}
