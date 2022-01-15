using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeleteAccountController : ControllerBase
    {
        [HttpPost]
        public async Task<DeleteAccount.Result> PostAccount (DeleteAccount.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            return result ;
        }
    }
}