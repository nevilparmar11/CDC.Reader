USE CDC_POC_DB;

USE [CDC_POC_DB]
GO

IF EXISTS (	SELECT	ss.name, t.[name], is_tracked_by_cdc
			FROM	sys.tables t
					inner join sys.schemas ss on ss.schema_id = t.schema_id
			WHERE	t.name = 'Employees' AND SS.name = 'dbo' AND is_tracked_by_cdc = 1)
	BEGIN
		EXECUTE sys.sp_cdc_disable_table
		@source_schema = N'dbo'  
		, @source_name = N'Employees'  
		, @capture_instance = N'dbo_Employees'
	END

BEGIN
	EXECUTE sys.sp_cdc_enable_table  
		@source_schema = N'dbo'  
		, @source_name = N'Employees'  
		, @role_name = NULL;
END

GO


exec sys.sp_cdc_enable_db;
exec sys.sp_cdc_disable_db;

drop table dbo.Employees;

CREATE TABLE [dbo].[Employees]
(
	[EmployeeId] INT Identity(1,1) PRIMARY KEY, 
	[FirstName] varchar(25),
	[LastName] varchar(25),
	[PhoneNumber] varchar(25),
	[Email] varchar(25),
	[Date] DateTime default SYSUTCDATETIME(),
);

CREATE TABLE [dbo].[CDCProcessLogs]
(
	[ProcessLogId] INT Identity(1,1) PRIMARY KEY, 
	[LSN] binary(10),
	[TableName] varchar(50) NOT NULL,
	[TimeStamp] DateTime default SYSUTCDATETIME(),
);

ALTER Table [dbo].[Employees]
ADD GlobalEmployeeId UNIQUEIDENTIFIER NULL
DEFAULT NEWID();

delete from dbo.Employees;

delete from dbo.CDCProcessLogs;


drop table dbo.CDCProcessLogs;

select * from dbo.Employees;
select * from cdc.dbo_Employees_CT order by __$start_lsn desc;
select * from dbo.CDCProcessLogs;

select * from cdc.dbo_Employees_CT where __$start_lsn > 0x0000002A000062380003;

update dbo.Employees set LastName='Christian' where EmployeeId  = 16;

delete from dbo.CDCProcessLogs where ProcessLogId = 1;


delete from dbo.Employees where EmployeeId = 17;

insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('Nevil', 'Parmar', '1234567890', 'nevil@gmail.com');
insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('Ashish', 'Parmar', '1234567890', 'Ashish@gmail.com');
insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('Anil', 'Parmar', '1234567890', 'temp@gmail.com');
insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('temp', 'patel', '1234567890', 'temp@gmail.com');
insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('Dhruval', 'gandhi', '1234567890', 'dhruval@gmail.com');
insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('Dhwanit', 'Patel', '0987654321', 'dhwanit@gmail.com');

insert into dbo.Employees(FirstName, LastName,PhoneNumber,Email) values('d1', 'asdf', '0987654321', 'tt@gmail.com');

insert into dbo.CDCProcessLogs(TableName, LSN) Values('dbo.CDCProcessLogs', 0x0000002A000062380003);