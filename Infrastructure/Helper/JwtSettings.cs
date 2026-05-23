using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Helper
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationHours { get; set; } = 1;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
