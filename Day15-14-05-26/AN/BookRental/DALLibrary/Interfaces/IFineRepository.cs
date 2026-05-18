using ModelLibrary.Models;
using System.Collections.Generic;

namespace DALLibrary.Interfaces
{
    public interface IFineRepository : IGenericRepository<Fine>
    {
        IEnumerable<Fine> GetFinesByMemberId(int memberId);
    }
}
