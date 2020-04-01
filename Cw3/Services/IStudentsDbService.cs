using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;

namespace Cw3.Services
{
    public interface IStudentsDbService
    {
        public bool Rejestracja(string studiesName, Student student);
        public void DodStudenta(Student student, SqlCommand com, int IdEnrollment);
        


    }
}
