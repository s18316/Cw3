using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cw3.DTOs;
using Cw3.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public IConfiguration Configuration { get; set; }

        public StudentsController(IDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
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


        [HttpPost]
        [Route("logowanie")]
        public IActionResult Login(LoginRequestDto request)
        {
            //sprawdzanie czy podany index i haslo jest poprawne

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select salt from Student where IndexNumber = @index;";
                com.Parameters.AddWithValue("index", request.Login);

                con.Open();
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    return BadRequest("nie ma osoby o podamym indeksie");
                }

                var salt = dr["salt"].ToString();


                com.CommandText =
                    "select Password from Student where IndexNumber = @index ";
                dr.Close();

                dr = com.ExecuteReader();
                dr.Read();

                var pass = dr[0].ToString();

                var hash = Create(request.Haslo, salt);
                if (!Validate(pass, salt, hash)) return BadRequest("bledne haslo");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "jan123"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Role, "student"),
                    new Claim(ClaimTypes.Role, "employee")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // podpis

                var token = new JwtSecurityToken(
                    issuer: "Sasha",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );


                var tmpToken = Guid.NewGuid();



                com.CommandText = "Update Student set refToken = @token  where IndexNumber = @index ";
                com.Parameters.AddWithValue("token", tmpToken);

                dr.Close();
                com.ExecuteNonQuery();



                return Ok(new
                {
                    //tekstowa reprezentacja
                    token = new JwtSecurityTokenHandler().WriteToken(token), // 5- 10 min
                    refreshToken = tmpToken

                });

            }

            //jezeli nie ma
        }

        [HttpPost("refresh-token/{refToken}")]
        public IActionResult RefreshToken([FromRoute] string refToken)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT IndexNumber FROM Student WHERE refToken = @refToken";
                com.Parameters.AddWithValue("refToken", refToken);
                var dr = com.ExecuteReader();

                
                
                if (!dr.Read()) return BadRequest("nie ma danego tokena w bazie");

                var index = dr["IndexNumber"].ToString();

                dr.Close();
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "jan123"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.Role, "student"),
                    new Claim(ClaimTypes.Role, "employee")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // podpis

                var token = new JwtSecurityToken(
                    issuer: "Sasha",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );


                var tmpToken = Guid.NewGuid();



                com.CommandText = "Update Student set refToken = @token  where IndexNumber = @index ";
                com.Parameters.AddWithValue("token", tmpToken);
                com.Parameters.AddWithValue("index", index);


                dr.Close();
                com.ExecuteNonQuery();



                return Ok(new
                {
                    //tekstowa reprezentacja
                    token = new JwtSecurityTokenHandler().WriteToken(token), // 5- 10 min
                    refreshToken = tmpToken

                });

            }
        }


        //tworzenie hashu i soli

        public static string Create(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );
            return Convert.ToBase64String(valueBytes);
        }


        public static bool Validate(string value, string salt, string hash)
            => Create(value, salt).Equals(hash);

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }


        //////////


        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> listaStudentow = new List<Student>();

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18316;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "select FirstName, LastName, birthdate, Studies.name, Enrollment.semester from Student " +
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
                com.CommandText = "select Enrollment.IdEnrollment, Semester, IdStudy,StartDate from Student " +
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