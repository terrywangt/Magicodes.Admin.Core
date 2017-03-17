var CurrentPage = function() {

    var handleEmailActivation = function() {

        var $form = $('.email-activation-form');

        $form.validate();

        $form.find('input').keypress(function(e) {
            if (e.which === 13) {
                if ($('.forget-form').valid()) {
                    $('.forget-form').submit();
                }
                return false;
            }
        });

        $form.submit(function(e) {
            e.preventDefault();

            if (!$form.valid()) {
                return;
            }
            
            abp.ui.setBusy(
                null,
                abp.ajax({
                    contentType: app.consts.contentTypes.formUrlencoded,
                    url: $form.attr('action'),
                    data: $form.serialize()
                }).done(function() {
                    abp.message.success(app.localize('ActivationMailSentMessage'), app.localize('MailSent'))
                        .done(function() {
                            location.href = abp.appPath + 'Account/Login';
                        });
                })
            );
        });
    }

    return {
        init: function() {
            handleEmailActivation();
        }
    };
}();