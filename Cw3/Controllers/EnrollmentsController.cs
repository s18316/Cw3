using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Cw3.Models2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Enrollment = Cw3.Models2.Enrollment;

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
        public IActionResult EnrollStudent([FromBody]StudentToEnroll student)
        {
            Console.WriteLine(student.Studies);
            if (student.FirstName == null || student.LastName == null || student.Studies == null ||
                student.IndexNumber == null || student.BirthDate == null)
            {
                return BadRequest();
            }

            Enrollment czyIstnieje = _dbService.Enroll(student.Studies, student);
            Console.WriteLine(czyIstnieje);
            if (czyIstnieje == null) return BadRequest();

            ObjectResult ob = new ObjectResult(czyIstnieje);
            ob.StatusCode = 201;
           return ob ;
        }

        [HttpPost("promotions")]
   public IActionResult PromoteStudent([FromBody] Studie studie)
        {
            if (studie.Studies == null || studie.Semester == null) return BadRequest();

         ObjectResult ob = new ObjectResult(_dbService.PromoteStudents(studie.Semester, studie.Studies));
          ob.StatusCode = 201;
            return ob;
        }
    }
}