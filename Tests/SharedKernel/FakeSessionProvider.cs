using Core.Domain.Session;
using Core.Domain.Session.Services;

namespace Tests.SharedKernel
{
    public class FakeSessionProvider : IChestSessionProvider
    {
        public static FakeSessionProvider INSTANCE = new FakeSessionProvider() ;
        public static string PASSWORD = "Password of the chest session" ; 
        public ChestSession GetSession() => new ChestSession() { IsOpen = true , Password = PASSWORD } ;
    }
}