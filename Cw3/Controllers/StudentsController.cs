using System.Collections.Generic;
using System.Data.SqlClient;
using Cw3.Models2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public IConfiguration Configuration { get; set; }

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteStudent([FromRoute] string id)
        {
            var czyUsuniety = _dbService.RemoveStudent(id);

            if (czyUsuniety) return Ok("student został usuninety");

            else
                return BadRequest("podano bledna eske");
        }

        [HttpPost]
        [Route("modify")]
        public IActionResult ModifyStudent(Student student)
        {
            var czyZmodyfikowany = _dbService.ModifyStudent(student);
            if (czyZmodyfikowany)
                return Ok("Student zostal zmodyfikowany");

            else
                return BadRequest("blad w modyfikacji");
        }

        [HttpPut]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokończona");
        }


        ///
        [HttpGet]
        public IActionResult GetStudents()
        {
            var listaStudentow = _dbService.GetStudents();

            return Ok(listaStudentow);
        }
    }
}