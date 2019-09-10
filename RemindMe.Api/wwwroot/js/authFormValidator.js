$(document).ready(function() {
    console.log("Binding event listeners...");
    $("#auth-form-button").on('click', function(event) {
        ValidateAuthForm(event);
    })
});

function ValidateAuthForm(event){
    console.log("ValidateAuthForm called");
    var validator = $("#auth-form").validate({
        invalidHandler: function(event, validator){
            event.preventDefault();
        },
        submitHandler: function(form)
        {
            form.submit();
        },
        rules: {
            //properties must match the name of the element and are case sensitive
            emailInput: {
                required: true,
                email: true
            },
            passwordInput: {
                required: true,
                PasswordStrengthCheck: true,
                minlength: 8,
                maxlength: 24
            },

            reEnterPasswordInput: {
                equalTo: "#passwordInput"
            },
            confirmationCodeInput: {
                required: true,
                minLength: 6,
                maxLength: 6
            }
        },

        messages: {
            emailInput: "Please enter a valid email address",
            passwordInput: "Password must be between 8 and 24 characters long, contain 1 uppercase letter, 1 lowercase letter, 1 number \
            and one of the symbols !@#$%^&*",
            reEnterPasswordInput: "Passwords must match",
            confirmationCodeInput: "Enter 6 digit code sent in an email"
        }
    });
    $.validator.addMethod("PasswordStrengthCheck", function(value) {
        return /[A-Z]+[a-z]+[0-9]+[!@#$%^&*]/.test(value) // consists of only these
     });
}