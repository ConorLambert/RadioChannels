function validatePassword() {
    if (!passwordsMatch()) {            
        $(".password-success").text("");
        $(".password-error").text("Passwords Don't Match");
        return false;
    } else {
        $(".password-error").text("");
        $(".password-success").text("Passwords Match");
    }
    return true;
}

function isAllFieldsEntered() {    
    var result = true;
    $(".validate").each(function () {
        if ($(this).val() === '' || $(this).val().length === 0) {
            result = false;
            return false;
        }
    }); 
    return result;
}

function passwordsMatch() {
    return (($("#password").val().length > 0 && $("#confirm_password").val().length > 0) && ($("#password").val() === $("#confirm_password").val()));
}

function isValid() {   
    return isAllFieldsEntered() && passwordsMatch();;
}

function validateForm() {
    if ($("#submit").val() === "Log In") {
        if (isAllFieldsEntered()) {
            $('#submit').prop('disabled', false);
        } else {
            $('#submit').prop('disabled', true);
        }
    } else {
        if (isValid()) {
            $('#submit').prop('disabled', false);
        } else {
            $('#submit').prop('disabled', true);
        }
    }
}

function ActionOnReady() { 
    $('#submit').prop('disabled', true);    
    $("#password #confirm-password").on('change', validatePassword);
    $('.validate').keyup(validateForm);  
    $('.validate-password').keyup(function () {
        $('#submit').prop('disabled', true);
        if (validatePassword()) {
            if ($(".validate").val() != '') // can we enable the submit button
                $('#submit').prop('disabled', false);
        }
    });      
}

$(document).ready(ActionOnReady);