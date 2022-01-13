using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.PasswordHash.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DecryptPasswordController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<DecryptPassword.Result>> PostPassword (DecryptPassword.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            return result ;
        }
    }
}