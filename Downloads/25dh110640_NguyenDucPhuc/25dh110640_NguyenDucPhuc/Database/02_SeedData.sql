USE [MyStore];
GO
INSERT INTO [User] ([Username], [Password], [UserRole]) VALUES
(N'admin',    N'123456', N'A'),
(N'customer', N'123456', N'C'),
(N'nguyenvana', N'123456', N'C');
INSERT INTO [Category] ([CategoryName]) VALUES
(N'Điện thoại'),
(N'Laptop'),
(N'Máy tính bảng'),
(N'Phụ kiện');
INSERT INTO [Customer] ([CustomerName], [CustomerPhone], [CustomerEmail], [CustomerAddress], [Username]) VALUES
(N'Nguyễn Văn A',  N'0909123456', N'nguyenvana@gmail.com', N'123 Nguyễn Huệ, Q.1, TP.HCM',     N'customer'),
(N'Nguyễn Văn A',  N'0909888777', N'a.nguyen@gmail.com',   N'456 Lê Lợi, Q.3, TP.HCM',         N'nguyenvana');
INSERT INTO [Product] ([CategoryID], [ProductName], [ProductDecription], [ProductPrice], [ProductImage]) VALUES
(1, N'iPhone 15 Pro Max',      N'Chip A17 Pro, camera 48MP, màn hình 6.7 inch Super Retina XDR',                 32990000, N'~/Content/images/default.png'),
(1, N'Samsung Galaxy S24 Ultra', N'Snapdragon 8 Gen 3, RAM 12GB, bút S-Pen tích hợp, camera 200MP',              30990000, N'~/Content/images/default.png'),
(1, N'Xiaomi Redmi Note 13',   N'Màn hình AMOLED 120Hz, pin 5000mAh, sạc nhanh 33W',                              6490000, N'~/Content/images/default.png'),
(2, N'MacBook Air M3',         N'Chip Apple M3, RAM 8GB, SSD 256GB, màn hình Liquid Retina 13.6 inch',           27990000, N'~/Content/images/default.png'),
(2, N'Dell XPS 15 9530',       N'Core i7-13700H, RAM 16GB, RTX 4050, màn hình OLED 3.5K',                         42990000, N'~/Content/images/default.png'),
(2, N'Asus Vivobook 15',       N'Core i5-1235U, RAM 8GB, SSD 512GB, mỏng nhẹ 1.7kg',                             15990000, N'~/Content/images/default.png'),
(3, N'iPad Pro 11 M2',         N'Chip M2, màn hình Liquid Retina 120Hz, hỗ trợ Apple Pencil 2',                   22990000, N'~/Content/images/default.png'),
(3, N'Samsung Galaxy Tab S9',  N'Snapdragon 8 Gen 2, màn hình AMOLED 120Hz, S-Pen đi kèm',                        18990000, N'~/Content/images/default.png'),
(4, N'AirPods Pro 2',          N'Chống ồn chủ động, chip H2, chống nước IP54, pin 6 giờ',                          5990000, N'~/Content/images/default.png'),
(4, N'Sạc nhanh Anker 65W',    N'GaN 65W, 3 cổng, sạc được laptop và điện thoại',                                  1290000, N'~/Content/images/default.png'),
(4, N'Bàn phím cơ Keychron K3', N'Switch low-profile, kết nối Bluetooth 5.1, pin 34 giờ',                          2190000, N'~/Content/images/default.png'),
(4, N'Ốp lưng Spigen iPhone 15', N'Chống sốc, chất liệu TPU + PC, thiết kế mỏng nhẹ',                               450000, N'~/Content/images/default.png');
INSERT INTO [Order] ([CustomerID], [OrderDate], [TotalAmount], [PaymentStatus], [AddressDelivery]) VALUES
(1, CAST(GETDATE() AS DATE), 32990000 + 5990000, N'Chờ thanh toán', N'123 Nguyễn Huệ, Q.1, TP.HCM'),
(1, DATEADD(DAY, -7, CAST(GETDATE() AS DATE)), 15990000, N'Đã giao', N'123 Nguyễn Huệ, Q.1, TP.HCM'),
(2, DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 18990000, N'Đang vận chuyển', N'456 Lê Lợi, Q.3, TP.HCM');
INSERT INTO [OrderDetail] ([ProductID], [OrderID], [Quantity], [UnitPrice]) VALUES
(1, 1, 1, 32990000),
(9, 1, 1,  5990000),
(6, 2, 1, 15990000),
(8, 3, 1, 18990000);
GO
