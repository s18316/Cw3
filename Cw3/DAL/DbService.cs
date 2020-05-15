using Cw3.Models2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

public class Dbservice : IDbService
{
    private static s18316Context _dbContext;
  
    public Dbservice(DbContext dbContext)
    {
        _dbContext = (s18316Context) dbContext;

    }


    public IEnumerable<Student> GetStudents()
    {

        var students = _dbContext.Student.ToList();

        return students;
    }

    public bool RemoveStudent(string id)
    {
        
        var student = _dbContext.Student.Single(s => s.IndexNumber.Equals(id));
        if (student == null) return false;
        _dbContext.Student.Remove(student);
        _dbContext.SaveChanges();
        return true;
    }

    public bool ModifyStudent(Student student)
    {
        var studentDoModyfikacji = _dbContext.Student.Single(s => s.IndexNumber.Equals(student.IndexNumber));
        if (studentDoModyfikacji == null) return false;
        else
        {
            if (student.FirstName != null)
            {
                studentDoModyfikacji.FirstName = student.FirstName;
            }

            if (student.LastName != null)
            {
                studentDoModyfikacji.LastName = student.LastName;
            }

            if (student.IdEnrollment != 0)
            {
                studentDoModyfikacji.IdEnrollment = student.IdEnrollment;
            }

            if (student.BirthDate != null)
            {
                studentDoModyfikacji.BirthDate = student.BirthDate;
            }

            if (student.Password != null)
            {
                studentDoModyfikacji.Password = student.Password;
            }

            if (student.Salt != null)
            {
                studentDoModyfikacji.Salt = student.Salt;
            }

            if (student.RefToken != null)
            {
                studentDoModyfikacji.RefToken = student.RefToken;
            }

            _dbContext.SaveChanges();

            return true;

            return true;
        }
    }

}
