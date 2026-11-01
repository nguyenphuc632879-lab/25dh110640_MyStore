# 25dh110640_MyStore — Website bán hàng ASP.NET MVC5 + Entity Framework

Bài làm theo bộ slide *Web BE Development* (p1 Project Setup, p2 Admin Site, p3 Customer Site) của ThS. Bùi Thị Thanh Tú.
Công nghệ: ASP.NET MVC 5, Entity Framework 6, SQL Server, Bootstrap 3, PagedList.Mvc, Google Chart.

## Cách chạy

1. Chạy lần lượt các file trong thư mục `Database/` (trên SQL Server):
   1. `00_CreateDatabase_MyStore.sql` – tạo database `MyStore`
   2. `database2024.sql` – file gốc của giảng viên (tạo bảng), không chỉnh sửa
   3. **`02_UpdateOrderTables.sql`** – cập nhật bảng theo slide "Trang Đặt hàng – Model – DB":
      thêm `Order.DeliveryMethod`, `Order.PaymentMethod`; đổi `AddressDelivery` → `ShippingAddress`;
      thêm `OrderDetail.TotalPrice AS (Quantity * UnitPrice) PERSISTED`
   4. `01_SeedData_optional.sql` – dữ liệu mẫu (tuỳ chọn, chạy sau bước 3)
2. Sửa chuỗi kết nối trong `Web.config` cho đúng tên server (SQLEXPRESS hoặc localhost).
3. Mở `.sln` bằng Visual Studio, restore NuGet (EntityFramework, PagedList, PagedList.Mvc), nhấn F5.
4. Tài khoản mẫu: xem `01_SeedData_optional.sql`. Admin đăng nhập tại `/Admin`.

## Chức năng

**Customer**: Customer Layout (`_CustomerLayout`, `_CategoriesPV`, `_PersonalMenuPV`) • Trang Home (banner, tìm kiếm, sản phẩm nổi bật dạng slider,
20 sản phẩm ít người mua nhất phân trang 6/trang) • Danh sách theo danh mục (tìm kiếm, lọc giá, sắp xếp, phân trang) •
Chi tiết sản phẩm (số lượng đã bán, sản phẩm tương tự dạng slider, Top Deals 3 sp/trang) • Giỏ hàng nâng cao (nhóm theo danh mục, tick chọn mua, sản phẩm tương tự) •
Đặt hàng (chỉ đặt các sản phẩm đã tick; lưu hình thức giao hàng/thanh toán) • Xác nhận đơn hàng • Đơn hàng của tôi (tab trạng thái, tìm kiếm, Xem chi tiết, Mua lại, Huỷ) •
Đăng ký / Đăng nhập / Đăng xuất / Thông tin tài khoản / Đổi mật khẩu • PayPal (optional, mô phỏng).

**Admin**: Admin Layout • CRUD Danh mục • CRUD Sản phẩm (upload ảnh, tìm kiếm, lọc giá, sắp xếp, phân trang) •
Khách hàng (xem, tìm kiếm, sắp xếp, phân trang; đơn hàng và chi tiết đơn của một khách) • CRUD tài khoản Admin (tìm kiếm, phân trang) • Đăng nhập/Đăng xuất • Trang Home thống kê bằng Google Chart.

## Lưu ý

- Model được viết sẵn theo kiểu code (không có file `.edmx`). Nếu giảng viên yêu cầu bắt buộc EDMX (Database First):
  xoá 6 lớp entity + `MyStoreEntities.cs` trong `Models/`, thêm *ADO.NET Entity Data Model* tên `MyStoreModel`
  (EF Designer from Database), rồi đổi chuỗi kết nối sang dạng EDMX.
- PayPal chỉ là mô phỏng luồng thanh toán; muốn chạy thật cần `Install-Package PayPal` và điền ClientId/Secret trong `Web.config`.
- Đơn hàng chỉ xem/huỷ/mua lại được bởi đúng chủ đơn.
