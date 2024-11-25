
async function deleteCafe(cafeId) {
    const isConfirmed = confirm('Bạn có chắc muốn xóa đi không?');
    if (!isConfirmed) return;

    const response = await fetch('/Manager/DeleteCafe', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(cafeId)
    });

    const result = await response.json();
    if (result.success) {
        const row = document.getElementById(`cafe-row-${cafeId}`);

        const cells = row.querySelectorAll('td:not(:last-child)');
        cells.forEach(cell => {
            cell.classList.add('dimmed');
        });
        document.getElementById(`restore-btn-${cafeId}`).style.display = 'inline-block';
        document.getElementById(`delete-btn-${cafeId}`).style.display = 'none';

        alert(result.message);
    } else {
        alert(result.message);
    }
}
async function restoreCafe(cafeId) {
    const isConfirmed = confirm('Bạn có chắc muốn khôi phục lại không?');
    if (!isConfirmed) return;

    const response = await fetch('/Manager/RestoreCafe', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(cafeId)
    });

    const result = await response.json();
    if (result.success) {
        const row = document.getElementById(`cafe-row-${cafeId}`);
        row.classList.remove('dimmed');

        const cells = row.querySelectorAll('td');
        cells.forEach(cell => cell.classList.remove('dimmed'));;

        document.getElementById(`restore-btn-${cafeId}`).style.display = 'none';
        document.getElementById(`delete-btn-${cafeId}`).style.display = 'inline-block';

        alert(result.message);
    } else {
        alert(result.message);
    }
}


