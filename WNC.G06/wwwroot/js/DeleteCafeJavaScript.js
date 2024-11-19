function deleteProduct(productId) {
    if (confirm("Bạn có chắc chắn muốn xóa sản phẩm này?")) {
        $.ajax({
            type: "POST",
            url: '/Product/DeleteProduct',  // Gọi đến action DeleteProduct
            data: { productId: productId },  // Truyền productId đến controller
            success: function (response) {
                if (response.success) {
                    // Sau khi thay đổi trạng thái thành False, làm mờ sản phẩm và ẩn nút "Xóa"
                    $("#product-row-" + productId).find("td").addClass("dimmed");
                    $("#product-row-" + productId).find("button").hide();  // Ẩn nút "Xóa"
                    $("#product-row-" + productId).find(".buttonRestore").show();  // Hiển thị nút "Khôi phục"
                }
            },
            error: function () {
                alert("Đã có lỗi xảy ra. Vui lòng thử lại.");
            }
        });
    }
}
