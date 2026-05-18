using ModelLibrary.Models;
using System.Collections.Generic;

namespace DALLibrary.Interfaces
{
    public interface IBookDamageHistoryRepository : IGenericRepository<BookDamageHistory>
    {
        IEnumerable<BookDamageHistory> GetDamageHistoryByBookCopyId(int copyId);
    }
}
