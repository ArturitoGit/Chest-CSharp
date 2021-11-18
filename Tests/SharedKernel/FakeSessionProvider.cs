using Core.Domain.Session;
using Core.Domain.Session.Services;

namespace Tests.SharedKernel
{
    public class FakeSessionProvider : IChestSessionProvider
    {
        public static FakeSessionProvider INSTANCE = new FakeSessionProvider() ;
        public static string DEFAULT_PASSWORD = "Password of the chest session" ; 

        public string Password { get; set; } = DEFAULT_PASSWORD ;

        public ChestSession GetSession() => new ChestSession() { IsOpen = true, Password = Password } ;
    }
}