using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using PH_Swag.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json.Serialization;

namespace PH_Swag.Services
{
    public class CartService
    {

        private readonly PH_SwagEntity _db;
        private readonly IConfiguration _configuration;

        public int _userID;
        public int _cartID;
        public int _numItems;
        public int _swagBucks;
        public int _isRecFound;

        public int _isCartSaved;
        public int _isNewCartItemID;

        public CartService(PH_SwagEntity db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            
            _isRecFound= 0;
            _userID = 0;
            _cartID = 0;
            _swagBucks = 0;
            _numItems = 0;

            _isNewCartItemID = 0;
        }


        // Initialize Cart if cart is missing, return totalItems in cart
        public int Initialize(string userName)
        {
            if (userName != null)
            {

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "sp_SetCart_CartTotal";
                        cmd.CommandType = CommandType.StoredProcedure; 
                        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userName;
                        
                        cmd.Connection = con;
                        con.Open();
                         
                        
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                _isRecFound = 1;
                                _userID = (int)reader.GetValue("userID");
                                _cartID = (int)reader.GetValue("cartID");
                                _swagBucks = (int)reader.GetValue("swagBucks");
                                _numItems = (int)reader.GetValue("cartNumTotal");
                            }

                        }
                        reader.Close();

                        if (con.State == System.Data.ConnectionState.Open)
                            con.Close();
                    }
                }
            }
                return 0;
        }
       

        // Get Current Swag Bucks from Activity Log database
        public int GetSwagBucks(string userName)
        {

            int swagBucks = 0;

           
            if (userName != null) {

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SELECT UserID, swagBucks FROM ActivityLogVer2.dbo.Users where UserName  = @userName";
                        cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = userName;
                        cmd.Connection = con;
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                swagBucks = (int)reader.GetValue("swagBucks");
                                _userID = (int)reader.GetValue("UserID");
                            }

                        }
                        reader.Close();

                        if (con.State == System.Data.ConnectionState.Open)
                            con.Close();
                    }
                }
                






                //using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                //{

                //    //string strSql = "SELECT swagBucks FROM ActivityLogVer2.dbo.Users where UserName  = '" + userName + "'";
                //    //SqlCommand sqlComm2 = new SqlCommand(strSql, sqlConnection);
                //    //sqlComm2.Parameters.AddWithValue("@userName", userName);

                //    string strSql = "SELECT swagBucks FROM ActivityLogVer2.dbo.Users where UserName  = @userName";
                //     SqlCommand sqlComm2 = new SqlCommand(strSql, sqlConnection);
                //    sqlComm2.Parameters.AddWithValue("@userName", userName);
                //    //sqlComm2.Parameters.Add("@userName", SqlDbType.VarChar, 20).Value = userName;
                //    try
                //    {
                //        sqlConnection.Open();
                //        SqlDataReader reader = sqlComm2.ExecuteReader();

                //        if (reader.HasRows)
                //        {
                //            while (reader.Read())
                //            {
                //                swagBucks = (int)reader.GetValue("swagBucks");
                //            }

                //        }
                //        reader.Close();

                //        //      if returnValue != null then
                //        //statusReturned = returnValue.ToString();
                //    }
                //    catch (Exception ex)
                //    {
                //        //handle exception
                //    }
                //}

            }

           
            return swagBucks;

        }


        public int AddProductsToCart(int userId, int cartID,  int prdId, int quantity, string size)
        {

            // Insert into Cart/Cart Items
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "sp_AddProductToCart";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@cartID", SqlDbType.Int).Value = cartID;
                    cmd.Parameters.Add("@prdId", SqlDbType.Int).Value = prdId;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@size", SqlDbType.VarChar, 10).Value = size;

                    cmd.Connection = con;
                    con.Open();


                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _isCartSaved = 1;
                            _isNewCartItemID = (int)reader.GetValue("new_CartItem");
                           
                        }

                    }
                    reader.Close();

                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                }
            }


            return _isNewCartItemID;


            //using (var trn = _db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        AddCart(userId);
            //        Cart cart = _db.Cart.FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);
            //        AddCartItem(prdId, quantity, cart);
            //        _db.SaveChanges();
            //        trn.Commit();
            //        return cart.Quantity;
            //    }
            //    catch (Exception e)
            //    {
            //        trn.Rollback();
            //        throw new Exception(e.Message);
            //    }
            //}

            //return 0;
        }

        public int UpdateCartItem(int userId, int cartItemId,  int quantity, string size)
        {

            // Insert into Cart/Cart Items
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "sp_UpdateCartItem";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@cartItemID", SqlDbType.Int).Value = cartItemId;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@size", SqlDbType.VarChar, 10).Value = size;

                    cmd.Connection = con;
                    con.Open();


                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _isCartSaved = 1;
                            _isNewCartItemID = (int)reader.GetValue("new_CartItem");

                        }

                    }
                    reader.Close();

                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                }
            }


            return _isNewCartItemID;
        }

        public int UpdateCartItemQuantity(int addSub, int userId, int cartItemId)
        {

            // Insert into Cart/Cart Items
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "sp_UpdateCartItemQuantity";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@addSub", SqlDbType.Int).Value = addSub;
                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@cartItemID", SqlDbType.Int).Value = cartItemId;
                    cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = 1;
                    

                    cmd.Connection = con;
                    con.Open();


                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _isCartSaved = 1;
                            _isNewCartItemID = (int)reader.GetValue("new_CartItem");

                        }

                    }
                    reader.Close();

                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                }
            }


            return _isNewCartItemID;
        }




        //private void AddCart(int userId)
        //{
        //    Cart cart = _db.Cart.FirstOrDefault(x => x.CustomerId == userId && !x.IsCheckedOut);
        //    if (cart == null)
        //    {
        //        cart = new Cart(userId);
        //        _db.Cart.Add(cart);
        //    }
        //    _db.SaveChanges();
        //}

        //public void AddCartItem(int prdId, int quantity, Cart cart)
        //{
        //    Product product = _db.Products.First(x => x.id == prdId);
        //    CartItem cartItem = _db.CartItem.FirstOrDefault(x => x.CartId == cart.Id && x.ProductId == prdId);

        //    if (cartItem == null)
        //    {
        //        cartItem = new CartItem(cart.Id, prdId);
        //        _db.CartItem.Add(cartItem);
        //    }
        //    else
        //    {
        //        cartItem.Quantity += quantity;
        //    }

        //    if (cartItem.Quantity == 0)
        //    {
        //        _db.CartItem.Remove(cartItem);
        //    }
        //    cart.Quantity += quantity;
        //    cart.Value += quantity * product.prodPrice;
        //}




    }
}
