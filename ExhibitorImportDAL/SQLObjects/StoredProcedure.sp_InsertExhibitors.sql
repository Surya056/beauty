USE [Messefrankfurt_BeautyWorld_Revamp]
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertExhibitors]    Script Date: 10/17/2022 10:10:33 PM ******/
DROP PROCEDURE [dbo].[sp_InsertExhibitors]
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertExhibitors]    Script Date: 10/17/2022 10:10:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_InsertExhibitors]  
@AllExhibitors UDT_Exhibitors READONLY  
AS  
BEGIN  
  
INSERT INTO Search_Exhibitors(FkMainCategory,FkFLCategory,FkSLCategory  
,ExhibitorName,Details,StandNo,HallNo,Address,TelephoneNo,FaxNo,EmailAddress,  
Website,StandManager,Profile,CreatedBy,CreatedOn,Exhibitor_DisplayName  
,API_EntryId,Slug,Language,ProdNomenclature)  
SELECT 0,0,0,ExhibitorName,Details,StandNo,HallNo,Address,TelephoneNo,FaxNo,EmailAddress,  
SUBSTRING(Website,0,50),StandManager,Profile,CreatedBy,GETDATE(),Exhibitor_DisplayName  
,API_EntryId,Slug,Language,ProdNomenclature FROM @AllExhibitors  


INSERT INTO Search_Brands(BrandName,FkExhibitors,CreatedBy,CreatedOn)  
SELECT BrandName,ExhibitorAutoID,CreatedBy,GETDATE()    
FROM   
(SELECT TE.API_EntryId,SE.ExhibitorAutoID,SE.CreatedBy,Brand1, Brand2, Brand3, Brand4, Brand5,Brand6  
FROM @AllExhibitors TE JOIN Search_Exhibitors SE ON TE.API_EntryId = SE.API_EntryId) P  
UNPIVOT    
   (BrandName FOR Brand IN     
      (Brand1, Brand2, Brand3, Brand4, Brand5,Brand6)    
)AS unpvt;    
  
  
INSERT INTO Search_Countries(CountryName,FkExhibitors,CreatedBy,CreatedOn)  
SELECT AE.Country,SE.ExhibitorAutoID,AE.CreatedBy,GETDATE() FROM @AllExhibitors AE JOIN Search_Exhibitors SE ON AE.API_EntryId = SE.API_EntryId  
  
  
  
END  
  
GO


