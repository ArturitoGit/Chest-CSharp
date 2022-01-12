using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Session.Pipelines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenChestController : ControllerBase
    {

        private readonly ILogger<OpenChestController> _logger ;

        public OpenChestController (ILogger<OpenChestController> logger)
        {
            _logger = logger ;
        }

        [HttpPost]
        public async Task<ActionResult<OpenChestSession.Result>> PostPassword (OpenChestSession.Request request)
        {
            // Log the parameter
            _logger.LogInformation("Password received : " + request.Password) ;

            // Try to open the chest
            var result = await ServiceCollection.Handle(request) ;

            // Return the result
            return result ;
        }
    }
}