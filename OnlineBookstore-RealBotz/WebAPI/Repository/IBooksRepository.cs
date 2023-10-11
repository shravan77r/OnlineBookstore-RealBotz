
using Model.Models;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Repository
{
    public interface IBooksRepository
    {
        Task<Response<List<Book>>> GetAll(BaseRequest request);
        Task<Response<Book>> GetById(int BookId);
        Task<Response<List<Cart>>> GetCartList(int UserId);
        Task<Response<int>> AddBookToCart(int BookId, int UserId, int Quantity, DateTime? DateAdded);
        Task<Response<int>> UpdateCartQty(int CartId, int Type);
        Task<Response<int>> DeleteCartItem(int Id);
        Task<Response<int>> PlaceOrder(User obj);
        Task<Response<List<Order>>> GetMyOrdersList(int UserId);

    }
}
