var CurrentPage = function () {

    function setPayment() {
        var $periodType = $('input[name=PaymentPeriodType]:checked');
        $('input[name=DayCount]').val($periodType.data('day-count'));
    }

    function handleRegister() {

        $('input[name=PaymentPeriodType]').change(function () {
            setPayment();
        });

        $('input[name=PaymentPeriodType]:first').prop('checked', true);

        setPayment();
    };

    function init() {
        handleRegister();
    }

    return {
        init: init
    };
}();