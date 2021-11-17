using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;

namespace Tests.SharedKernel
{
    public class FakeAccountProvider : IAccountProvider
    {
        private List<ChestAccount> _accounts = new() ;

        public Task AddAccount(ChestAccount account)
        {
            _accounts.Add(account) ;
            return Task.CompletedTask ;
        }

        public Task DeleteAccount(Guid id)
        {
            _accounts.Remove(_accounts.Where(a => a.Id == id).First()) ;
            return Task.CompletedTask ;
        }

        public Task DeleteAllAccounts()
        {
            _accounts = new List<ChestAccount>() ;
            return Task.CompletedTask ;
        }

        public Task<ChestAccount[]> GetAccounts()
        {
            return Task.FromResult(_accounts.ToArray()) ;
        }

        public Task UpdateAccount(ChestAccount account)
        {
            var local = _accounts.Where(a => a.Id == account.Id).First() ;
            if (local is null) return Task.CompletedTask ;

            local.Name = account.Name ;
            local.HashedPassword = account.HashedPassword ;
            local.IV = account.IV ;
            local.Salt = account.Salt ;

            return Task.CompletedTask ; 
        }

        public static FakeAccountProvider INSTANCE = new FakeAccountProvider() ;
    }
}