using System;

namespace Core.Domain.PasswordHash
{
    public class PasswordHash
    {
        public Guid Id { get; set; }
        public string Hash { get; set; } = null!;
    }
}