USE [Messefrankfurt_BeautyWorld_Revamp]
GO

/****** Object:  StoredProcedure [dbo].[sp_TruncateTables]    Script Date: 10/17/2022 10:10:55 PM ******/
DROP PROCEDURE [dbo].[sp_TruncateTables]
GO

/****** Object:  StoredProcedure [dbo].[sp_TruncateTables]    Script Date: 10/17/2022 10:10:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_TruncateTables]   
AS  
  
BEGIN  
  
Truncate Table  [Search_MainCategory];  
  
Truncate Table  [Search_FLCategory];  
  
Truncate Table  [Search_SLCategory];   
  
Truncate Table  [Search_TLCategory];  
  
Truncate Table  [Search_Exhibitors];  
  
Truncate Table  [Search_Brands];  
  
Truncate Table  [Search_Countries];    
  
Truncate Table  [SEARCH_PRODUCTCATEGORIES];  
  

 
  
END  
  
  
  
  
  
GO


