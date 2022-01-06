using System.Threading.Tasks;
using UI.DependencyInjection;
using UI.Domain.PasswordHash.Pipelines;

namespace UI.Domain.Session
{
    public class ChestSession
    {
        public string? Password { get; set; }

        public bool IsOpen { get; set; } = false;
    }
}