using System;
using System.Threading.Tasks;
using UI.Domain.Accounts;
using UI.Domain.Accounts.Services;
using UI.Domain.Crypto;
using UI.Domain.Crypto.Services;
using UI.Domain.PasswordHash;
using UI.Domain.PasswordHash.Services;
using UI.Domain.Session;
using UI.Domain.Session.Services;
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static UI.DependencyInjection.ServiceCollection ;

namespace UI
{
    public class Startup
    {
        public static Boolean DEBUG = true ; 

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Use what is inside wwwroot folder as content of the website
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Subscribe the project to the ui requests
            RegisterServices() ;
            UIRequestHandler.Subscribe() ;

            // Open the electron window here
            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }

        public void RegisterServices ()
        {
            GetInstance().RegisterScope<ICryptoAgent, CryptoAgent>();
            GetInstance().RegisterScope<IAccountProvider, AccountProvider>();
            GetInstance().RegisterScope<IPasswordHashProvider, PasswordHashProvider>();
            GetInstance().RegisterScope<IChestSessionProvider, ChestSessionProvider>();
            GetInstance().RegisterScope<IPasswordChecker, PasswordChecker>();

            Console.WriteLine("Dependencies registered !") ;
        }
    }
}
