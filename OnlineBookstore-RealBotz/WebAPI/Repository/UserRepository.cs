
using Microsoft.Extensions.Configuration;
using Model.Models;
using Model.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<Response<User>> GetUser(string Email, string Password)
        {
            var response = new Response<User>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                string constr = GetConnectionString();
                string query = "SELECT U.*,(SELECT Count(C.Id) FROM tbl_Cart C where C.UserId = U.Id) CartCount " +
                    "FROM tbl_Users U WHERE U.Email = '" + Email + "' and U.Password = '" + Password + "'";
                DataTable dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(query, conn);
                    ad.Fill(dt);
                }


                var result = dt;
                if (result != null && result.Rows.Count > 0)
                {
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        response.Data = new User
                        {
                            Id = Convert.ToInt32(result.Rows[i]["Id"]),
                            FirstName = Convert.ToString(result.Rows[i]["FirstName"]),
                            LastName = Convert.ToString(result.Rows[i]["LastName"]),
                            Address = Convert.ToString(result.Rows[i]["Address"]),
                            Email = Convert.ToString(result.Rows[i]["Email"]),
                            Password = Convert.ToString(result.Rows[i]["Password"]),
                            ProfileImage = Convert.ToString(result.Rows[i]["ProfileImage"]),
                            CartCount = Convert.ToInt32(result.Rows[i]["CartCount"]),
                            Is2FAEnable = Convert.ToBoolean(result.Rows[i]["Is2FAEnable"]),
                            AuthRecoveryCode = Convert.ToString(result.Rows[i]["AuthRecoveryCode"]),
                        };
                    }
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "User Found Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "Invalid User Details!";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<User>> GetUserByEmailId(string Email)
        {
            var response = new Response<User>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                string constr = GetConnectionString();
                string query = "SELECT U.*,(SELECT Count(C.Id) FROM tbl_Cart C where C.UserId = U.Id) CartCount " +
                    "FROM tbl_Users U WHERE U.Email = '" + Email + "'";
                DataTable dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(query, conn);
                    ad.Fill(dt);
                }


                var result = dt;
                if (result != null && result.Rows.Count > 0)
                {
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        response.Data = new User
                        {
                            Id = Convert.ToInt32(result.Rows[i]["Id"]),
                            FirstName = Convert.ToString(result.Rows[i]["FirstName"]),
                            LastName = Convert.ToString(result.Rows[i]["LastName"]),
                            Address = Convert.ToString(result.Rows[i]["Address"]),
                            Email = Convert.ToString(result.Rows[i]["Email"]),
                            Password = Convert.ToString(result.Rows[i]["Password"]),
                            ProfileImage = Convert.ToString(result.Rows[i]["ProfileImage"]),
                            CartCount = Convert.ToInt32(result.Rows[i]["CartCount"]),
                        };
                    }
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "User Found Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "Invalid User Details!";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<int>> Add(User obj)
        {
            var response = new Response<int>
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                var result = 0;

                string constr = GetConnectionString();
                using (SqlConnection connection = new SqlConnection(constr))
                {
                    connection.Open();
                    string insertSql = "INSERT INTO tbl_Users (FirstName, LastName, Email, Password," +
                        " MobileNo, Gender, CountryId, StateId, CityId, Address, PinCode, ProfileImage) " +
                        "VALUES (@FirstName, @LastName, @Email, @Password, @MobileNo, @Gender, @CountryId, @StateId, @CityId," +
                        " @Address, @PinCode, @ProfileImage); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", obj.FirstName);
                        command.Parameters.AddWithValue("@LastName", obj.LastName);
                        command.Parameters.AddWithValue("@Email", obj.Email);
                        command.Parameters.AddWithValue("@Password", obj.Password);
                        command.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                        command.Parameters.AddWithValue("@Gender", obj.Gender);
                        command.Parameters.AddWithValue("@CountryId", obj.CountryId);
                        command.Parameters.AddWithValue("@StateId", obj.StateId);
                        command.Parameters.AddWithValue("@CityId", obj.CityId);
                        command.Parameters.AddWithValue("@Address", obj.Address);
                        command.Parameters.AddWithValue("@PinCode", obj.PinCode);
                        command.Parameters.AddWithValue("@ProfileImage", obj.ProfileImage);

                        try
                        {
                            //result = command.ExecuteNonQuery();
                            object objresult = command.ExecuteScalar();
                            result = Convert.ToInt32(objresult);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    }
                }
                if (result > 0)
                {
                    response.Data = result;
                    response.Status = 1;
                    response.Message = "Record Inserted Successfully!";
                    response.MessageType = "Success";
                }
                else
                {
                    response.Data = result;
                    response.Status = 0;
                    response.Message = "Record Not Inserted!";
                    response.MessageType = "Error";
                }

            }
            catch (Exception ex)
            {
                response.Data = 0;
                response.Status = 0;
                response.Message = "Something Went Wrong!";
                response.MessageType = "Error";
            }
            return response;
        }

        public async Task<Response<int>> UpdateProfile(User obj)
        {
            var response = new Response<int>
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                var result = 0;

                string constr = GetConnectionString();
                using (SqlConnection connection = new SqlConnection(constr))
                {
                    connection.Open();
                    string insertSql = "Update tbl_Users SET ProfileImage = @ProfileImage WHERE Id = @Id; SELECT @Id;";

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", obj.Id);
                        command.Parameters.AddWithValue("@ProfileImage", obj.ProfileImage);

                        try
                        {
                            object objresult = command.ExecuteScalar();
                            result = Convert.ToInt32(objresult);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    }
                }
                if (result > 0)
                {
                    response.Data = result;
                    response.Status = 1;
                    response.Message = "Profile Updated Successfully!";
                    response.MessageType = "Success";
                }
                else
                {
                    response.Data = result;
                    response.Status = 0;
                    response.Message = "Profile Not Updated!";
                    response.MessageType = "Error";
                }

            }
            catch (Exception ex)
            {
                response.Data = 0;
                response.Status = 0;
                response.Message = "Something Went Wrong!";
                response.MessageType = "Error";
            }
            return response;
        }

        public async Task<Response<List<Country>>> GetCountries()
        {
            var response = new Response<List<Country>>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            response.Data = new();
            try
            {
                string constr = GetConnectionString();
                string _query = "select Id, CountryName from tbl_Country order by Id";
                DataTable data = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(_query, conn);
                    ad.Fill(data);
                }

                if (data != null && data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        response.Data.Add(new Country
                        {
                            CountryId = Convert.ToInt32(data.Rows[i]["Id"]),
                            CountryName = Convert.ToString(data.Rows[i]["CountryName"])
                        });
                    }
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Fetched Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<List<State>>> GetState(int CountryId)
        {
            var response = new Response<List<State>>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            response.Data = new();
            try
            {
                string constr = GetConnectionString();
                string _query = "select Id, StateName from tbl_State where CountryId=" + CountryId;
                DataTable data = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(_query, conn);
                    ad.Fill(data);
                }

                if (data != null && data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        response.Data.Add(new State
                        {
                            StateId = Convert.ToInt32(data.Rows[i]["Id"]),
                            StateName = Convert.ToString(data.Rows[i]["StateName"])
                        });
                    }
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Fetched Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<List<City>>> GetCity(int StateId)
        {
            var response = new Response<List<City>>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            response.Data = new();
            try
            {
                string constr = GetConnectionString();
                string _query = "select Id, CityName from tbl_City where StateId=" + StateId;
                DataTable data = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(_query, conn);
                    ad.Fill(data);
                }

                if (data != null && data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        response.Data.Add(new City
                        {
                            CityId = Convert.ToInt32(data.Rows[i]["Id"]),
                            CityName = Convert.ToString(data.Rows[i]["CityName"])
                        });
                    }
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Fetched Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }

        public async Task<Response<int>> Update2FAFlag(string UserName, bool Is2FAEnable, string AuthRecoveryCode)
        {
            var response = new Response<int>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };

            int result = 0;
            string constr = GetConnectionString();
            try
            {
                
                string query = "SELECT Id FROM tbl_Users where Email = '" + UserName +"'";
                DataTable dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(query, conn);
                    ad.Fill(dt);
                }

                foreach (DataRow dr1 in dt.Rows)
                    result = (int)dr1[0];

                string Query = "";
                if (result > 0)
                {
                    Query = "Update tbl_Users set Is2FAEnable=@Is2FAEnable, AuthRecoveryCode = @AuthRecoveryCode where Id=@Id";

                    using (SqlConnection connection = new SqlConnection(constr))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(Query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", result);
                            command.Parameters.AddWithValue("@Is2FAEnable", Is2FAEnable);
                            command.Parameters.AddWithValue("@AuthRecoveryCode", AuthRecoveryCode?? "");

                            try
                            {
                                object objresult = command.ExecuteScalar();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    }

                    if (result > 0)
                    {
                        response.Data = result;
                        response.Status = 1;
                        response.Message = "Settings Updated Successfully!";
                        response.MessageType = "Success";
                    }
                    else
                    {
                        response.Data = result;
                        response.Status = 0;
                        response.Message = "Settings Not Updated!";
                        response.MessageType = "Error";
                    }

                }
            }
            catch (Exception ex)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }

    }
}
