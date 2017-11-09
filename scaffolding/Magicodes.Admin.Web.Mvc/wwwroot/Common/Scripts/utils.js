var app = app || {};
(function () {

    app.utils = app.utils || {};

    app.utils.string = {
        truncate: function (str, maxLength, postfix) {
            if (!str || !maxLength || str.length <= maxLength) {
                return str;
            }

            if (postfix === false) {
                return str.substr(0, maxLength);
            }

            return str.substr(0, maxLength - 1) + '&#133;';
        }
    }

    app.utils.date = {
        containsTime: function (date) {
            if (!date) {
                return false;
            }

            return date.indexOf(":") !== -1;
        },

        getEndOfDay: function (date) {
            if (!date || !moment) {
                return null;
            }

            return moment(date).endOf('day');
        },

        getEndOfDayIfTimeNotExists: function (date) {
            if (this.containsTime(date)) {
                return date;
            }

            return this.getEndOfDay(date);
        },

        formatAsLongDateTime: function (date) {
            return moment(date).format("YYYY-MM-DDTHH:mm:ss.SSS[Z]");

        }
    }


})();