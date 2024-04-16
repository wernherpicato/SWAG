using Microsoft.EntityFrameworkCore;
using PH_Swag.Models;

namespace PH_Swag.Services
{
    public class LoginService
    {
        private readonly PH_SwagEntity _db;

        public LoginService(PH_SwagEntity db)
        {
            db = db;
        }

        public int Login(string username, string password, ISession session)
        {
            //Customer customer;
            //customer = db.Customers.Where(x => x.UserName == username).FirstOrDefault();
            //if (customer == null)
            //{
            //    return 1;
            //}
            //string pwd = customer.Password;
            //password = GetHash(password);
            //if (pwd != password)
            //{
            //    return 2;
            //}
            //else
            //{
            //    session.SetInt32("customerId", customer.Id);
            //    return 3;
            //}

            return 0;
        }




    }
}
