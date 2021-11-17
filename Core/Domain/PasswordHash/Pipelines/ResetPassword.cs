using System.Threading.Tasks;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.PasswordHash.Pipelines
{
    public class ResetPassword
    {
        public record Request () : IRequest<Result> {}
        public record Result () : IResult {}

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IPasswordHashProvider _passwordProvider;

            public Handler (IPasswordHashProvider passwordProvider)
            {
                _passwordProvider = passwordProvider ?? throw new System.ArgumentNullException(nameof(passwordProvider));
            }

            public async Task<Result> Handle(Request request)
            {
                await _passwordProvider.ResetPasswordHash() ;
                return new Result() ;
            }
        }
    }
}