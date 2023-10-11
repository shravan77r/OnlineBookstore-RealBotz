using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model.Models;
using Model.Models.Common;
using Newtonsoft.Json;
using OnlineBookstore_RealBotz.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBookstore_RealBotz.Controllers
{
    //[SessionExpirationCheck]
    [Route("Home")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private APIManager _apiManager;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _apiManager = new APIManager(configuration);
        }

        [HttpPost("GetUserDetails")]
        public async Task<JsonResult> GetUserDetails()
        {
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                if (UserId == 0)
                {
                    return Json(new
                    {
                        Status = 0,
                        Message = "Session expired please login again.",
                    });
                }
                int CartCount = Convert.ToInt32(HttpContext.Session.GetInt32("CartCount"));
                int Is2FAEnable = Convert.ToInt32(HttpContext.Session.GetInt32("Is2FAEnable"));
                string UserName = HttpContext.Session.GetString("UserName");
                string ProfilePhoto = HttpContext.Session.GetString("ProfilePhoto");

                return Json(new
                {
                    Status = 1,
                    Message = "User Found Successfully!",
                    UserId,
                    CartCount,
                    UserName,
                    ProfilePhoto,
                    Is2FAEnable
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Status = 0,
                    Message = "Session expired please login again.",
                });
            }

        }

        [HttpPost("GetBookList")]
        public async Task<JsonResult> GetBookList(BaseRequest request)
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

            var resp = JsonConvert.DeserializeObject<Response<List<Book>>>(
                   await _apiManager.CallPostMethod("Books/GetBookList", request, jwtToken));

            return Json(resp);
        }
        [HttpPost("GetBookDetails")]
        public async Task<JsonResult> GetBookDetails(int BookId)
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

            var resp = JsonConvert.DeserializeObject<Response<Book>>(
                   await _apiManager.CallPostMethod("Books/GetBookDetails/" + BookId, null, jwtToken));

            return Json(resp);
        }

        [HttpPost("AddBookToCart")]
        public async Task<JsonResult> AddBookToCart(Cart request)
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

            request.UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            var resp = JsonConvert.DeserializeObject<Response<int>>(
                 await _apiManager.CallPostMethod("Books/AddBookToCart" , request, jwtToken));

            if (resp.Data > 0)
            {
                int CartCount = Convert.ToInt32(HttpContext.Session.GetInt32("CartCount")) + Convert.ToInt32(request.Quantity);
                HttpContext.Session.SetInt32("CartCount", CartCount);
            }
            return Json(resp);

        }

        [HttpPost("GetCartList")]
        public async Task<JsonResult> GetCartList()
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

            int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            var resp = JsonConvert.DeserializeObject<Response<List<Cart>>>(
                await _apiManager.CallPostMethod("Books/GetCartList/" + UserId, null, jwtToken));

            return Json(resp);
        }
        [HttpPost("UpdateCartQty")]
        public async Task<JsonResult> UpdateCartQty(int CartId, int Type)
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

            var request = new Cart();
            request.Id = CartId;
            request.Type = Type;

            var resp = JsonConvert.DeserializeObject<Response<int>>(
                await _apiManager.CallPostMethod("Books/UpdateCartQty", request, jwtToken));
            
            return Json(resp);
        }

        [HttpPost("DeleteCartItem")]
        public async Task<JsonResult> DeleteCartItem(int Id, int Qty)
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

            var resp = JsonConvert.DeserializeObject<Response<int>>(
                 await _apiManager.CallPostMethod("Books/DeleteCartItem/" + Id, null, jwtToken));

            if (resp.Data > 0)
            {
                int CartCount = Convert.ToInt32(HttpContext.Session.GetInt32("CartCount")) - 1;

                HttpContext.Session.SetInt32("CartCount", CartCount);
            }

            return Json(resp);
        }
        [HttpPost("PlaceOrder")]
        public async Task<JsonResult> PlaceOrder(User obj)
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

            obj.Id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            var resp = JsonConvert.DeserializeObject<Response<int>>(
                await _apiManager.CallPostMethod("Books/PlaceOrder", obj, jwtToken));

            if (resp.Data > 0)
            {
                HttpContext.Session.SetInt32("CartCount", 0);
            }
            return Json(resp);
        }
        
        [HttpPost("GetMyOrdersList")]
        public async Task<JsonResult> GetMyOrdersList()
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

            int UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            var resp = JsonConvert.DeserializeObject<Response<List<Order>>>(
                await _apiManager.CallPostMethod("Books/GetMyOrdersList/" + UserId, null, jwtToken));

            return Json(resp);
        }

    }
}
