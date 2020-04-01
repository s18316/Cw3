﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

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
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    Console.WriteLine(studiesName);
                    com.Connection = con;
                    com.CommandText = "Select * from Studies where Studies.Name = @studiesName1;";
                    com.Parameters.AddWithValue("studiesName1", studiesName);

                    con.Open();

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
                                "DECLARE @idEnrollment int = (SELECT TOP 1 Enrollment.IdEnrollment FROM Enrollment " +
                                "ORDER BY Enrollment.IdEnrollment DESC) + 1; " +
                                "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate)" +
                                " VALUES(@idEnrollment, 1, @idStudy, CURRENT_TIMESTAMP); " +
                                "Select @idEnrollment";
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

                        ans.Close();
                        DodStudenta(student, com, IdEnrollment);

                        ans.Close();
                        trans.Commit();
                        con.Close();
                    }

                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return false;
                }


            }
                return czy;
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
                                com.Parameters.AddWithValue("IdEnrollment", IdEnrollment);

                                com.ExecuteNonQuery();
                                
        }


    }
}
