IF (OBJECT_ID('ManyDataTypes') IS NOT NULL)
BEGIN
    DROP TABLE ManyDataTypes
END

CREATE TABLE ManyDataTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    C1 NVARCHAR(50),
	C2 Char(3),
	C3 DateTime2(7),
	C4 DateTimeOffset(7),
	C5 int,
	C6 smallint,
	C7 tinyint,
	C8 varbinary(max),
	C9 uniqueidentifier,
	c10 date,
	c11 datetime,
	c12 time,
	c13 float,
	c14 decimal,
	c15 money,
	c16 real,
    C17 bit,
	C18 int null
)