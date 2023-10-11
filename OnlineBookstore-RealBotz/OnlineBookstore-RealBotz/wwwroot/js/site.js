var cartCount = 0;

$(document).ready(function () {

    $('#logout-link').click(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Account/Logout',
            type: 'POST',
            data: {},
            success: function (response) {
                window.location.href = "/index.html";
            },
            error: function (error) {
                console.error(error);
            }
        });
    });
});

function fnGetUserDetails() {
    $.ajax({
        type: "POST",
        url: "/Home/GetUserDetails",
        dataType: "json",
        data: {},
        success: function (data) {
            if (data.status == 0) {
                $(".loader-container").hide();
                //toastr.error(data.message, "Error");
                window.location.href = "/index.html?message=" + data.message;
            }
            else {
                
                $("#mainuserimage").attr("src", data.profilePhoto);
                $("#profile-image").attr("src", data.profilePhoto);

                if (data.profilePhoto) {
                    var segments = data.profilePhoto.split('/');
                    var fileName = segments[segments.length - 1];
                    $("#OldProfileImage").val(fileName);

                }
                
                cartCount = data.cartCount;
                $('.cart-count').text(data.cartCount);
                $('.username').text(data.userName);
                if (data.is2FAEnable == 1) {
                    $("#enable-2fa").prop('checked', true);
                }
                //$(".loader-container").hide();
            }
        },
        error: function (response) {
            //$(".loader-container").hide();
            console.log(response);
        }
    });
}
setTimeout(function () {
    fnGetUserDetails();
}, 1000);

function fnAddToCart(BookId) {
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Home/AddBookToCart",
        dataType: "json",
        data: { BookId: BookId, Quantity: 1 },
        success: function (data) {
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
            }
            else {
                toastr.success(data.message, data.messageType);
                cartCount++;
                $('.cart-count').text(cartCount);
            }
            $(".loader-container").hide();
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        }
    });
};

$(document).on('keypress', '.numI', function (event) {
    var charCode = event.which ? event.which : event.keyCode;
    if (charCode > 47 && charCode < 58)
        return true;
    return false;
});

$(document).on('keypress', '.varchar', function (e) {
    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
    var ret = ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode == 32) || (keyCode > 47 && keyCode < 58));

    return ret;
});



