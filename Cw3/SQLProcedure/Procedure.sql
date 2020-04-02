create PROCEDURE PromoteStudents @Studies varchar(100), @Semester INT
AS
	BEGIN
	--SET XACT_ABORD ON;
	BEGIN TRAN
	
	DECLARE @IdStudies INT = (SELECT IdStudy From Studies where Name=@Studies);
	IF @IdStudies IS NULL
		BEGIN
			RAISERROR (15600,-1,-1, 'Error'); 
			ROLLBACK
			RETURN;
		END
		
		--czy jest kolejny semestr
		DECLARE @NextIdEnrollment INT = (Select IdEnrollment from Enrollment where IdStudy = @IdStudies AND Semester = (@Semester +1) );
		DECLARE @IdEnrollment INT = (Select IdEnrollment from Enrollment where IdStudy = @IdStudies AND Semester = @Semester );
		IF @NextIdEnrollment IS NULL
			BEGIN
				DECLARE @NewIdEnrollment INT = (Select MAX(IdEnrollment) from Enrollment) + 1;
				INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@NewIdEnrollment, @Semester + 1, @IdStudies, CURRENT_TIMESTAMP); 
				UPDATE Student set IdEnrollment = @NewIdEnrollment where IdEnrollment = @IdEnrollment;
				Commit
				Select * from Enrollment where IdEnrollment = @NewIdEnrollment;
			END

		UPDATE Student set IdEnrollment = @NextIdEnrollment where IdEnrollment = @IdEnrollment;
		COMMIT
		Select * from Enrollment where IdEnrollment = @NextIdEnrollment;
	END
