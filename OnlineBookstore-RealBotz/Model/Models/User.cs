using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public int Gender { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string ProfileImage { get; set; }
        public string OldProfileImage { get; set; }
        public int CartCount { get; set; }
        public bool Is2FAEnable { get; set; }
        public string AuthRecoveryCode { get; set; }

    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
    public class State
    {
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
    }
    public class City
    {
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
}
