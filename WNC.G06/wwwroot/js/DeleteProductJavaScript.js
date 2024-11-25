function deleteProduct(productId) {
    // Xác nhận xóa sản phẩm
    if (confirm("Bạn có chắc chắn muốn xóa sản phẩm này?")) {
        $.ajax({
            type: 'POST',
            url: '/Product/DeleteProduct',  // Đường dẫn đến action trong controller
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Cập nhật giao diện, thay đổi trạng thái sản phẩm
                    $('#product-row-' + response.productId).removeClass('active').addClass('inactive');
                    $('#product-row-' + response.productId + ' .buttonDele').prop('disabled', true);  // Vô hiệu hóa nút "Xóa"
                    $('#product-row-' + response.productId + ' .buttonRestore').prop('disabled', false);  // Kích hoạt nút "Khôi phục"
                } else {
                    alert("Đã xảy ra lỗi khi xóa sản phẩm.");
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi khi thực hiện yêu cầu.");
            }
        });
    }
}

function restoreProduct(productId) {
    // Xác nhận khôi phục sản phẩm
    if (confirm("Bạn có chắc chắn muốn khôi phục sản phẩm này?")) {
        $.ajax({
            type: 'POST',
            url: '/Product/RestoreProduct',  // Action để khôi phục sản phẩm
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Cập nhật giao diện, thay đổi trạng thái sản phẩm
                    $('#product-row-' + response.productId).removeClass('inactive').addClass('active');
                    $('#product-row-' + response.productId + ' .buttonDele').prop('disabled', false);  // Kích hoạt nút "Xóa"
                    $('#product-row-' + response.productId + ' .buttonRestore').prop('disabled', true);  // Vô hiệu hóa nút "Khôi phục"
                } else {
                    alert("Đã xảy ra lỗi khi khôi phục sản phẩm.");
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi khi thực hiện yêu cầu.");
            }
        });
    }
}
