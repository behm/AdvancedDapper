CREATE PROCEDURE [dbo].[spPerson_Search]
	@searchTerm VARCHAR(50)
AS
begin
	set nocount on;

	select [Id], [FirstName], [LastName]
	from dbo.Person
	WHERE FirstName LIKE '%' + @searchTerm + '%' 
		OR LastName LIKE '%' + @searchTerm + '%';
end
