/* SCRIPT TO CREATE Account TABLE IN THE COMPANY DATABASE */
USE [Company]
GO

CREATE TABLE [dbo].[Account](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[balance] [decimal](16, 2) NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ( [id] ASC))
GO