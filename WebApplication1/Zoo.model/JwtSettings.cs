using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.model
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "";

        public string Audience { get; set; } = "";

        public string Key { get; set; } = "";

        public int ExpirationMinutes { get; set; }
    }
}
