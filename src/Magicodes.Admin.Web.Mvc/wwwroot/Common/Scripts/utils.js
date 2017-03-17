var app = app || {};
(function () {

    app.utils = app.utils || {};

    app.utils.truncateString = function(str, maxLength, postfix) {
        if (!str || !maxLength || str.length <= maxLength) {
            return str;
        }

        if (postfix === false) {
            return str.substr(0, maxLength);
        }

        return str.substr(0, maxLength - 1) + '&#133;';
    }

})();