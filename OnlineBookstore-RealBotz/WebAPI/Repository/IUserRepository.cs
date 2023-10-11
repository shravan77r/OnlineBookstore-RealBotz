
using Model.Models;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Repository
{
    public interface IUserRepository
    {
        Task<Response<User>> GetUser(string Email, string Password);
        Task<Response<User>> GetUserByEmailId(string Email);
        Task<Response<int>> Add(User obj);
        Task<Response<int>> UpdateProfile(User obj);
        Task<Response<List<Country>>> GetCountries();
        Task<Response<List<State>>> GetState(int CountryId);
        Task<Response<List<City>>> GetCity(int StateId);

        Task<Response<int>> Update2FAFlag(string UserName, bool Is2FAEnable, string AuthRecoveryCode);
    }
}
