-- ============================================================
-- BE#7 (slide "Trang Đặt hàng - Model - DB"): cập nhật bảng Order và OrderDetail
-- CHẠY SAU database2024.sql, TRƯỚC 01_SeedData_optional.sql
-- ============================================================
USE MyStore;
GO

-- Bảng Order: thêm DeliveryMethod, PaymentMethod; đổi AddressDelivery -> ShippingAddress
IF COL_LENGTH('dbo.[Order]', 'DeliveryMethod') IS NULL
    ALTER TABLE [Order] ADD DeliveryMethod NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.[Order]', 'PaymentMethod') IS NULL
    ALTER TABLE [Order] ADD PaymentMethod NVARCHAR(100) NULL;
GO
IF COL_LENGTH('dbo.[Order]', 'AddressDelivery') IS NOT NULL
    EXEC sp_rename 'dbo.[Order].AddressDelivery', 'ShippingAddress', 'COLUMN';
GO

-- Bảng OrderDetail: thêm TotalPrice tự động tính từ Quantity và UnitPrice
IF COL_LENGTH('dbo.OrderDetail', 'TotalPrice') IS NULL
    ALTER TABLE OrderDetail ADD TotalPrice AS (Quantity * UnitPrice) PERSISTED;
GO
