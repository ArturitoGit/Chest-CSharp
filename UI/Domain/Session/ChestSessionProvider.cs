using UI.Domain.Session.Services;

namespace UI.Domain.Session
{
    public class ChestSessionProvider : IChestSessionProvider
    {
        public static ChestSession INSTANCE = new ChestSession ();
        public ChestSession GetSession() => INSTANCE ;
    }
}