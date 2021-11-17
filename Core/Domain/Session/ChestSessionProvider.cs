using Core.Domain.Session.Services;

namespace Core.Domain.Session
{
    public class ChestSessionProvider : IChestSessionProvider
    {
        public static ChestSession INSTANCE = new ChestSession ();
        public ChestSession GetSession() => INSTANCE ;
    }
}