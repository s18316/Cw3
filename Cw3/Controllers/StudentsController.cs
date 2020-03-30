
using System.Collections.Generic;
using System.Data.SqlClient;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]

    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;

        }
        [HttpDelete]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpPut]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokończona");
        }


        [HttpGet]
        public IActionResult GetStudents()
        {

            List<Student> listaStudentow = new List<Student>();
        
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select FirstName, LastName, birthdate, Studies.name, Enrollment.semester from Student " +
                    "inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment " +
                    "inner join Studies on Studies.IdStudy = Enrollment.IdStudy; ";

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["birthdate"].ToString();
                    st.Studies = dr["name"].ToString();
                    st.Semester = dr["semester"].ToString();

                    listaStudentow.Add(st);

                }


            }
            return Ok(listaStudentow);
        }
        [HttpGet("{id}")]
        public IActionResult GetEnrollment(int id)
        {

            List<Enrollment> enList = new List<Enrollment>();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                
                //com.CommandText = "select Enrollment.IdEnrollment, Semester, IdStudy,StartDate from Student " +
                    //"inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment" +
                   // $" where IndexNumber={id};";

                //ostatnie zadanie 
                com.CommandText= "select Enrollment.IdEnrollment, Semester, IdStudy,StartDate from Student " +
                    "inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment" +
                    $" where IndexNumber=@id;";

                com.Parameters.AddWithValue("id", id);
                con.Open();
                var dr = com.ExecuteReader();
                
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEndrollment = dr["IdEnrollment"].ToString();
                    en.Semester = dr["Semester"].ToString();
                    en.IdStudy = dr["IdStudy"].ToString();
                    en.StartDate = dr["StartDate"].ToString();
                    enList.Add(en);
                   
                }
            
            return Ok(enList);
            }
            

        }

            }

       // [HttpGet]
       // public string GetStudent(string orderBy)
       // {
        //    return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
       // }


      //  [HttpGet("{id}")]
       // public IActionResult GetStudent(int id)
        //{
          //  if (id == 1)
          //  {
          //      return Ok("Kowalski");
          //  }else if (id == 2)
         //   {
          //      return Ok("Malewski");
          //  }

        //   return NotFound("Nie znaleziono studenta");
      //  }

      //  [HttpPost]
      //  public IActionResult CreateStudent(Student student)
      //  {
            //...add to database
            //...generation index number
         //   student.IndexNumber = $"s{new Random().Next(1, 20000)}";
         //   return Ok(student);
      //  }
  //  }
}