using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Core.DTOs
{
    // AtanAlir.Core/DTOs/EmailVerificationDto.cs
    public class EmailVerificationDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
