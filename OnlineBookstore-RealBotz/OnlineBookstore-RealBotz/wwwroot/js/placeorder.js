
var validdata = 1;

$(document).ready(function () {
    $("#CountryId").change(function () {
        $.ajax({
            type: "GET",
            url: "/Account/GetState/",
            data: { cid: $("#CountryId").val() },
            datatype: "json",
            success: function (data) {
                $("#StateId").empty();
                $("#StateId").append("<option value=''>Select State</option>");
                $.each(data.data, function (key, value) {
                    $("#StateId").append("<option value= '" + value.stateId + "'>" + value.stateName + "</option>");
                });
            },
        });
    });

    $("#StateId").change(function () {
        $.ajax({
            type: "GET",
            url: "/Account/GetCity/",
            data: { sid: $("#StateId").val() },
            datatype: "json",
            success: function (data) {
                $("#CityId").empty();
                $("#CityId").append("<option value=''>Select City</option>");
                $.each(data.data, function (key, value) {
                    $("#CityId").append("<option value= '" + value.cityId + "'>" + value.cityName + "</option>");
                });
            },
        });
    });

    $("#OrderForm").submit(function (event) {
        event.preventDefault();

        if (validdata == 0) {
            toastr.error("There is no data in cart to order", "Error");
            return false;
        }
        if (!$('#Name').val()) {
            toastr.error("Please Enter Name", "Error");
            return false;
        }
        if (!$('#CountryId').val()) {
            toastr.error("Please Select Country", "Error");
            return false;
        }
        if (!$('#StateId').val()) {
            toastr.error("Please Select State", "Error");
            return false;
        }
        if (!$('#CityId').val()) {
            toastr.error("Please Select City", "Error");
            return false;
        }
        if (!$('#PinCode').val()) {
            toastr.error("Please Enter PinCode", "Error");
            return false;
        }
        if (!$('#Address').val()) {
            toastr.error("Please Enter Address", "Error");
            return false;
        }

        var formData = new FormData(this);
        $(".loader-container").show();
        $.ajax({
            url: "/Home/PlaceOrder/",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                $(".loader-container").hide();
                if (data.status == 0) {
                    toastr.error(data.message, data.messageType);
                }
                else {
                    toastr.success(data.message, data.messageType);
                    cartCount = 0;
                    $('.cart-count').text(cartCount);

                    setTimeout(function () {
                        window.location.href = "/myorderlist.html";
                    }, 2000);
                }
            },
            error: function (response) {
                $(".loader-container").hide();
                console.log(response);
            },
        });
    });
});

function GetCountry() {
    $.ajax({
        type: "GET",
        url: "/Account/GetCountry/",
        data: {},
        datatype: "json",
        success: function (data) {
            $("#CountryId").empty();
            $("#CountryId").append("<option value=''>Select Country</option>");
            $.each(data.data, function (key, value) {
                $("#CountryId").append("<option value= '" + value.countryId + "'>" + value.countryName + "</option>")
            });
        },
    });
}
GetCountry();

function fnLoadList() {
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Home/GetCartList",
        dataType: "json",
        data: {},
        success: function (data) {
            $(".loader-container").hide();
            let GrandTotal = 0;
            let html = '';
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
                html = `<tr>
                            <td colspan="6" style="text-align: center;">
                                    No data found.
                            </td>
                       </tr>`;
                validdata = 0;
            }
            else {
                let i = 0;
                data.data.forEach(item => {
                    let total = item.price * item.quantity;
                    GrandTotal += total;
                    html += `<tr>
                        <td>${(i+1)}</td>
                        <td>${item.title}</td>
                        <td>${item.description}</td>
                        <td>${item.price}</td>
                        <td>
                            <span>${item.quantity}</span>
                        </td>
                        <td>${total}</td>
                    </tr>`;
                    i++;
                });
            }
            $("#cart-tbody").html(html);
            $("#GrandTotal").text(GrandTotal);
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        }
    });
}
fnLoadList();