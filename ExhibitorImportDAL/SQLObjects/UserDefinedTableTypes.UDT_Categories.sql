USE [Messefrankfurt_BeautyWorld_Revamp]
GO

/****** Object:  UserDefinedTableType [dbo].[UDT_Categories]    Script Date: 10/17/2022 10:13:18 PM ******/
DROP TYPE [dbo].[UDT_Categories]
GO

/****** Object:  UserDefinedTableType [dbo].[UDT_Categories]    Script Date: 10/17/2022 10:13:18 PM ******/
CREATE TYPE [dbo].[UDT_Categories] AS TABLE(
	[ID] [int] NULL,
	[CategoryCode] [varchar](20) NULL,
	[CategoryName] [varchar](200) NULL,
	[ParentCategoryCode] [varchar](20) NULL,
	[CategoryLevel] [int] NULL
)
GO


