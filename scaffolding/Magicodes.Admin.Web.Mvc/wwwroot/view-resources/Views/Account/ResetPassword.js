var CurrentPage = function () {

    var _passwordComplexityHelper = new app.PasswordComplexityHelper();

    var handleResetPassword = function () {

        $('.pass-reset-form').validate({
            rules: {
                PasswordRepeat: {
                    equalTo: "#Password"
                }
            },

            submitHandler: function (form) {
                form.submit();
            }
        });

        $('.pass-reset-form input').keypress(function (e) {
            if (e.which === 13) {
                if ($('.pass-reset-form').valid()) {
                    $('.pass-reset-form').submit();
                }

                return false;
            }
        });

        _passwordComplexityHelper.setPasswordComplexityRules($('input[name=Password],input[name=PasswordRepeat]'), window.passwordComplexitySetting);
    }

    return {
        init: function () {
            handleResetPassword();
        }
    };

}();