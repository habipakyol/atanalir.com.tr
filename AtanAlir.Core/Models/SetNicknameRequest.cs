using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtanAlir.Core.Models
{
    // Core/Models/SetNicknameRequest.cs
    public class SetNicknameRequest
    {
        public string Email { get; set; } = null!;
        public string Nickname { get; set; } = null!;
    }
}
