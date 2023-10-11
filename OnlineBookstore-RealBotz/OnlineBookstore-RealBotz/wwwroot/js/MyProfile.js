
function showTab(tabName) {
    // Hide all tab contents
    const tabContents = document.querySelectorAll('.tab-content');
    tabContents.forEach(tab => tab.classList.remove('active'));

    // Show the selected tab content
    const selectedTab = document.querySelector(`.${tabName}`);
    selectedTab.classList.add('active');

    // Highlight the selected tab
    const tabs = document.querySelectorAll('.tab');
    tabs.forEach(tab => tab.classList.remove('active'));
    event.target.classList.add('active');
}

$(document).ready(function () {

    $("#myform").submit(function (event) {
        $(".loader-container").show();
        event.preventDefault();
        var formData = new FormData(this);

        $.ajax({
            url: "/Account/UpdateProfile/",
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
                    //$("#mainuserimage").attr("src", data.profilePhoto);
                    //$("#profile-image").attr("src", data.profilePhoto);
                    //$("#file-input").val('');
                    window.location.reload();
                }
            },
            error: function (response) {
                $(".loader-container").hide();
                console.log(response);
            },
        });
    });

    $('#enable-2fa').change(function () {
        if (this.checked) {
            $('#enable-authenticator-container').show();
            fnSet2FAAuthFlag(true);
        } else {
            $('#enable-authenticator-container').hide();
            fnSet2FAAuthFlag(false);
        }
    });
    $("#6digcode").on("keyup", function () {
        $("#divError").remove();
        if (!$("#6digcode").val()) {
            $("#6digcode").after("<span id='divError' class='text-danger'>Please Enter code</span>");
        }
        else if ($("#6digcode").val().length > 6) {
            $("#6digcode").after("<span id='divError' class='text-danger'>Please Enter valid code</span>");
        }
        else if ($("#6digcode").val().length == 6) {
            $("#divError").remove();
            $("#6digcode").prop("disabled", true);
            VerifyQrcode();
        }
    });
});

function fnSet2FAAuthFlag(Flag) {
    $(".loader-container").show();
    $.ajax({
        url: "/Account/Set2FAAuthFlag/",
        type: "GET",
        data: { Flag: Flag },
        success: function (data) {
            $(".loader-container").hide();
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
            }
            else {
                if (Flag) {
                    $("#qrcodeImg").attr("src", data.data);
                }
                else {
                    toastr.success(data.message, data.messageType);
                }
            }
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        },
    });
}

function VerifyQrcode() {
    var pin = $("#6digcode").val();
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Account/ValidateTwoFactorPIN/",
        data: { pin: pin },
        success: function (data) {
            $(".loader-container").hide();
            if (data.status == 0) {
                $("#6digcode").prop("disabled", false);
                toastr.error(data.message, data.messageType);
            }
            else {
                $('#reccode').text(data.data);
                $("#6digcode").prop("disabled", true);
                $('#divStep3').show();
                toastr.success(data.message, data.messageType);
            }
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        },
    });
}
function download(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

function download1() {
    var code =  $('#reccode').html();
    
    download("recovery_code.txt", code);

}

