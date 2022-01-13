using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.PasswordHash.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetPasswordController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<SetPassword.Result>> PostPassword (SetPassword.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            return result ;
        }
    }
}