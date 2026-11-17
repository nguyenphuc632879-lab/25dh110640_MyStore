SHOPTECH / 25dh110640 - Ghi chú phần bổ sung

1. Đã bổ sung:
- Models/ViewModel/CartViewModels.cs: CartItem, Cart, CheckoutVM.
- Models/CartService.cs: lưu giỏ hàng trong Session.
- Models/Order.Extensions.cs: thuộc tính giao diện [NotMapped], không yêu cầu thêm cột database.
- Views/Cart/PVSimilarProduct.cshtml.
2. Đã cập nhật:
- AccountController: UserRole dùng 'C' theo database (Admin dùng 'A').
- OrderController: lưu đơn hàng theo đúng AddressDelivery, UnitPrice, Quantity; tổng tiền lấy từ giỏ hàng trong Session.
- OrderSuccess/MyOrder: hiển thị ngày theo DATE (không giả giờ/phút).
- HomeController: bổ sung các action mà View đang gọi.
- packages.config và .csproj: thêm NuGet PagedList 1.17.0.
3. Theo xác nhận:
- PayPal chỉ là lựa chọn giao diện, chưa xử lý giao dịch thật.
- PaymentMethod/ShippingMethod chỉ dùng tạm trong phiên đặt hàng; không thêm cột database.
4. Lưu ý:
- Các action danh mục và ProductDetails hiện chuyển về trang chủ vì project gốc chưa có View tương ứng; không tự tạo thiết kế mới.
- Cần mở project bằng Visual Studio, chạy Restore NuGet Packages để tải PagedList, sau đó Build.
- Môi trường chỉnh sửa hiện tại không có MSBuild/.NET Framework nên chưa thể xác nhận Build thực tế trên Visual Studio.
- Database không bị sửa.
