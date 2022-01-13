using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts.Pipelines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EditAccountController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<UpdateAccount.Result>> PostAccount (UpdateAccount.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            return result ;
        }
    }
}