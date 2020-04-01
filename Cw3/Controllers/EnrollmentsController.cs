using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;

        }

        [HttpPost]
        public IActionResult PutStudentOnSemester(Student student)
        {
            if (student.FirstName == null || student.LastName == null || student.Studies == null ||
                student.IndexNumber == null || student.BirthDate == null)
            {
                return BadRequest();
            }


            return Accepted();
        }

        [HttpPost]
        public IActionResult upgreadStudent(Student student)
        {
            if (student.Studies == null || student.Semester == null) return BadRequest();
            bool czyIstnieje = _dbService.CheckStudies(student.Studies);

            return Accepted();
        }
    }
}