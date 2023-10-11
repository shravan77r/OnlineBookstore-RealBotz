using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.Models;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/Books")]
    [Authorize]
    public class BooksController : ControllerBase
    {

        private readonly IBooksRepository _repo;
        public BooksController(IBooksRepository repository)
        {
            _repo = repository;
        }

        [HttpPost("GetBookList")]
        public async Task<Response<List<Book>>> GetBookList(BaseRequest request)
        {
            return await _repo.GetAll(request);
        }
        [HttpPost("GetBookDetails/{BookId}")]
        public async Task<Response<Book>> GetBookDetails(int BookId)
        {
            return await _repo.GetById(BookId);
        }
        [HttpPost("AddBookToCart")]
        public async Task<Response<int>> AddBookToCart(Cart request)
        {
            return await _repo.AddBookToCart(request.BookId, request.UserId, Convert.ToInt32(request.Quantity), DateTime.Now);
        }
        [HttpPost("GetCartList/{UserId}")]
        public async Task<Response<List<Cart>>> GetCartList(int UserId)
        {
            return await _repo.GetCartList(UserId);
        }
        [HttpPost("UpdateCartQty")]
        public async Task<Response<int>> UpdateCartQty(Cart request)
        {
            return await _repo.UpdateCartQty(request.Id, request.Type);
        }
        [HttpPost("DeleteCartItem/{Id}")]
        public async Task<Response<int>> DeleteCartItem(int Id)
        {
            return await _repo.DeleteCartItem(Id);
        }
        [HttpPost("PlaceOrder")]
        public async Task<Response<int>> PlaceOrder(User obj)
        {
            return await _repo.PlaceOrder(obj);
        }
        [HttpPost("GetMyOrdersList/{UserId}")]
        public async Task<Response<List<Order>>> GetMyOrdersList(int UserId)
        {
            return await _repo.GetMyOrdersList(UserId);
        }
    }
}
