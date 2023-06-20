IF EXISTS (SELECT * FROM sys.databases WHERE name = 'SpatialDataDB')
    DROP DATABASE SpatialDataDB;
GO

CREATE DATABASE SpatialDataDB;

USE SpatialDataDB
GO

CREATE TABLE Points (
    id INT IDENTITY(1,1) PRIMARY KEY,
    point dbo.Point
);
GO

CREATE TABLE Figures (
    id INT IDENTITY(1,1) PRIMARY KEY,
    figure dbo.Figure
);
GO

CREATE OR ALTER PROCEDURE create_polygon
AS
BEGIN
    DECLARE @point dbo.Point;
    
    DECLARE curs CURSOR
        LOCAL STATIC READ_ONLY FORWARD_ONLY
    FOR
        SELECT Point FROM Points;
        
    OPEN curs
    FETCH NEXT FROM curs INTO @point
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE Figures set figure.ImportPoints = @point;
        FETCH NEXT FROM curs INTO @point;
    END
    
    CLOSE curs;
    DEALLOCATE curs;
END;
GO

CREATE OR ALTER PROCEDURE drop_tables
AS
BEGIN
    IF OBJECT_ID('Points') IS NOT NULL
        DROP TABLE Points;
        
    IF OBJECT_ID('Polygons') IS NOT NULL
        DROP TABLE Polygons;
END;