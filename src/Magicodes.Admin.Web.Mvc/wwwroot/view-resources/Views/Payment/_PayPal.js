(function () {

    function preparePaypalButton() {
        window.paypal.request.addHeaderBuilder(() => {
            return { 'X-XSRF-TOKEN': abp.security.antiForgery.getToken() };
        });
        
        window.paypal.Button.render({
            style: { size: 'responsive' },
            env: $('#paypal-environment').val(),
            commit: true,
            payment: function () {
                var paymentPeriodType = getPaymentPeriodType();
                var input = {
                    editionId: $('input[name=EditionId]').val(),
                    editionPaymentType: $('input[name=EditionPaymentType]').val(),
                    gateway: 'PayPal'
                };
                
                if (paymentPeriodType) {
                    input.paymentPeriodType = paymentPeriodType;
                }

                return window.paypal.request.post(abp.appPath + 'Payment/CreatePayment', input).then(function (response) {
                    return response.result.id;
                });
            },
            onAuthorize: function (data) {
                finishPayment(data);
            }
        }, '#paypal-button');
    }

    function getPaymentPeriodType() {
        var $input = $('input[name=PaymentPeriodType]');
        if (!$input) {
            return null;
        }
        if ($input.attr('type') === 'radio') {
            return $('input[name=PaymentPeriodType]:checked').val();
        }

        return $input.val();
    }

    function finishPayment(data) {
        $('input[name=PaymentGatewayState]').val('created');
        $('input[name=PaymentId]').val(data.paymentID);
        $('input[name=PayerId]').val(data.payerID);
        $('input[name=Gateway]').val("PayPal");
        
        $('#formPaymentResult').submit();
    }

    preparePaypalButton();

})();