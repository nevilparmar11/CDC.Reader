use CDC_POC_DB_BACKUP;

CREATE TABLE [dbo].[Employees]
(
	[EmployeeId] INT PRIMARY KEY, 
	[FirstName] varchar(25),
	[LastName] varchar(25),
	[PhoneNumber] varchar(25),
	[Email] varchar(25),
	[Date] DateTime default SYSUTCDATETIME(),
);

ALTER Table [dbo].[Employees]
ADD GlobalEmployeeId UNIQUEIDENTIFIER NULL
DEFAULT NEWID();

drop table dbo.Employees;

select * from dbo.Employees;

delete from dbo.Employees;
