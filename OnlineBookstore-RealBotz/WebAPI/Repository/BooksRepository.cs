
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
    public class BooksRepository : IBooksRepository
    {
        private readonly IConfiguration _configuration;
        public BooksRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<Response<List<Book>>> GetAll(BaseRequest request)
        {
            var response = new Response<List<Book>>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                string constr = GetConnectionString();
                DataSet dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlCommand com = new SqlCommand("GetBooks", conn);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@PageIndex", request.PageIndex);
                    com.Parameters.AddWithValue("@PageSize", request.PageSize);
                    com.Parameters.AddWithValue("@SortCol", request.SortCol);
                    com.Parameters.AddWithValue("@SortDir", request.SortDir);
                    com.Parameters.AddWithValue("@Keyword", request.Keyword);
                    
                    SqlDataAdapter ad = new SqlDataAdapter(com);
                    ad.Fill(dt);
                }

                response.Data = new List<Book>();

                var result = dt.Tables[1];
                if (result != null && result.Rows.Count > 0)
                {
                    response.Count = Convert.ToInt32(dt.Tables[0].Rows[0][0]);

                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        response.Data.Add(new Book
                        {
                            Id = Convert.ToInt32(result.Rows[i]["Id"]),
                            Title = Convert.ToString(result.Rows[i]["Title"]),
                            Author = Convert.ToString(result.Rows[i]["Author"]),
                            Price = Convert.ToDecimal(result.Rows[i]["Price"]),
                            ISBN = Convert.ToString(result.Rows[i]["ISBN"]),
                            Description = Convert.ToString(result.Rows[i]["Description"]),
                            Image = Convert.ToString(result.Rows[i]["Image"]),
                        });
                    }
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Found Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found!";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<Book>> GetById(int BookId)
        {
            var response = new Response<Book>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                string constr = GetConnectionString();
                string query = "SELECT * FROM tbl_Books where Id = " + BookId;
                DataTable dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(query, conn);
                    ad.Fill(dt);
                }

                response.Data = new Book();

                var result = dt;
                if (result != null && result.Rows.Count > 0)
                {
                    response.Data = new Book
                    {
                        Id = Convert.ToInt32(result.Rows[0]["Id"]),
                        Title = Convert.ToString(result.Rows[0]["Title"]),
                        Author = Convert.ToString(result.Rows[0]["Author"]),
                        Price = Convert.ToDecimal(result.Rows[0]["Price"]),
                        ISBN = Convert.ToString(result.Rows[0]["ISBN"]),
                        Description = Convert.ToString(result.Rows[0]["Description"]),
                        Image = Convert.ToString(result.Rows[0]["Image"]),
                    };
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Found Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found!";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<int>> AddBookToCart(int BookId, int UserId, int Quantity, DateTime? DateAdded)
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
                    string insertSql = "INSERT INTO tbl_Cart (UserId, BookId, Quantity, DateAdded) " +
                        "VALUES (@UserId, @BookId, @Quantity, @DateAdded); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@BookId", BookId);
                        command.Parameters.AddWithValue("@Quantity", Quantity);
                        command.Parameters.AddWithValue("@DateAdded", DateAdded);

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
                    response.Message = "Book added to cart!";
                    response.MessageType = "Success";
                }
                else
                {
                    response.Data = result;
                    response.Status = 0;
                    response.Message = "Failed to add book into cart!";
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
        public async Task<Response<List<Cart>>> GetCartList(int UserId)
        {
            var response = new Response<List<Cart>>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                string constr = GetConnectionString();
                DataTable dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlCommand com = new SqlCommand("Get_UserCartDetails", conn);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@UserId", UserId);
                    SqlDataAdapter ad = new SqlDataAdapter(com);
                    ad.Fill(dt);
                }

                response.Data = new List<Cart>();

                var result = dt;
                if (result != null && result.Rows.Count > 0)
                {
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        response.Data.Add(new Cart
                        {
                            Id = Convert.ToInt32(result.Rows[i]["Id"]),
                            UserId = Convert.ToInt32(result.Rows[i]["UserId"]),
                            Quantity = Convert.ToInt32(result.Rows[i]["Quantity"]),
                            DateAdded = Convert.ToString(result.Rows[i]["DateAdded"]),
                            UserName = Convert.ToString(result.Rows[i]["UserName"]),
                            BookId = Convert.ToInt32(result.Rows[i]["BookId"]),
                            Title = Convert.ToString(result.Rows[i]["Title"]),
                            Author = Convert.ToString(result.Rows[i]["Author"]),
                            Price = Convert.ToDecimal(result.Rows[i]["Price"]),
                            ISBN = Convert.ToString(result.Rows[i]["ISBN"]),
                            Description = Convert.ToString(result.Rows[i]["Description"]),
                            Image = Convert.ToString(result.Rows[i]["Image"]),
                        });
                    }
                    response.Count = response.Data.Count;
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Found Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found!";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<int>> UpdateCartQty(int CartId, int Type)
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
                    string insertSql = "";
                    if (Type == 1)
                    {
                        insertSql = "update tbl_cart set Quantity = Quantity + 1 where id = " + CartId;
                    }
                    else
                    {
                        insertSql = "update tbl_cart set Quantity = Quantity - 1 where id = " + CartId;
                    }

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        try
                        {
                            object objresult = command.ExecuteScalar();
                            result = CartId;
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
                    response.Message = "Cart qty updated successfully.";
                    response.MessageType = "Success";
                }
                else
                {
                    response.Data = result;
                    response.Status = 0;
                    response.Message = "Failed to update Cart qty";
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
        public async Task<Response<int>> DeleteCartItem(int Id)
        {
            var response = new Response<int>
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                if (Convert.ToInt32(Id) > 0)
                {
                    var result = 0;

                    string constr = GetConnectionString();
                    using (SqlConnection connection = new SqlConnection(constr))
                    {
                        connection.Open();
                        string insertSql = "delete from tbl_Cart where id = " + Id;
                        using (SqlCommand command = new SqlCommand(insertSql, connection))
                        {
                            try
                            {
                                object objresult = command.ExecuteScalar();
                                result = Id;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    }
                    if (result > 0)
                    {
                        response.Data = 1;
                        response.Status = 1;
                        response.MessageType = "Success";
                        response.Message = "Record Deleted Successfully!";
                    }
                    else
                    {
                        response.Data = 0;
                        response.Status = 0;
                        response.MessageType = "Error";
                        response.Message = "Record Not Deleted!";
                    }
                }
                else
                {
                    response.Data = 0;
                    response.Status = 0;
                    response.MessageType = "Error";
                    response.Message = "Invalid Id.";
                }
            }
            catch (Exception e)
            {
                response.Data = 0;
                response.Status = 0;
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
        public async Task<Response<int>> PlaceOrder(User obj)
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
                    string insertSql = "INSERT INTO tbl_Orders (UserId, OrderDate, TotalQty," +
                        " TotalAmount, PaymentStatus, Name, CountryId, StateId, CityId, Address, PinCode) " +
                        "VALUES (@UserId, @OrderDate, @TotalQty, @TotalAmount, @PaymentStatus, @Name," +
                        " @CountryId, @StateId, @CityId, @Address, @PinCode); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", obj.Id);
                        command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        command.Parameters.AddWithValue("@TotalQty", 0);
                        command.Parameters.AddWithValue("@TotalAmount", 0);
                        command.Parameters.AddWithValue("@PaymentStatus", "Pending");
                        command.Parameters.AddWithValue("@Name", obj.FirstName);
                        command.Parameters.AddWithValue("@CountryId", obj.CountryId);
                        command.Parameters.AddWithValue("@StateId", obj.StateId);
                        command.Parameters.AddWithValue("@CityId", obj.CityId);
                        command.Parameters.AddWithValue("@Address", obj.Address);
                        command.Parameters.AddWithValue("@PinCode", obj.PinCode);

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
                    var cartdetails = await GetCartList(obj.Id);
                    decimal TotalQty = 0;
                    decimal TotalAmount = 0;

                    if (cartdetails.Data.Count > 0)
                    {
                        foreach (var item in cartdetails.Data)
                        {
                            TotalQty += item.Quantity;
                            TotalAmount += (item.Price * item.Quantity);

                            using (SqlConnection connection = new SqlConnection(constr))
                            {
                                connection.Open();
                                string insertSql = "INSERT INTO tbl_ordersDetails (OrderId, BookId, Quantity," +
                                    " Price, TotalAmount) " +
                                    "VALUES (@OrderId, @BookId, @Quantity, @Price, @TotalAmount); SELECT SCOPE_IDENTITY();";

                                using (SqlCommand command = new SqlCommand(insertSql, connection))
                                {
                                    
                                    command.Parameters.AddWithValue("@OrderId", result);
                                    command.Parameters.AddWithValue("@BookId", item.BookId);
                                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    command.Parameters.AddWithValue("@Price", item.Price);
                                    command.Parameters.AddWithValue("@TotalAmount", (item.Price * item.Quantity));

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
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(constr))
                    {
                        connection.Open();
                        string insertSql = "update tbl_orders set TotalQty = "+ TotalQty+ ", " +
                            "TotalAmount = "+ TotalAmount + " where id = " + result + " Delete from tbl_Cart where UserId = " + obj.Id;
                       
                        using (SqlCommand command = new SqlCommand(insertSql, connection))
                        {
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

                    response.Data = result;
                    response.Status = 1;
                    response.Message = "Order Placed Successfully!";
                    response.MessageType = "Success";
                }
                else
                {
                    response.Data = result;
                    response.Status = 0;
                    response.Message = "Order Not Placed!";
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
        public async Task<Response<List<Order>>> GetMyOrdersList(int UserId)
        {
            var response = new Response<List<Order>>()
            {
                Status = 0,
                Message = "Exception Occured.",
                MessageType = "Error",
            };
            try
            {
                string constr = GetConnectionString();
                DataTable dt = new();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    SqlCommand com = new SqlCommand("GetMyOrdersList", conn);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@UserId", UserId);
                    SqlDataAdapter ad = new SqlDataAdapter(com);
                    ad.Fill(dt);
                }

                response.Data = new List<Order>();

                var result = dt;
                if (result != null && result.Rows.Count > 0)
                {
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        response.Data.Add(new Order
                        {
                            Id = Convert.ToInt32(result.Rows[i]["Id"]),
                            UserId = Convert.ToInt32(result.Rows[i]["UserId"]),
                            UserName = Convert.ToString(result.Rows[i]["UserName"]),
                            TotalQty = Convert.ToInt32(result.Rows[i]["TotalQty"]),
                            TotalAmount = Convert.ToDecimal(result.Rows[i]["TotalAmount"]),
                            OrderDate = Convert.ToString(result.Rows[i]["OrderDate"]),
                            PaymentStatus = Convert.ToString(result.Rows[i]["PaymentStatus"]),
                            Name = Convert.ToString(result.Rows[i]["Name"]),
                            Email = Convert.ToString(result.Rows[i]["Email"]),
                            MobileNo = Convert.ToString(result.Rows[i]["MobileNo"]),
                        });
                    }
                    response.Count = response.Data.Count;
                    response.Status = 1;
                    response.MessageType = "Success";
                    response.Message = "Data Found Successfully!";
                }
                else
                {
                    response.MessageType = "Error";
                    response.Message = "No Data Found!";
                }
            }
            catch (Exception e)
            {
                response.MessageType = "Error";
                response.Message = "Something Went Wrong!";
            }
            return response;
        }
    }
}
