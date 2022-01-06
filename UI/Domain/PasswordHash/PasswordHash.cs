using System;

namespace UI.Domain.PasswordHash
{
    public class PasswordHash
    {
        public Guid Id { get; set; }
        public string Hash { get; set; } = null!;
    }
}