
async function deleteProduct(productId) {
    const isConfirmed = confirm('Bạn có chắc muốn xóa đi không?');
    if (!isConfirmed) return; 

    const response = await fetch('/Manager/DeleteProduct', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(productId) 
    });

    const result = await response.json();
    if (result.success) {
        const row = document.getElementById(`product-row-${productId}`);

        const cells = row.querySelectorAll('td:not(:last-child)'); 
        cells.forEach(cell => {
            cell.classList.add('dimmed');
        });
        const image = row.querySelector('img');
        image.classList.add('dimmed');
        document.getElementById(`restore-btn-${productId}`).style.display = 'inline-block';
        document.getElementById(`delete-btn-${productId}`).style.display = 'none';

        alert(result.message);
    } else {
        alert(result.message);
    }
}
async function restoreProduct(productId) {
    const isConfirmed = confirm('Bạn có chắc muốn khôi phục lại không?');
    if (!isConfirmed) return; 

    const response = await fetch('/Manager/RestoreProduct', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(productId) 
    });

    const result = await response.json();
    if (result.success) {
        const row = document.getElementById(`product-row-${productId}`);
        row.classList.remove('dimmed');

        const cells = row.querySelectorAll('td');
        cells.forEach(cell => cell.classList.remove('dimmed'));

        const image = row.querySelector('img');
        image.classList.remove('dimmed');

        document.getElementById(`restore-btn-${productId}`).style.display = 'none';
        document.getElementById(`delete-btn-${productId}`).style.display = 'inline-block';

        alert(result.message);
    } else {
        alert(result.message);
    }
}


