using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Generator.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneratePasswordController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<GeneratePassword.Result>> Post (GeneratePassword.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            return result ;
        }
    }
}