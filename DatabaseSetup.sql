IF EXISTS (SELECT * FROM sys.databases WHERE name = 'SpatialDataDB')
    DROP DATABASE SpatialDataDB;
GO

CREATE DATABASE SpatialDataDB;

USE SpatialDataDB
GO

EXEC sp_configure 'show advanced options', 1;
	RECONFIGURE;
GO

EXEC sp_configure 'clr enable', 1;
	RECONFIGURE;
GO

EXEC sp_configure 'clr strict security', 0;
	RECONFIGURE;
GO

IF OBJECT_ID('Points') IS NOT NULL
        DROP TABLE Points;

IF EXISTS (SELECT 1 FROM sys.assembly_types WHERE assembly_id = (SELECT assembly_id FROM sys.assemblies WHERE name = 'MyAssembly'))
BEGIN
    DROP TYPE [dbo].[Point];
END

IF OBJECT_ID('Figures') IS NOT NULL
    DROP TABLE Figures;

IF EXISTS (SELECT 1 FROM sys.assembly_types WHERE assembly_id = (SELECT assembly_id FROM sys.assemblies WHERE name = 'MyAssembly'))
BEGIN
    DROP TYPE [dbo].[Figure];
END

IF EXISTS (SELECT 1 FROM sys.assemblies WHERE name = 'MyAssembly')
BEGIN
    DROP ASSEMBLY [MyAssembly];
END

CREATE ASSEMBLY MyAssembly
	FROM 'C:\Users\Administrator\source\repos\SpatialData\SpatialData\bin\Debug\SpatialData.dll'
	WITH PERMISSION_SET = SAFE;
GO

CREATE TYPE dbo.Point
	EXTERNAL NAME MyAssembly.[SpatialData.Point];
GO

CREATE TABLE Points (
    id INT IDENTITY(1,1) PRIMARY KEY,
    point dbo.Point
);
GO

CREATE TYPE dbo.Figure
	EXTERNAL NAME MyAssembly.[SpatialData.Figure];
GO

CREATE TABLE Figures (
    id INT IDENTITY(1,1) PRIMARY KEY,
	figure dbo.Figure
);
GO