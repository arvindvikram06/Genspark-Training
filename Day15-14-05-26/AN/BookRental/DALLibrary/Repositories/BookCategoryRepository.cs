using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using ModelLibrary.Models;
using System.Collections.Generic;

namespace DALLibrary.Repositories
{
    public class BookCategoryRepository : GenericRepository<BookCategory>, IBookCategoryRepository
    {
        public BookCategoryRepository(BookRentalContext context) : base(context)
        {
        }
    }
}
