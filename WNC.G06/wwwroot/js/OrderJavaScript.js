function addToCart(productElement) {
    // Lấy thông tin sản phẩm từ các thuộc tính data-*
    const productName = productElement.getAttribute("data-name");
    const productPrice = parseFloat(productElement.getAttribute("data-price"));

    // Xóa thông báo giỏ hàng trống (nếu có)
    const emptyCartMessage = document.getElementById("empty-cart-message");
    if (emptyCartMessage) emptyCartMessage.remove();

    // Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa
    const cartContainer = document.getElementById("tinhtien");
    let existingProduct = document.querySelector(`#tinhtien .sanpham[data-name="${productName}"]`);

    if (existingProduct) {
        // Nếu sản phẩm đã có, tăng số lượng
        let quantityInput = existingProduct.querySelector(".soluongsp");
        quantityInput.value = parseInt(quantityInput.value) + 1;

        // Cập nhật giá hiển thị
        updateProductTotal(quantityInput, productPrice);
    } else {
        // Nếu chưa, thêm sản phẩm mới
        const productHTML = `
            <div class="sanpham" data-name="${productName}">
                <span>${productName}</span>
                <input class="soluongsp" type="number" value="1" min="1" onchange="updateProductTotal(this, ${productPrice})">
                <span class="product-total">${productPrice.toLocaleString()} VND</span>
            </div>
        `;
        cartContainer.insertAdjacentHTML("beforeend", productHTML);
    }

    // Cập nhật tổng tiền
    updateCartTotal();
}

function updateProductTotal(quantityInput, productPrice) {
    const quantity = parseInt(quantityInput.value);
    const totalPrice = productPrice * quantity;

    const productElement = quantityInput.closest(".sanpham");
    productElement.querySelector(".product-total").textContent = `${totalPrice.toLocaleString()} VND`;

    // Cập nhật tổng tiền
    updateCartTotal();
}

function updateCartTotal() {
    const cartProducts = document.querySelectorAll("#tinhtien .sanpham");
    let total = 0;

    cartProducts.forEach((product) => {
        const productTotal = product.querySelector(".product-total").textContent.replace(/\D/g, "");
        total += parseFloat(productTotal);
    });

    // Cập nhật tổng tiền trong View
    document.getElementById("tientamtinh").textContent = `${total.toLocaleString()} VND`;
    document.getElementById("tongtien").textContent = `${total.toLocaleString()} VND`;
}

function submitOrder() {
    const cartProducts = document.querySelectorAll("#tinhtien .sanpham");
    if (cartProducts.length === 0) {
        alert("Giỏ hàng trống! Vui lòng thêm sản phẩm.");
        return;
    }

    const orderDetails = [];
    cartProducts.forEach((product) => {
        const productName = product.getAttribute("data-name");
        const quantity = parseInt(product.querySelector(".soluongsp").value);
        const totalPrice = product.querySelector(".product-total").textContent;

        orderDetails.push({
            name: productName,
            quantity: quantity,
            price: totalPrice,
        });
    });

    console.log("Order submitted:", orderDetails);
    alert("Đơn hàng đã được gửi!");
}
