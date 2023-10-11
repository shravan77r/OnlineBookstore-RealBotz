using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebAPI.Auth;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    
    [Route("api/Account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IJwtAuth jwtAuth;
        private readonly IUserRepository _repo;
        private readonly ITwoFARepository _repoTwoFAR;
        public AccountController(IUserRepository repository, IJwtAuth jwtAuth, ITwoFARepository repoTwoFAR)
        {
            this.jwtAuth = jwtAuth;
            _repo = repository;
            _repoTwoFAR = repoTwoFAR;
        }
        [AllowAnonymous]
        [HttpPost("authentication")]
        public IActionResult Authentication(LoginModel userCredential)
        {
            var token = jwtAuth.Authentication(userCredential.Email, userCredential.Password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }

        [HttpPost("Login")]
        public async Task<Response<User>> Login(LoginModel request)
        { 
            return await _repo.GetUser(request.Email, request.Password);
        }
        [HttpPost("VerifyLoginOTP2FA")]
        public async Task<Response<string>> VerifyLoginOTP2FA(TwoFactorAuthenticationModel request)
        {
            return await _repoTwoFAR.VerifyLoginOTP2FA(request.Email, request.Password, request.OTP);
        }

        [HttpPost("Signup")]
        public async Task<Response<int>> Signup(User request)
        {
            return await _repo.Add(request);
        }
        [HttpPost("CheckEmailIdExist/{Email}")]
        public async Task<Response<User>> CheckEmailIdExist(string Email)
        {
            return await _repo.GetUserByEmailId(Email);
        }

        [HttpPost("UpdateProfile")]
        public async Task<Response<int>> UpdateProfile(User request)
        {
            return await _repo.UpdateProfile(request);
        }

        [HttpPost("GetCountry")]
        public async Task<Response<List<Country>>> GetCountries()
        {
            return await _repo.GetCountries();
        }
        [HttpPost("GetState/{cid}")]
        public async Task<Response<List<State>>> GetState(int cid)
        {
            return await _repo.GetState(cid);
        }
        [HttpPost("GetCity/{sid}")]
        public async Task<Response<List<City>>> GetCity(int sid)
        {
            return await _repo.GetCity(sid);
        }

        #region 2 Factor Authenticator

        [HttpPost("Set2FAAuthFlag")]
        public async Task<Response<string>> Set2FAAuthFlag(TwoFactorAuthenticationModel request)
        {
            return await _repoTwoFAR.Set2FAAuthFlag(request.Email, request.Is2FAEnable);
        }
        [HttpPost("ValidateTwoFactorPIN")]
        public async Task<Response<string>> ValidateTwoFactorPIN(TwoFactorAuthenticationModel request)
        {
            return await _repoTwoFAR.ValidateTwoFactorPIN(request.Email, request.OTP);
        }
        #endregion 2 Factor Authenticator
    }
}
