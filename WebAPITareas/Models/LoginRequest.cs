using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebAPITareas.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool isValid()
        {
            int count = 0;

            string config = ConfigurationManager.ConnectionStrings["ci2_test"].ConnectionString;

            using (SqlConnection sqlcon = new SqlConnection(config))
            {
                SqlCommand sqlcmd = new SqlCommand("select * from Usuario where username='" + Username + "' and password='" + Password + "'", sqlcon);
                sqlcmd.Connection.Open();
                count = System.Convert.ToInt32(sqlcmd.ExecuteScalar());
                sqlcmd.Connection.Close();

                if (count > 0)
                    return true;
                else
                    return false;
            }

        }
    }
}