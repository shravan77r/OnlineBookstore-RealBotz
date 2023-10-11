
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

    $("#registrationForm").submit(function (event) {
        event.preventDefault();
        if (!$('#FirstName').val()) {
            toastr.error("Please enter First Name", "Error");
            return false;
        }
        if (!$('#LastName').val()) {
            toastr.error("Please enter Last Name", "Error");
            return false;
        }
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
        if (!$('#MobileNo').val()) {
            toastr.error("Please enter Mobile No", "Error");
            return false;
        }
        if (!$('#Gender').val()) {
            toastr.error("Please Select Gender", "Error");
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
            url: "/Account/Signup/",
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
                    setTimeout(function () {
                        window.location.href = "/index.html";
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

function fnCheckEmailExist() {
    let Email = $('#Email').val();
    if (!Email) {
        toastr.error("Please enter email address", "Error");
        $('#Email').focus();
        return false;
    }
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    if (!emailPattern.test(Email)) {
        toastr.error("Please enter a valid email address.", "Error");
        $('#Email').focus();
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Account/CheckEmailIdExist/",
        data: { Email: Email},
        datatype: "json",
        success: function (data) {
            if (data) {
                toastr.error("Email address already exist", "Error");
                $('#Email').focus();
            }
        },
    });
}

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
