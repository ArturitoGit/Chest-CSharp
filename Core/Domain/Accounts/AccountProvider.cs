using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Domain.Accounts.Services;

namespace Core.Domain.Accounts
{
    public class AccountProvider : IAccountProvider
    {

        private static string DEAULT_ACCOUNTS_FILE_PATH = Path.Combine(Environment.CurrentDirectory, "accounts.json" );

        private string _accountsFile = DEAULT_ACCOUNTS_FILE_PATH;
        public AccountProvider() { }
        public AccountProvider(string accountsFile)
        {
            _accountsFile = accountsFile;
        }

        public async Task AddAccount(ChestAccount account)
        {
            // Build the serialized line
            string json_line = JsonSerializer.Serialize(account);
            // Append it to the end of the file
            using (StreamWriter file = new(_accountsFile, append: true))
            {
                await file.WriteLineAsync(json_line);
            }
        }

        public Task DeleteAllAccounts()
        {
            if (File.Exists(_accountsFile)) File.Delete(_accountsFile);
            return Task.CompletedTask;
        }

        public async Task DeleteAccount(Guid id)
        {
            var accounts = await GetAccounts();
            await DeleteAllAccounts();
            var tasks = accounts
                .Where(a => a.Id != id)
                .Select(a => AddAccount(a));

            await Task.WhenAll(tasks) ;
        }

        public async Task<ChestAccount[]> GetAccounts()
        {
            List<ChestAccount> accounts = new();
            string? line;

            if (!File.Exists(_accountsFile)) return new ChestAccount[] { };

            using (StreamReader file = new(_accountsFile))
            {
                while ((line = await file.ReadLineAsync()) is not null)
                {
                    try
                    {
                        // Deserialize the line
                        var account = JsonSerializer.Deserialize<ChestAccount>(line);
                        // Add it to the list
                        accounts.Add(account!);
                    }
                    catch (JsonException) { }
                }
            }

            return accounts.ToArray();
        }

        public async Task UpdateAccount (ChestAccount account)
        {
            var accounts = await GetAccounts();
            await DeleteAllAccounts();
            var tasks = accounts.Select(a => AddAccount(a.Id != account.Id ? a : account)) ;
            await Task.WhenAll(tasks) ;
        }

        public void Dispose()
        {
            if (File.Exists(_accountsFile)) File.Delete(_accountsFile);
        }

        public async Task ApplyToAllAccounts(Func<ChestAccount, ChestAccount> task)
        {
            var accounts = await GetAccounts() ;
            await DeleteAllAccounts() ;
            var tasks = accounts.Select(a => AddAccount(task(a))) ;
            await Task.WhenAll(tasks) ;
        }
    }
}