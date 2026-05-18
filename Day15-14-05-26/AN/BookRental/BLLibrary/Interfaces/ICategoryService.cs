using ModelLibrary.Models;
using System.Collections.Generic;

namespace BLLibrary.Interfaces
{
    public interface ICategoryService
    {
        void AddCategory(BookCategory category);
        void UpdateCategory(int categoryId, string categoryName);
        void DeleteCategory(int categoryId);
        BookCategory? GetCategoryById(int categoryId);
        IEnumerable<BookCategory> GetAllCategories();
    }
}
