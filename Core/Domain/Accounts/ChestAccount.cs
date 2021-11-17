using System;
using Chest.Core.Infrastructure;

namespace Core.Domain.Accounts
{
    public class ChestAccount
    {
        public Guid Id { get; set; }
        public byte[] HashedPassword { get; set; } = null!;
        public string Name { get; set; } = null!;

        public byte[] Salt { get; set; } = null!;

        public byte[] IV { get; set; } = null!;

        private ChestAccount() { }
        public ChestAccount(byte[] hashedPassword, string name, byte[] salt, byte[] iv)
        {
            Id = Guid.NewGuid();
            HashedPassword = hashedPassword;
            Name = name;
            Salt = salt;
            IV = iv;
        }
    }

    public class AccountNameNotNull : IValidator<ChestAccount>
    {
        public (bool IsValid, string Error) validate(ChestAccount target)
        {
            if (string.IsNullOrWhiteSpace(target.Name)) return (false, "Name of Account must not be null");
            return (true, string.Empty);
        }
    }

    public class AccountIDNotNull : IValidator<ChestAccount>
    {
        public (bool IsValid, string Error) validate(ChestAccount target)
        {
            if (target.Id == Guid.Empty) return (false, "Id of Account must not be null");
            return (true, string.Empty);
        }
    }

    public class AccountPasswordNotNull : IValidator<ChestAccount>
    {
        public (bool IsValid, string Error) validate(ChestAccount target)
        {
            if (target.HashedPassword is null || target.HashedPassword.Length <= 0) return (false, "Password of Account must not be null");
            return (true, string.Empty);
        }
    }

    public class AccountSaltNotNull : IValidator<ChestAccount>
    {
        public (bool IsValid, string Error) validate(ChestAccount target)
        {
            if (target.Salt is null || target.Salt.Length <= 0) return (false, "Salt must not be empty");
            return (true, string.Empty);
        }
    }
}