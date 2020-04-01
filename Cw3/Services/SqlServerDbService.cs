using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerDbService :IStudentsDbService
    {
        public bool CheckStudies(string studiesName)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "Select 1 from Studies where Studies.Name = @studiesName;";
                com.Parameters.AddWithValue("@" + studiesName, studiesName);

                con.Open();

                var ans = com.ExecuteReader();
                if (ans.HasRows) return true;
                else return false;


            }
        }
    }
}
