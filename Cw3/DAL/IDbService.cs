using Cw3.Models;
using System;
using System.Collections.Generic;

public interface IDbService
{
	public IEnumerable<Student> GetStudents();

}
