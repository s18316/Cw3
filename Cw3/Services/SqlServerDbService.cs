using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;

namespace Cw3.Services
{
    public class SqlServerDbService :IStudentsDbService
    {
        public bool Rejestracja(string studiesName, Student student)
        {
            bool czy;
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                Console.WriteLine(studiesName);
                com.Connection = con;
                com.CommandText = "Select * from Studies where Studies.Name = @studiesName;";
                com.Parameters.AddWithValue("studiesName", studiesName);

                con.Open();

                var ans = com.ExecuteReader();
                
                czy = ans.Read();

                if (czy)
                {
                    //sprawdzanie czy jest pierwszy semestr danego kierunku
                    com.CommandText = "Select TOP 1 * from Enrollment  " +
                                      "inner join Studies on Studies.IdStudy = Enrollment.IdStudy " +
                                      "where Name = @studiesName" +
                                      " Order by StartDate Desc;";
                    com.Parameters.AddWithValue("studiesName", studiesName);
                    ans = com.ExecuteReader();

                    //dodawanie przedmiotu na pierwszym roku
                    if (!ans.Read())
                    {
                        


                    }

                }


            }
                return czy;
        }


        public void DodStudenta(Student student, SqlCommand com)
        {
            
        }

        public void DodPrzedmiot(string nazwa, SqlCommand com)
        {
            com.CommandText =
        }
    }
}
