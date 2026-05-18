using ModelLibrary.Models;
using System.Collections.Generic;

namespace BLLibrary.Interfaces
{
    public interface IFineService
    {
        IEnumerable<Fine> GetFinesForMember(int memberId);
        void PayFine(int fineId);
    }
}
