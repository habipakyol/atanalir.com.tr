using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Core.Entities
{
    // AtanAlir.Core/Entities/VerificationCode.cs
    public class VerificationCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
