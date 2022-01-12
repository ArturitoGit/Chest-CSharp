using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.PasswordHash.Pipelines;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IsPasswordRegisteredController : ControllerBase
    {
        [HttpGet]
        public async Task<IsPasswordRegistered.Result> Get()
        {
            var result = await ServiceCollection.Handle(new IsPasswordRegistered.Request()) ;
            return result ;
        }
    }
}