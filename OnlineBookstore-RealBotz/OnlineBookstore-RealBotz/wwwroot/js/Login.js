
document.addEventListener("DOMContentLoaded", function () {
    var message = decodeURIComponent(new URLSearchParams(window.location.search).get("message"));
    if (message && message != "null") {
        toastr.error(message, "Error");
    }
});

function fnLogin() {
    let Email = $('#Email').val();
    if (!Email) {
        toastr.error("Please enter email address", "Error");
        return false;
    }
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    if (!emailPattern.test(Email)) {
        toastr.error("Please enter a valid email address.", "Error");
        return false;
    }
    let Password = $('#Password').val();
    if (!Password) {
        toastr.error("Please enter password", "Error");
        return false;
    }
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Account/Login/",
        data: {
            Email: Email,
            Password: Password,
        },
        success: function (data) {
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
            }
            else {
                if (data.data.is2FAEnable) {
                    $("#twofactor").modal("show");
                    $("#divverifyrecovery").hide();
                    $("#btnRecovery").show();
                }
                else {
                    fnSetSession();
                }
            }
            $(".loader-container").hide();
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        }
    });
}
function recoveryclick() {
    $("#btnRecovery").hide();
    $("#divverifyapp").hide();
    $("#txtverifyapp").val("");
    $("#txtverifyrecovery").val("");
    $("#divverifyrecovery").show();
}
function backclick() {
    $("#btnRecovery").show();
    $("#divverifyapp").show();
    $("#txtverifyapp").val("");
    $("#txtverifyrecovery").val("");
    $("#divverifyrecovery").hide();
}
function fnSetSession() {
    let Email = $('#Email').val();
    let Password = $('#Password').val();
    $.ajax({
        type: "POST",
        url: "/Account/SetSession/",
        data: {
            Email: Email,
            Password: Password,
        },
        success: function (data) {
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
            }
            else {
                window.location.href = "/BookList.html";
            }
        },
        error: function (response) {
            console.log(response);
        },
    });
}

var lastCode = "";
function appverify() {
    $("#divError").remove();
    if (!$("#txtverifyapp").val()) {
        $("#txtverifyapp").after("<span id='divError' class='text-danger'>Please Enter Authontication Code</span>")
    } else if ($("#txtverifyapp").val().length != 6) {
        $("#txtverifyapp").after("<span id='divError' class='text-danger'>Please Enter 6 digit Authontication Code</span>")

    } else if ($("#txtverifyapp").val().length == 6) {
        $("#divError").remove();
        $("#txtverifyrecovery").val("");
        if (lastCode != $("#txtverifyapp").val())
            VerifyLoginOTP();
        lastCode = $("#txtverifyapp").val()
    }
}
function recoveryverify() {
    $("#divError").remove();
    if (!$("#txtverifyrecovery").val()) {
        $("#txtverifyrecovery").after("<span id='divError' class='text-danger'>Please Enter Recovery Code</span>")
    }
    else if ($("#txtverifyrecovery").val().length == 36) {
        $("#divError").remove();
        $("#txtverify").val("");
        if (lastCode != $("#txtverifyrecovery").val()) {
            VerifyLoginOTP();
        }
        lastCode = $("#txtverifyrecovery").val();
    }
}
function VerifyLoginOTP() {
    let Email = $('#Email').val();
    let Password = $('#Password').val();
    let OTP = '';
    if ($("#txtverifyrecovery").val() == "") {
        OTP = $("#txtverifyapp").val();
    } else {
        OTP = $("#txtverifyrecovery").val();
    }
    $("#txtverifyapp").prop("disabled", true);
    $("#txtverifyrecovery").prop("disabled", true);
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Account/VerifyLoginOTP2FA/",
        data: { OTP: OTP, Email: Email, Password: Password },
        success: function (data) {
            if (data.status == 0) {
                $("#txtverifyapp").prop("disabled", false);
                $("#txtverifyrecovery").prop("disabled", false);
                toastr.error(data.message, data.messageType);
            }
            else {
                toastr.success(data.message, data.messageType);
                fnSetSession();
            }
        },
        error: function (response) {
            console.log(response);
        },
        complete: function (response) {
            $(".loader-container").hide();
        }
    });
}