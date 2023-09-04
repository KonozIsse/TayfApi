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
        Task<IEnumerable<Category>> GetAllActiveMainCategories(bool trackChanges);
        Task<IEnumerable<Category>> GetSubActiveCategories(bool trackChanges);
        Task<IEnumerable<Category>> GetSubCategoriesByMainId(int id, bool trackChanges);
        Task<IEnumerable<Category>> SearchSubCategories(int mainId, string search);
        Task<IEnumerable<Category>> SearchMainCategoriesCP(string search);
        void CreateMainCategory(Category category);
        void DeleteCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategoriesImageId(int imagId, bool trackChanges);
    }
}
