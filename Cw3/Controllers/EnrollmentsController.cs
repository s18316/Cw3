using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;

        }

        [HttpPost]
        [Authorize(Roles ="employee")]
        public IActionResult EnrollStudent([FromBody]Student student)
        {
            Console.WriteLine(student.Studies);
            if (student.FirstName == null || student.LastName == null || student.Studies == null ||
                student.IndexNumber == null || student.BirthDate == null)
            {
                return BadRequest();
            }

           // Enrollment czyIstnieje = _dbService.Rejestracja(student.Studies, student);
          //  Console.WriteLine(czyIstnieje);
          //  if (czyIstnieje == null) return BadRequest();

         //   ObjectResult ob = new ObjectResult(czyIstnieje);
           // ob.StatusCode = 201;
           //return ob ;
           return null;
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent([FromBody] Studie studie)
        {
            if (studie.Studies == null || studie.Semester == null) return BadRequest();

        //  ObjectResult ob = new ObjectResult(new SqlServerDbService().PromoteStudents(studie.Semester, studie.Studies));
        //  ob.StatusCode = 201;
           // return ob;
           return null;
        }
    }
}