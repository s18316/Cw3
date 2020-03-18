using System;
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
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
       }

       // [HttpGet]
       // public string GetStudent(string orderBy)
       // {
        //    return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
       // }


        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }else if (id == 2)
            {
                return Ok("Malewski");
            }

           return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //...add to database
            //...generation index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
    }
}