CREATE PROCEDURE [dbo].[spPerson_InsertSet]
	@people BasicUDT readonly
AS
BEGIN
	INSERT INTO dbo.Person(FirstName, LastName)
	SELECT [FirstName], [LastName]
	FROM @people;
end
