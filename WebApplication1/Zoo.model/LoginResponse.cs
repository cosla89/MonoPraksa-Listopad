using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.model
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";

        public DateTime ExpiresAtUtc { get; set; }

        public string Role { get; set; } = "";
    }
}
