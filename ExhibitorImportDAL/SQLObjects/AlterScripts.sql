IF COL_LENGTH('Search_Exhibitors', 'ProdNomenclature') IS  NULL
BEGIN
ALTER TABLE Search_Exhibitors ADD  ProdNomenclature VARCHAR(1000)
END

IF COL_LENGTH('Search_MainCategory', 'CategoryCode') IS  NULL
BEGIN
ALTER TABLE Search_MainCategory ADD CategoryCode VARCHAR(30)
END

IF COL_LENGTH('Search_FLCategory', 'CategoryCode') IS  NULL
BEGIN
ALTER TABLE Search_FLCategory ADD CategoryCode VARCHAR(30)
END

IF COL_LENGTH('Search_SLCategory', 'CategoryCode') IS  NULL
BEGIN
ALTER TABLE Search_SLCategory ADD CategoryCode VARCHAR(30)
END

IF COL_LENGTH('Search_TLCategory', 'CategoryCode') IS  NULL
BEGIN
ALTER TABLE Search_TLCategory ADD CategoryCode VARCHAR(30)
END