using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AuthLibrary
{
    public class Authorization
    {
        public static bool ValidateAccess(string token, int sysID, int levelID)
        {
            bool validated = false;
            string conString = "Data Source=jobu.database.windows.net;Initial Catalog=AuthDB;Integrated Security=False;User ID=jobu@sydtrafik.dk@jobu;Password=St@yf0cus3d;";

            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand("ValidateAccess", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@token", token);
            cmd.Parameters.AddWithValue("@sysID", sysID);
            DataTable dt = new DataTable();
            conn.Open();
            dt.Load(cmd.ExecuteReader());
            foreach (DataRow myRow in dt.Rows)
            {
                if(levelID == (int)myRow[0])
                {
                    validated = true;
                }
            }
            //cmd.ExecuteNonQuery();
            conn.Close();
            return validated;
        }
    }
}
