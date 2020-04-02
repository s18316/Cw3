using System;
using System.Data;
using System.Data.SqlClient;
using Cw3.Models;

namespace Cw3.Services
{
    public class SqlServerDbService :IStudentsDbService
    {
        public Enrollment Rejestracja(string studiesName, Student student)
        {
            Enrollment newEnroll = null;
            bool czy;
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                SqlTransaction trans = null;
                
                try
                {
                   // Console.WriteLine(studiesName);
                     com.Connection = con;
                    con.Open();
                 trans = con.BeginTransaction();
                 com.Transaction = trans;

                    com.CommandText = "Select * from Studies where Studies.Name = @studiesName1;";
                    com.Parameters.AddWithValue("studiesName1", studiesName);

                    

                    var ans = com.ExecuteReader();

                    //sprawdzanie czy jest taki numer IdStudenta
                    czy = ans.Read();
                    com.CommandText = "Select * from Student where IndexNumber = @IndexNumer; ";
                    com.Parameters.AddWithValue("IndexNumer", student.IndexNumber);

                    ans.Close();
                    ans = com.ExecuteReader();
                    if (ans.Read()) czy = false;

                    if (czy)
                    {
                        //sprawdzanie czy jest pierwszy semestr danego kierunku
                        com.CommandText = "Select TOP 1 * from Enrollment  " +
                                          "inner join Studies on Studies.IdStudy = Enrollment.IdStudy " +
                                          "where Name = @studiesName2 AND Semester = 1" +
                                          " Order by StartDate Desc;";
                        com.Parameters.AddWithValue("studiesName2", studiesName);
                        ans.Close();
                        ans = com.ExecuteReader();

                        int IdEnrollment;


                        //dodawanie przedmiotu na pierwszym roku
                        if (!ans.Read())
                        {
                            com.CommandText =
                                "DECLARE @idStudy int = (SELECT Studies.IdStudy FROM Studies" +
                                " WHERE Studies.Name = @studiesName3); " +
                                "DECLARE @idEnrollment2 int = (SELECT TOP 1 Enrollment.IdEnrollment FROM Enrollment " +
                                "ORDER BY Enrollment.IdEnrollment DESC) + 1; " +
                                "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)" +
                                " VALUES(@idEnrollment2, 1, @idStudy, CURRENT_TIMESTAMP); " +
                                "Select @idEnrollment2";
                            com.Parameters.AddWithValue("studiesName3", studiesName);

                            ans.Close();
                            ans = com.ExecuteReader();
                            ans.Read();
                            IdEnrollment = ans.GetInt32(0);
                        }
                        else
                        {
                            IdEnrollment = ans.GetInt32(0);
                        }
                        com.Parameters.AddWithValue("IdEnrollment", IdEnrollment);
                        ans.Close();
                        DodStudenta(student, com, IdEnrollment);

                        ans.Close();

                        //odstatni element zadania 1
                        com.CommandText = "Select * from Enrollment where IdEnrollment = @IdEnrollment";
                       
                         newEnroll = new Enrollment();
                        ans = com.ExecuteReader();
                        ans.Read();
                        newEnroll.IdEndrollment = ans[0].ToString();
                        newEnroll.Semester = ans[1].ToString();
                        newEnroll.IdStudy = ans[2].ToString();
                        newEnroll.StartDate = ans[3].ToString();
                        ans.Close();
;                        trans.Commit();
                        
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    trans.Rollback();
                    return null;
                }


            }
                return newEnroll;
        }


        public void DodStudenta(Student student, SqlCommand com, int IdEnrollment)
        {
            com.CommandText = "Declare @datetyp date = Parse(@BirthDate as date Using 'en-GB'); " +
                              "Insert into Student (IndexNumber,FirstName,LastName,BirthDate, IdEnrollment)" +
                              " Values (@IndexNumber, @FirstName, @LastName, @datetyp, @IdEnrollment);";

                                com.Parameters.AddWithValue("IndexNumber", student.IndexNumber);
                                com.Parameters.AddWithValue("FirstName", student.FirstName);
                                com.Parameters.AddWithValue("LastName", student.LastName);
                                com.Parameters.AddWithValue("BirthDate", student.BirthDate);
                              
                                com.ExecuteNonQuery();
                                
        }
        public Enrollment PromoteStudents(int semester, string studies)
        {

            SqlTransaction trans = null;

            Enrollment enrollment = new Enrollment();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "PromoteStudents";
                com.Parameters.AddWithValue("Studies", studies);
                com.Parameters.AddWithValue("Semester", semester);

                var ans = com.ExecuteReader();
                ans.Read();

                enrollment.IdEndrollment = ans.GetInt32(0)+"";
                enrollment.Semester = ans.GetInt32(1)+"";
                enrollment.IdStudy = ans.GetInt32(2)+"";
                enrollment.StartDate = ans[3].ToString();

                ans.Close();
            }
            return enrollment;

        }


    }
}
