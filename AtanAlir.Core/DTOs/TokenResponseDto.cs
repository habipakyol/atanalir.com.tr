using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Core.DTOs;

public class TokenResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
