using ModelLibrary.Models;

namespace BLLibrary.Interfaces
{
    public interface IMembershipService
    {
        void CreateMembership(string name, decimal price, int borrowLimit, int borrowDaysLimit);
        void UpdateMembership(int id, string name, decimal price, int borrowLimit, int borrowDaysLimit);
        
        void ActivateMembership(int id);
        void DeactivateMembership(int id);
        Membership GetMembershipById(int id);
        IEnumerable<Membership> GetAllMemberships();
        void PurchaseMembership(int memberId, int membershipId);
    }
}
