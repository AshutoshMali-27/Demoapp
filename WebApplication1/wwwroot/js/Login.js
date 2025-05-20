$(document).ready(function () {
    clearAllCookies();
    removeLocalStorage('UserName');
    removeLocalStorage('PassWord');
    //removeSessionStorage('UserName');
    //removeSessionStorage('PassWord');

    // Clear Specific Cookies
    deleteCookie('UserName');
    deleteCookie('PassWord');
    $('#txtUserName').keydown(function (event) {
        var keyPressed = event.which;
        if (keyPressed === 13) {
            event.preventDefault();
            $('#pwd').focus();
        }
    });
    $('#pwd').keydown(function (event) {
        var keyPressed = event.which;
        if (keyPressed === 13) {
            event.preventDefault();
            $('#btnlogin').focus();
        }
    });
    $('#btnlogin').click(function () {
        if ($('#txtUserName').val().trim() == "") {
            swal('Error', 'Please Enter User Name !', 'error').then(() => {
                clearForm();
            });
            return false;
        }
        if ($('#pwd').val().trim() == "") {
            swal('Error', 'Please Enter Password !', 'error').then(() => {
                clearForm();
            });
            return false;
        }
        login();
    });
    $('#pwd').on('change', function (event) {
        var input = $(this).val();
        if (input != null && input != '') {
            GetEncryptString(input)
        }
    });
    $('#pwd').on('keydown keyup', function (event) {
        checkCapsLock(event);
    });
    $('#pwd').on('focus', function () {
        const event = new KeyboardEvent('keyup', { bubbles: true });
        checkCapsLock(event);
    });
})
function removeLocalStorage   (key) {
    localStorage.removeItem(key);
};
//// Clear Session Storage using jQuery
//function removeSessionStorage  (key) {
//    sessionStorage.removeItem(key);
//};

// Clear Cookies using jQuery
function deleteCookie  (name) {
    document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:01 GMT;path=/;';
};
function clearAllCookies () {
    document.cookie.split(";").forEach(function (c) {
        var cookieName = c.trim().split("=")[0];
        deleteCookie(cookieName);
    });
};
function login() {
    var data = {
        UserName: $("#txtUserName").val(),
        PassWord: $("#pwd").val()
    }
    var url = "/Login/LoginUser?" + $.param(data);
    $.ajax({
        type: "Get",
        url: url,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            data = JSON.parse(data);
            var Id = data.userDetails[0].id;
            var UTypeID = data.userDetails[0].uTypeID;
            var message = data.userDetails[0].message;
            var passChangeRemainingDay = data.userDetails[0].passChangeRemainingDay;
            if (message.includes("User is Blocked")) {
                $('#div_blockUserNotification').show();
                $('#lbl_blockUserNotification').text(message)
                clearForm();
            } else if (message.includes("Invalid Username and Password")) {
                swal('Error', 'User Name Or Password Is Incorrect...!', 'error');
                isValid = false;
                clearForm();
                $('#div_blockUserNotification').hide();
            }
            else if (message.includes("Charge handover has not been provided to this user")) {
                swal('Error', 'Charge handover has not been provided to this user...!', 'error');
                isValid = false;
                clearForm();
                $('#div_blockUserNotification').hide();
            }
            else if (passChangeRemainingDay <= 0) {
                swal('Alert', 'Request to change your password', 'warning').then(() => {
                    window.location.href = '/ChangeExpirePassword/Index';
                });
                $('#div_blockUserNotification').hide();
            } else if (Id > 0) {
                if (UTypeID == 2 || UTypeID == 3 || UTypeID == 4) {
                    window.location.href = '/dashboard/dashboard';
                }
                else {
                    window.location.href = '/Home/Index';

                }
            }
            else {
                swal('Error', 'User Name Or Password Is Incorrect...!', 'error');
                isValid = false;
                clearForm();
                $('#div_blockUserNotification').hide();
            }

        },
        failure: function (data) {
            swal('Error', 'Something went wrong', 'error');
            clearForm();
        },
        error: function (data) {
            swal('Error', 'Something went wrong', 'error');
            clearForm();
        }
    });
}
function clearForm() {
    $('#txtUserName').val('') // Clear User Name
    $('#pwd').val(''); // Clear Password
    $("#txtUserName").focus();
    $('#div_CapsLockNotification').hide();
    $('#lbl_CapsLockNotification').hide();
}
function GetEncryptString(input) {
    $.ajax({
        type: "Get",
        url: "/Login/GetEncryptString",
        data: {
            Input: input
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var pass = data.result;
            $('#pwd').val(pass);
        },
        failure: function (data) {
            swal('Error', 'Something went wrong', 'error');
            clearForm();
        },
        error: function (data) {
            swal('Error', 'Something went wrong', 'error');
            clearForm();
        }
    });
}
function checkCapsLock(event) {
    if (event.originalEvent.getModifierState('CapsLock')) {
        $('#div_CapsLockNotification').show();
        $('#lbl_CapsLockNotification').show();
    } else {
        $('#div_CapsLockNotification').hide();
        $('#lbl_CapsLockNotification').hide();
    }
}
