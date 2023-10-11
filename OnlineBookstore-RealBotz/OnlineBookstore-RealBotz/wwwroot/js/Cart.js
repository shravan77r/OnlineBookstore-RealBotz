
function fnLoadList() {
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Home/GetCartList",
        dataType: "json",
        data: {},
        success: function (data) {
            let GrandTotal = 0;
            let html = '';
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
                html = `<tr>
                            <td colspan="7" style="text-align: center;">
                                    No data found.
                            </td>
                       </tr>`;
                $("#btn-ordernow").hide();
            }
            else {
                let i = 0;
                data.data.forEach(item => {
                    let total = item.price * item.quantity;
                    GrandTotal += total;
                    let QtyHtml = '';
                    if (item.quantity == 1) {
                        QtyHtml += '<button class="decrement" disabled onclick="fnUpdateCartQty(' + item.id +', 0);">-</button>';
                        QtyHtml += '<span>' + item.quantity +'</span>';
                        QtyHtml += '<button class="increment" onclick="fnUpdateCartQty(' + item.id +', 1);">+</button>';
                    }
                    else {
                        QtyHtml += '<button class="decrement" onclick="fnUpdateCartQty(' + item.id +', 0);">-</button>';
                        QtyHtml += '<span>' + item.quantity +'</span>';
                        QtyHtml += '<button class="increment" onclick="fnUpdateCartQty(' + item.id +', 1);">+</button>';
                    }

                    html += `<tr>
                        <td>${(i+1)}</td>
                        <td>${item.title}</td>
                        <td>${item.description}</td>
                        <td>${item.price}</td>
                        <td>
                            ${QtyHtml}
                        </td>
                        <td>${total}</td>
                        <td class="text-center">
                            <button type="button" class="btn btn-danger btn-sm" onclick="DeleteCartItem(${item.id}, ${item.quantity});" title="Delete">Delete</button>
                        </td>
                    </tr>`;
                    i++;
                });
            }
            $("#cart-tbody").html(html);
            $("#GrandTotal").text(GrandTotal);
            $(".loader-container").hide();
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        }
    });
}
fnLoadList();

function fnUpdateCartQty(CartId, Type) {
    $.ajax({
        type: "POST",
        url: "/Home/UpdateCartQty",
        dataType: "json",
        data: { CartId: CartId, Type: Type },
        success: function (data) {
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
            }
            else {
                toastr.success(data.message, data.messageType);
            }
        },
        error: function (response) {
            console.log(response);
        },
        complete: function () {
            fnLoadList();
        }
    });
}

function DeleteCartItem(Id, Qty) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Are you sure you want to delete this record?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    })
        .then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "Post",
                    url: "/Home/DeleteCartItem",
                    data: {
                        Id: Id, Qty: Qty
                    },
                    success: function (data) {
                        if (data.status == 0) {
                            toastr.error(data.message, data.messageType);
                        }
                        else {
                            cartCount = cartCount - 1;
                            $('.cart-count').text(cartCount);
                            toastr.success(data.message, data.messageType);
                        }
                    },
                    error: function (response) {
                        console.log(response);
                    },
                    complete: function () {
                        fnLoadList();
                    }
                });
            }
        });
}