
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Pipelines;
using Core.Domain.Accounts.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddAccountController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<RegisterAccount.Result>> PostPassword (RegisterAccount.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            return result ;
        }
    }
}