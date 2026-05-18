using ModelLibrary.Models;
using BLLibrary.Interfaces;
using DALLibrary.Interfaces;
using BLLibrary.Exceptions;
using System.Collections.Generic;
using DALLibrary.Contexts;
using DALLibrary.Exceptions;
using System;

namespace BLLibrary.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBookCategoryRepository _categoryRepository;
        private readonly BookRentalContext _context;

        public CategoryService(IBookCategoryRepository categoryRepository, BookRentalContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public void AddCategory(BookCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ValidationException("Category name cannot be empty.");
            }
            try
            {
                _categoryRepository.Add(category);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Unable to add category.", ex);
            }
        }

        public void UpdateCategory(int categoryId, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ValidationException("Category name cannot be empty.");
            }

            var category = _categoryRepository.GetById(categoryId);
            if (category == null)
            {
                throw new DataNotFoundException("Category", categoryId);
            }

            try
            {
                category.CategoryName = categoryName;
                _categoryRepository.Update(category);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Unable to update category.", ex);
            }
        }

        public void DeleteCategory(int categoryId)
        {
            var category = _categoryRepository.GetById(categoryId);
            if (category == null)
            {
                throw new DataNotFoundException("Category", categoryId);
            }

            try
            {
                _categoryRepository.Delete(category);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Unable to delete category. Ensure no books belong to this category first.", ex);
            }
        }

        public BookCategory? GetCategoryById(int categoryId)
        {
            return _categoryRepository.GetById(categoryId);
        }

        public IEnumerable<BookCategory> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }
    }
}
