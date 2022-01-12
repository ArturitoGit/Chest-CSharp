
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetAccountsController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ChestAccount>> Get()
        {
            var provider = ServiceCollection.GetInstance().GetScope<IAccountProvider>() ;
            var accounts = await provider.GetAccounts() ;
            return accounts ;
        }
    }
}