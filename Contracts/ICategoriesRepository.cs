using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICategoriesRepository
    {
        Task<Category> GetCategoryById(int id, bool trackChanges);
        Task<IEnumerable<Category>> GetAllCategories(bool trackChanges);
        Task<IEnumerable<Category>> GetCategoriesWithMainCategories(bool trackChanges);
        Task<IEnumerable<Category>> GetSubCategoriesByMainId(int id, bool trackChanges);
        Task<IEnumerable<Category>> GetMainCategories(bool trackChanges);
        Task<IEnumerable<Category>> SearchMainCategoriesStoreId(int storeId, int categoryId, string search);
        Task<IEnumerable<Category>> SearchSubCategories(int categoryId, string search);
        Task<IEnumerable<Category>> SearchMainCategories(string search);
        void CreateMainCategory(Category category);
        void CreateSubCategory(int mainId, Category subCategory);
        void DeleteCategory(Category category);
        Task DeleteSupCategories(List<int> Ids);
        Task<Category> GetCategoryToPrductId(int productId);
    }
}
