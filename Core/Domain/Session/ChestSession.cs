using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.PasswordHash.Pipelines;

namespace Core.Domain.Session
{
    public class ChestSession
    {
        public string? Password { get; set; }

        public bool IsOpen { get; set; } = false;
    }
}