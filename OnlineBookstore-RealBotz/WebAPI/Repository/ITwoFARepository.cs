
using Model.Models;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Repository
{
    public interface ITwoFARepository
    {
        Task<Response<string>> Set2FAAuthFlag(string Email,bool Is2FAEnable);
        Task<Response<string>> ValidateTwoFactorPIN(string Email, string pin);
        Task<Response<string>> VerifyLoginOTP2FA(string Email, string Password, string OTP);
    }
}
