using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model.Models.Common;
using OnlineBookstore_RealBotz.Models.Common;

namespace OnlineBookstore_RealBotz.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        private APIManager _apiManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public AccountController(ILogger<AccountController> logger, 
            IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _apiManager = new APIManager(configuration);
            _webHostEnvironment = webHostEnvironment;
            
        }
        public IActionResult Index(string message)
        {
            return File("~/index.html?message=" + message, "text/html");
        }

        #region Login/Logout
        [HttpPost("Logout")]
        public async Task<JsonResult> Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            return Json("Session abandoned");
        }

        [HttpPost("Login")]
        public async Task<JsonResult> Login(LoginModel obj)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var resp = JsonConvert.DeserializeObject<Response<User>>(
                    await _apiManager.CallPostMethod("Account/Login", obj, jwtToken));

            return Json(resp);
        }
        [HttpPost("VerifyLoginOTP2FA")]
        public async Task<JsonResult> VerifyLoginOTP2FA(TwoFactorAuthenticationModel obj)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var resp = JsonConvert.DeserializeObject<Response<string>>(
                    await _apiManager.CallPostMethod("Account/VerifyLoginOTP2FA", obj, jwtToken));

            return Json(resp);
        }
        [HttpPost("SetSession")]
        public async Task<JsonResult> SetSession(LoginModel obj)
        {
            var resp = new Response<User>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            try
            {
                var jwtToken = "";
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
                {
                    jwtToken = await _apiManager.GenerateToken();
                    HttpContext.Session.SetString("jwtToken", jwtToken);
                }
                else
                {
                    jwtToken = HttpContext.Session.GetString("jwtToken");
                }

                var baseResponse = JsonConvert.DeserializeObject<Response<User>>(
                    await _apiManager.CallPostMethod("Account/Login", obj, jwtToken));

                if (baseResponse != null)
                {
                    resp = baseResponse;
                }

                if (baseResponse.Data != null)
                {
                    var result = baseResponse.Data;
                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(result.Id));
                    HttpContext.Session.SetInt32("Is2FAEnable", Convert.ToInt32(result.Is2FAEnable));
                    var UserName = Convert.ToString(result.FirstName) + " " + Convert.ToString(result.LastName);
                    HttpContext.Session.SetString("UserName", UserName);
                    HttpContext.Session.SetString("Email", Convert.ToString(result.Email));
                    HttpContext.Session.SetString("Password", Convert.ToString(result.Password));
                    string FileName = Convert.ToString(result.ProfileImage);
                    HttpContext.Session.SetString("ProfilePhoto", "/Uploads/" + Convert.ToInt32(result.Id) + "/" + FileName);
                    HttpContext.Session.SetInt32("CartCount", Convert.ToInt32(result.CartCount));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                resp.MessageType = "Error";
                resp.Message = "Something Went Wrong!";
            }
            return Json(resp);
        }
        #endregion  Login/Logout

        #region Signup
        [HttpPost("Signup")]
        public async Task<JsonResult> Signup(User obj, [FromForm] IFormFile profileImage)
        {
            var resp = new Response<int>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            try
            {
                if (profileImage != null && profileImage.Length > 0)
                {
                    obj.ProfileImage = profileImage.FileName;
                }

                var jwtToken = "";
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
                {
                    jwtToken = await _apiManager.GenerateToken();
                    HttpContext.Session.SetString("jwtToken", jwtToken);
                }
                else
                {
                    jwtToken = HttpContext.Session.GetString("jwtToken");
                }

                var baseResponse = JsonConvert.DeserializeObject<Response<int>>(
                   await _apiManager.CallPostMethod("Account/Signup", obj, jwtToken));

                if (baseResponse != null)
                {
                    resp = baseResponse;
                }

                if (baseResponse.Data > 0)
                {
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads/" + baseResponse.Data);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = Path.GetFileName(profileImage.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                resp.MessageType = "Error";
                resp.Message = "Something Went Wrong!";
            }
            return Json(resp);

        }
        [HttpPost("CheckEmailIdExist")]
        public async Task<JsonResult> CheckEmailIdExist(string Email)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var data = JsonConvert.DeserializeObject<Response<User>>(
                  await _apiManager.CallPostMethod("Account/CheckEmailIdExist/" + Email, null, jwtToken));

            if (data.Data != null)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost("UpdateProfile")]
        public async Task<JsonResult> UpdateProfile(User obj, [FromForm] IFormFile profileImage)
        {
            var resp = new Response<int>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            try
            {
                obj.Id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                if (profileImage != null && profileImage.Length > 0)
                {
                    obj.ProfileImage = profileImage.FileName;
                }

                var jwtToken = "";
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
                {
                    jwtToken = await _apiManager.GenerateToken();
                    HttpContext.Session.SetString("jwtToken", jwtToken);
                }
                else
                {
                    jwtToken = HttpContext.Session.GetString("jwtToken");
                }

                var baseResponse = JsonConvert.DeserializeObject<Response<int>>(
                   await _apiManager.CallPostMethod("Account/UpdateProfile", obj, jwtToken));

                if (baseResponse != null)
                {
                    resp = baseResponse;
                }

                if (baseResponse.Data > 0)
                {
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads/" + baseResponse.Data);
                    if (!string.IsNullOrEmpty(obj.OldProfileImage))
                    {
                        var filePath = Path.Combine(path, obj.OldProfileImage);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = Path.GetFileName(profileImage.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }
                    HttpContext.Session.SetString("ProfilePhoto", "/Uploads/" + Convert.ToInt32(baseResponse.Data) + "/" + fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                resp.MessageType = "Error";
                resp.Message = "Something Went Wrong!";
            }
            return Json(resp);

        }

        #endregion Signup

        #region Dropdown Binding

        [HttpGet("GetCountry")]
        public async Task<JsonResult> GetCountry()
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var data = JsonConvert.DeserializeObject<Response<List<Country>>>(
                   await _apiManager.CallPostMethod("Account/GetCountry", null, jwtToken));

            return Json(data);
        }
        [HttpGet("GetState")]
        public async Task<JsonResult> GetState(int cid)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var data = JsonConvert.DeserializeObject<Response<List<State>>>(
                  await _apiManager.CallPostMethod("Account/GetState/" + cid, null, jwtToken));

            return Json(data);
        }
        [HttpGet("GetCity")]
        public async Task<JsonResult> GetCity(int sid)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var data = JsonConvert.DeserializeObject<Response<List<City>>>(
                 await _apiManager.CallPostMethod("Account/GetCity/"+ sid, null, jwtToken));

            return Json(data);
        }

        #endregion Dropdown Binding

        #region 2 Factor Authenticator

        [HttpGet("Set2FAAuthFlag")]
        public async Task<Response<string>> Set2FAAuthFlag(bool Flag)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var request = new TwoFactorAuthenticationModel();
            request.Email = HttpContext.Session.GetString("Email");
            request.Is2FAEnable = Flag;

            var baseResponse = JsonConvert.DeserializeObject<Response<string>>(
                    await _apiManager.CallPostMethod("Account/Set2FAAuthFlag", request, jwtToken));
            if (baseResponse.Status > 0)
            {
                HttpContext.Session.SetInt32("Is2FAEnable", Convert.ToInt32(Flag));
            }
            return baseResponse;
        }
        [HttpPost("ValidateTwoFactorPIN")]
        public async Task<Response<string>> ValidateTwoFactorPIN(string pin)
        {
            var jwtToken = "";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwtToken")))
            {
                jwtToken = await _apiManager.GenerateToken();
                HttpContext.Session.SetString("jwtToken", jwtToken);
            }
            else
            {
                jwtToken = HttpContext.Session.GetString("jwtToken");
            }

            var request = new TwoFactorAuthenticationModel();
            request.Email = HttpContext.Session.GetString("Email");
            request.OTP = pin;

            var baseResponse = JsonConvert.DeserializeObject<Response<string>>(
                    await _apiManager.CallPostMethod("Account/ValidateTwoFactorPIN", request, jwtToken));

            return baseResponse;
        }
        #endregion 2 Factor Authenticator

    }
}
