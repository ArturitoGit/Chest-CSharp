using System.Threading.Tasks;
using UI.Domain.PasswordHash.Services;
using static UI.DependencyInjection.Service;

namespace UI.Domain.PasswordHash.Pipelines
{
    public class IsPasswordRegistered
    {
        public record Request () : IRequest<Result> {}
        public record Result (bool IsPasswordRegistered) : IResult {}

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IPasswordHashProvider _passwordProvider;

            public Handler (IPasswordHashProvider passwordProvider)
            {
                _passwordProvider = passwordProvider ?? throw new System.ArgumentNullException(nameof(passwordProvider));
            }

            public async Task<Result> Handle(Request request) 
            {
                try
                {
                    await _passwordProvider.GetPasswordHash() ;
                    return new Result(true) ;
                }
                catch (NoPasswordStoredException)
                {
                    return new Result(false) ;
                }
            }

        }
    }
}