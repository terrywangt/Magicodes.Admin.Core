(function () {
    app.PasswordComplexityHelper = function () {

        function reviver(key, val) {
            if (key && key.charAt(0) !== key.charAt(0).toLowerCase())
                this[key.charAt(0).toLowerCase() + key.slice(1)] = val;
            else
                return val;
        };

        var _buildPasswordComplexityErrorMessage = function (setting) {
            var message = "<ul style='display: inline-block;'>";

            if (setting.requireDigit) {
                message += "<li>" + app.localize("PasswordComplexity_RequireDigit_Hint") + "</li>";
            }

            if (setting.requireLowercase) {
                message += "<li>" + app.localize("PasswordComplexity_RequireLowercase_Hint") + "</li>";
            }

            if (setting.requireNonAlphanumeric) {
                message += "<li>" + app.localize("PasswordComplexity_RequireNonAlphanumeric_Hint") + "</li>";
            }

            if (setting.useLowerCaseLetters) {
                message += "<li>" + app.localize("PasswordComplexity_UseLowerCaseLetters_Hint") + "</li>";
            }

            if (setting.requireUppercase) {
                message += "<li>" + app.localize("PasswordComplexity_RequireUppercase_Hint") + "</li>";
            }

            if (setting.requiredLength > 0) {
                message += "<li>" + abp.utils.formatString(app.localize("PasswordComplexity_RequiredLength_Hint"), setting.requiredLength) + "</li>";
            }

            return message + "</ul>";
        }

        var _setPasswordComplexityRules = function ($elements, setting) {
            if (!$elements) {
                return;
            }

            setting = JSON.parse(JSON.stringify(setting), reviver);

            if (setting) {
                var message = _buildPasswordComplexityErrorMessage(setting);

                jQuery.validator.addMethod("passwordComplexity", function (value, element) {
                    if (!element.hasAttribute("required") && value === "") {
                        return true;
                    }

                    if (setting.requireDigit && !/[0-9]/.test(value)) {
                        return false;
                    }

                    if (setting.requireLowercase && !/[a-z]/.test(value)) {
                        return false;
                    }

                    if (setting.requireUppercase && !/[A-Z]/.test(value)) {
                        return false;
                    }

                    if (setting.requiredLength && value.length < setting.requiredLength) {
                        return false;
                    }

                    if (setting.requireNonAlphanumeric && /[0-9a-zA-Z]+$/.test(value)) {
                        return false;
                    }

                    return true;
                }, message);

                for (var i = 0; i < $elements.length; i++) {
                    var $element = $($elements[i]);
                    $element.rules("add", "passwordComplexity");
                }

            }
        };

        return {
            setPasswordComplexityRules: _setPasswordComplexityRules
        };
    };
})();