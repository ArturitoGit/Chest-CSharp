using System;
using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.PasswordHash.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckPasswordController : ControllerBase
    {
        [HttpPost]
        public async Task<CheckPassword.Result> PostPassword (CheckPassword.Request request)
        {
            var result = await ServiceCollection.Handle(request) ;
            Console.WriteLine(result) ;
            return result ;
        }
    }
}