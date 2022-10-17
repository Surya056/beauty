USE [Messefrankfurt_BeautyWorld_Revamp]
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertLevelCategories]    Script Date: 10/17/2022 10:10:39 PM ******/
DROP PROCEDURE [dbo].[sp_InsertLevelCategories]
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertLevelCategories]    Script Date: 10/17/2022 10:10:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_InsertLevelCategories]
@Allcategories UDT_Categories READONLY
AS
BEGIN


SET IDENTITY_INSERT Search_ProductCategories  ON

INSERT INTO Search_ProductCategories(ID,CategoryCode,CategoryName,Parent_ID,Count,level,EventId,EventYear,CreatedBy,CreatedOn,Language)
SELECT ID,CategoryCode,CategoryName,0,0,CategoryLevel,0,'2022','gmi-testing',GETDATE(),1 FROM @Allcategories

SET IDENTITY_INSERT Search_ProductCategories  OFF


UPDATE SPC SET SPC.Parent_ID = parent.ID
FROM Search_ProductCategories SPC JOIN @Allcategories child ON SPC.ID = child.ID
JOIN @Allcategories parent ON child.ParentCategoryCode = parent.CategoryCode;



INSERT INTO Search_MainCategory(CategoryName,CreatedBy,CreatedOn,CategoryCode)
SELECT CategoryName,'gmi-testing',GETDATE(),CategoryCode FROM Search_ProductCategories WHERE [level] = 0

INSERT INTO Search_FLCategory(CategoryName,FK_MainCategory,CreatedBy,CreatedOn,CategoryCode)
SELECT CategoryName,
(SELECT M.MainCategoryAutoID FROM Search_MainCategory M WHERE M.CategoryCode = SUBSTRING(SPC.CategoryCode, 1, DATALENGTH(SPC.CategoryCode)/2 - CHARINDEX('.', REVERSE(SPC.CategoryCode))))
,'gmi-testing',GETDATE(),CategoryCode FROM Search_ProductCategories SPC WHERE SPC.[level] = 1


INSERT INTO Search_SLCategory(CategoryName,FK_FLCategory,CreatedBy,CreatedOn,CategoryCode)
SELECT CategoryName,
(SELECT F.FLCategoryAutoID FROM Search_FLCategory F WHERE F.CategoryCode = SUBSTRING(SPC.CategoryCode, 1, DATALENGTH(SPC.CategoryCode)/2 - CHARINDEX('.', REVERSE(SPC.CategoryCode))))
,'gmi-testing',GETDATE(),CategoryCode FROM Search_ProductCategories SPC WHERE SPC.[level] = 2

INSERT INTO Search_TLCategory(CategoryName,FK_SLCategory,CreatedBy,CreatedOn,CategoryCode)
SELECT CategoryName,
(SELECT S.SLCategoryAutoID FROM Search_SLCategory S WHERE S.CategoryCode = SUBSTRING(SPC.CategoryCode, 1, DATALENGTH(SPC.CategoryCode)/2 - CHARINDEX('.', REVERSE(SPC.CategoryCode))))
,'gmi-testing',GETDATE(),CategoryCode FROM Search_ProductCategories SPC WHERE SPC.[level] = 3


END
GO


