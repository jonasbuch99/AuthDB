using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AuthLibrary
{
    public class Authentication
    {
        public static string Login(string email, string password, int sysID)
        {
            int userID;
            string token;
            //Parameterne til at etablere connection til Azure DB 
            #region Stored Proccedure Call LoginProcedure
            string conString = "Data Source=jobu.database.windows.net;Initial Catalog=AuthDB;Integrated Security=False;User ID=jobu@sydtrafik.dk@jobu;Password=P@ssword123;";
            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand("LoginProcedure", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            DataTable dt = new DataTable();
            #endregion 

            conn.Open();
            dt.Load(cmd.ExecuteReader());
            try
            {
                 userID = Convert.ToInt32(dt.Rows[0][0]);//der kan kun være en række derfor tør jeg godt bruge rows[0][0]
            }
            catch
            {
                userID = 0;
            }

            conn.Close();

            if(userID != 0)
            {
                token = GetToken(userID, sysID);
            }
            else
            {
                token = "";
            }

            return token;
        }

        public static string GetToken(int userID, int sysID)
        {

            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string conString = "Data Source=jobu.database.windows.net;Initial Catalog=AuthDB;Integrated Security=False;User ID=jobu@sydtrafik.dk@jobu;Password=P@ssword123;";

            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand("CreateToken", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@system", sysID);
            cmd.Parameters.AddWithValue("@token", token);
            DataTable dt = new DataTable();
            conn.Open();
            dt.Load(cmd.ExecuteReader());
            if(dt.Rows.Count != 0)
            {
                token = dt.Rows[0][0].ToString();
            }
            conn.Close();

            return token;
        }
    }
}
