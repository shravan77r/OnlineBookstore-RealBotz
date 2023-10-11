using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models
{
    public class TwoFactorAuthenticationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string OTP { get; set; }

        public bool Is2FAEnable { get; set; }
    }
}
