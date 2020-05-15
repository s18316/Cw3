using Cw3.Models2;
using System;
using System.Collections.Generic;

public interface IDbService
{
	public IEnumerable<Student> GetStudents();

    bool RemoveStudent(string id);
    bool ModifyStudent(Student student);
    Enrollment PromoteStudents(int semester, string name);
    Enrollment Enroll(string studies, StudentToEnroll student);
}
