using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Repository
{
    public class TwoFARepository : ITwoFARepository
    {
        private readonly IUserRepository _iUserRepository;
        public TwoFARepository(IUserRepository IUserRepository)
        {
            _iUserRepository = IUserRepository;
        }
       
        public async Task<Response<string>> Set2FAAuthFlag(string Email, bool Is2FAEnable)
        {
            byte[] bytes;
            Response<string> result = new Response<string>();
            try
            {
                var res = await _iUserRepository.Update2FAFlag(Email, Is2FAEnable,"");

                if (Is2FAEnable)
                {
                    var myObject = await _iUserRepository.GetUserByEmailId(Email);
                    bytes = Encoding.ASCII.GetBytes(Email.ToLower());
                    TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                    if (!string.IsNullOrEmpty(myObject.Data.Email) && (myObject.Data != null))
                    {
                        result.Data = tfa.GenerateSetupCode("LocalHost", myObject.Data.Email.ToLower(), bytes).QrCodeSetupImageUrl;
                        result.Message = "2FA has been enabled and QR-Generated";
                        result.MessageType = "Success";
                        result.Status = 1;
                    }
                }
                else
                {
                    result.Data = Email;
                    result.Message = "2FA has been disabled";
                    result.MessageType = "Success";
                    result.Status = 1;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.MessageType = "Error";
                result.Status = 0;
            }
            return result;
        }
        public async Task<Response<string>> ValidateTwoFactorPIN(string Email, string pin)
        {
            Response<string> result = new Response<string>();
            try
            {
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                byte[] bytes = { };
                string str = "";
                if (!string.IsNullOrEmpty(Email))
                {
                    bytes = Encoding.ASCII.GetBytes(Email.ToLower());
                    str = System.Text.Encoding.Default.GetString(bytes);
                    result.Status = Convert.ToInt32(tfa.ValidateTwoFactorPIN(str, pin));
                }
                if (result.Status == 1)
                {
                    result.Message = "Your verification is completed";
                    result.MessageType = "Success";
                    result.Data = Convert.ToString(Guid.NewGuid());

                    var res = await _iUserRepository.Update2FAFlag(Email, true, result.Data);

                }
                else
                {
                    result.Message = "Sorry, you entered wrong code";
                    result.MessageType = "Error";
                    result.Data = "";
                    result.Status = 0;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.MessageType = "Error";
                result.Status = 0;
            }
            return result;
        }

        public async Task<Response<string>> VerifyLoginOTP2FA(string Email, string Password, string OTP)
        {
            Response<string> result = new Response<string>();
            try
            {
                var user = await _iUserRepository.GetUser(Email, Password);
                if (OTP.Length == 6)
                {
                    byte[] bytes = { };
                    TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                    bytes = Encoding.ASCII.GetBytes(Email.ToLower());
                    var Str = System.Text.Encoding.Default.GetString(bytes);
                    result.Status = Convert.ToInt32(tfa.ValidateTwoFactorPIN(Str, OTP));
                    if (result.Status > 0)
                    {
                        result.Message = "You verification is completed";
                        result.MessageType = "Success";
                    }
                    else
                    {
                        result.Message = "Please Enter a valid Code";
                        result.MessageType = "Error";
                    }
                }
                else if (user.Data.AuthRecoveryCode != OTP)
                {
                    result.Status = 0;
                    result.Message = "Please Enter a valid RecoveryCode";
                    result.MessageType = "Error";
                }
                else
                {
                    result.Status = 1;
                    result.Message = "Your RecoveryCode Is verified";
                    result.MessageType = "Success";
                    
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.MessageType = "Error";
                result.Status = 0;
            }
            return result;
        }
    }
}
