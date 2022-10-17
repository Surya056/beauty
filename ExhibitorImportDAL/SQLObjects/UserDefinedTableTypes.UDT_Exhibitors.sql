USE [Messefrankfurt_BeautyWorld_Revamp]
GO

/****** Object:  UserDefinedTableType [dbo].[UDT_Exhibitors]    Script Date: 10/17/2022 10:11:16 PM ******/
DROP TYPE [dbo].[UDT_Exhibitors]
GO

/****** Object:  UserDefinedTableType [dbo].[UDT_Exhibitors]    Script Date: 10/17/2022 10:11:16 PM ******/
CREATE TYPE [dbo].[UDT_Exhibitors] AS TABLE(
	[ExhibitorName] [varchar](200) NULL,
	[Details] [varchar](20) NULL,
	[StandNo] [varchar](20) NULL,
	[HallNo] [varchar](20) NULL,
	[Address] [varchar](1000) NULL,
	[TelephoneNo] [varchar](20) NULL,
	[FaxNo] [varchar](20) NULL,
	[EmailAddress] [varchar](200) NULL,
	[Website] [varchar](100) NULL,
	[StandManager] [varchar](100) NULL,
	[Profile] [varchar](5000) NULL,
	[CreatedBy] [varchar](100) NULL,
	[Exhibitor_DisplayName] [varchar](100) NULL,
	[API_EntryId] [varchar](20) NULL,
	[Slug] [nvarchar](1000) NULL,
	[Language] [varchar](100) NULL,
	[Brand1] [nvarchar](max) NULL,
	[Brand2] [nvarchar](max) NULL,
	[Brand3] [nvarchar](max) NULL,
	[Brand4] [nvarchar](max) NULL,
	[Brand5] [nvarchar](max) NULL,
	[Brand6] [nvarchar](max) NULL,
	[Country] [varchar](100) NULL,
	[ProdNomenclature] [varchar](1000) NULL
)
GO


