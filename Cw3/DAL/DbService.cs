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
        }
    }

    public Enrollment Enroll(string studies, StudentToEnroll student)
    {
        //czy istnieje taki student
        var aktStudent = _dbContext.Student.SingleOrDefault(s => s.IndexNumber == student.IndexNumber);
        if (aktStudent != null) return null;

        //czy isnieja takie studia
        var stu =  _dbContext.Studies.FirstOrDefault(s => s.Name == studies);
        if (stu == null) return null;

        //czy te studia maja pierwszy semestr
        var aktStudies = _dbContext.Enrollment.SingleOrDefault(s => s.IdStudy == stu.IdStudy && s.Semester == 1);
        if (aktStudies == null)
        {
             var idEnrollment = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1;
            aktStudies = new Enrollment
            {
                IdEnrollment = idEnrollment,
                Semester = 1,
                IdStudy = stu.IdStudy,
                StartDate = DateTime.Now
            };
            _dbContext.Enrollment.Add(aktStudies);
        }

        //dodawanie studenta na kierunek
        Student studencik = new Student
        {
   IndexNumber = student.IndexNumber,
    FirstName = student.FirstName,
    LastName  = student.LastName,
    BirthDate = student.BirthDate,
    IdEnrollment = aktStudies.IdEnrollment,
    Salt  = 
   RefToken =
     Password = 
};

        _dbContext.Student.Add(studencik);
        //
        return null;
        
    }


    public Enrollment PromoteStudents(int semester, string name)
    {
        var studies = _dbContext.Studies.Single(stu => stu.Name == name);

        //sprawdzanie czy isnieje kolejny semestr danego przedmiotu

        var enrollment = _dbContext.Enrollment.SingleOrDefault(en => en.IdStudy == studies.IdStudy && en.Semester == semester + 1);

        if (enrollment == null)
        {
            //jak nie to tworzymy i dodajemy do bazy 
            //tworzenie nowego numeru Id
            var idEnrollment = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1;
            enrollment = new Enrollment
            {
                IdEnrollment = idEnrollment,
                Semester = semester + 1,
                IdStudy = studies.IdStudy,
                StartDate = DateTime.Now
            };

            _dbContext.Enrollment.Add(enrollment);
        }

        //wybranie studentow ktorzy maja byc przeniesieni na kolejny semestr
        var aktEnrollment = _dbContext.Enrollment.Single(en => en.IdStudy == studies.IdStudy && en.Semester == semester);

        var studenci = _dbContext.Student.Where(s => s.IdEnrollment == aktEnrollment.IdEnrollment).ToList();

        foreach (Student student in studenci)  student.IdEnrollment = enrollment.IdEnrollment;

        _dbContext.SaveChanges();

        return enrollment;
    }
}