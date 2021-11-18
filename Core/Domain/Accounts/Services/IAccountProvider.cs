using System;
using System.Threading.Tasks;

namespace Core.Domain.Accounts.Services
{

    public interface IAccountProvider
    {
        Task<ChestAccount[]> GetAccounts();

        Task AddAccount(ChestAccount account);

        Task DeleteAccount(Guid id);

        Task UpdateAccount(ChestAccount account);

        Task DeleteAllAccounts();

        Task ApplyToAllAccounts(Func<ChestAccount, ChestAccount> task) ;
    }
}