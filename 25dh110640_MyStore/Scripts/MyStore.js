/* ===================== MyStore.js - Các hàm JS dùng chung ===================== */

// Xác nhận trước khi xoá
function confirmDelete(message) {
    return confirm(message || "Bạn có chắc chắn muốn xoá?");
}

// Tăng / giảm số lượng trên trang chi tiết sản phẩm
function changeQuantity(delta) {
    var input = document.getElementById("quantity");
    if (!input) return;
    var value = parseInt(input.value || "1") + delta;
    if (value < 1) value = 1;
    input.value = value;
    updateSubTotal();
}

// Tính lại tạm tính (đơn giá x số lượng)
function updateSubTotal() {
    var input = document.getElementById("quantity");
    var priceEl = document.getElementById("unitPrice");
    var totalEl = document.getElementById("subTotal");
    if (!input || !priceEl || !totalEl) return;

    var price = parseFloat(priceEl.getAttribute("data-price"));
    var qty = parseInt(input.value || "1");
    var total = price * qty;
    totalEl.innerText = total.toLocaleString("vi-VN") + " đ";
}

// Bật chế độ chỉnh sửa địa chỉ giao hàng ở trang Thanh toán
function enableAddressEdit() {
    var box = document.getElementById("newAddressBox");
    if (box) box.style.display = "block";
}

// Gán địa chỉ mới vào trường ShippingAddress (cột Order.ShippingAddress) trước khi submit
function updateShippingAddress() {
    var input = document.getElementById("newShippingAddress");
    var hidden = document.getElementById("ShippingAddress");
    if (input && hidden && input.value.trim() !== "") {
        hidden.value = input.value.trim();
    }
    return true;
}

// Chọn / bỏ chọn tất cả sản phẩm trong giỏ hàng
function toggleAll(source) {
    var boxes = document.querySelectorAll(".item-check");
    for (var i = 0; i < boxes.length; i++) boxes[i].checked = source.checked;
    updateCartSelection();
}

// Tính lại số mặt hàng và tổng tiền của các sản phẩm đang được tick chọn mua
function updateCartSelection() {
    var boxes = document.querySelectorAll(".item-check");
    var total = 0, count = 0;
    for (var i = 0; i < boxes.length; i++) {
        if (boxes[i].checked) {
            total += parseFloat(boxes[i].getAttribute("data-total") || "0");
            count += parseInt(boxes[i].getAttribute("data-qty") || "0");
        }
    }
    var totalEl = document.getElementById("selectedTotal");
    var countEl = document.getElementById("selectedCount");
    if (totalEl) totalEl.innerText = total.toLocaleString("vi-VN") + " đ";
    if (countEl) countEl.innerText = count;
}

// Gắn danh sách sản phẩm đã chọn (selectedIds) vào form "Mua hàng"
function prepareBuyForm() {
    var boxes = document.querySelectorAll(".item-check:checked");
    if (boxes.length === 0) {
        alert("Vui lòng chọn ít nhất một sản phẩm để mua.");
        return false;
    }
    var holder = document.getElementById("buyIds");
    holder.innerHTML = "";
    for (var i = 0; i < boxes.length; i++) {
        var input = document.createElement("input");
        input.type = "hidden";
        input.name = "selectedIds";
        input.value = boxes[i].value;
        holder.appendChild(input);
    }
    return true;
}

// Slider ngang cho phần Sản phẩm nổi bật / Sản phẩm tương tự: cuộn sang trái (-1) hoặc phải (1)
function slideProducts(btn, dir) {
    var track = btn.parentNode.querySelector(".slider-track");
    if (track) track.scrollBy({ left: dir * track.clientWidth * 0.8, behavior: "smooth" });
}
