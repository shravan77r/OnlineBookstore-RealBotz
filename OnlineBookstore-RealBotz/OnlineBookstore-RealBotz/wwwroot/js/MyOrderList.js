
function fnLoadList() {
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Home/GetMyOrdersList",
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
            }
            else {
                let i = 0;
                data.data.forEach(item => {
                    html += `<tr>
                        <td>${(i+1)}</td>
                        <td>${item.id}</td>
                        <td>${item.orderDate}</td>
                        <td>${item.totalQty}</td>
                        <td>${item.totalAmount}</td>
                        <td>${item.name}</td>
                        <td>${item.paymentStatus}</td>
                    </tr>`;
                    i++;
                });
            }
            $("#orders-tbody").html(html);
            $(".loader-container").hide();
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        },
        complete: function () {
            $(".loader-container").hide();
        }
    });
}
fnLoadList();