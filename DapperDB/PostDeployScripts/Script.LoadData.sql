/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DELETE FROM dbo.Person;
INSERT INTO dbo.Person (FirstName, LastName)
VALUES ('Tim', 'Corey'),
('Sue', 'Storm'),
('George', 'Smith'),
('Jane', 'Jones'),
('Fred', 'Smith');

DELETE FROM dbo.Phone;
INSERT INTO dbo.Phone (PhoneNumber)
VALUES ('555-1212');

update dbo.Person
set CellPhoneId = @@IDENTITY
where FirstName = 'Tim' and LastName = 'Corey';

INSERT INTO dbo.Phone (PhoneNumber)
VALUES ('555-7876');

update dbo.Person
set CellPhoneId = @@IDENTITY
where FirstName = 'Jane' and LastName = 'Jones';