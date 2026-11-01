-- ============================================================
-- BƯỚC 0: Tạo database MyStore (chạy trước database2024.sql)
-- Lý do: file database2024.sql của giảng viên bắt đầu bằng CREATE TABLE
-- và kết thúc bằng "ALTER AUTHORIZATION ON DATABASE::MyStore TO sa;",
-- nên database MyStore phải tồn tại trước khi chạy file đó.
-- ============================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MyStore')
BEGIN
    CREATE DATABASE [MyStore];
END
GO

USE [MyStore];
GO
