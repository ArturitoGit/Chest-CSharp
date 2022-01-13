using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chest.Core;
using Chest.Core.DependencyInjection;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service;
using static Core.Domain.Accounts.Pipelines.RegisterAccount;

namespace Core
{
    public class Program
    {
        public static void Main (string[] args)
        {   
            OpenUrlInBrowser(null!) ;
        }

        public static void OpenUrlInBrowser (string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
