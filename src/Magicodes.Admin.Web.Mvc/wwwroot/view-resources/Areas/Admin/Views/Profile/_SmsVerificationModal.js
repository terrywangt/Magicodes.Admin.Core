(function () {
    app.modals.SmsVerificationModal = function () {

        var _profileService = abp.services.app.profile;

        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var $modal = _modalManager.getModal();

            _$form = $modal.find('form[name=SmsVerificationModalForm]');
            _$form.validate();

            var $verifyButton = $modal.find('#verifyButton');

            $verifyButton.click(function () {
                if (!_$form.valid()) {
                    return;
                }

                var code = _$form.serializeFormToObject();
                _profileService.verifySmsCode(code)
                    .done(function () {
                        _modalManager.setResult(null);
                        _modalManager.close();
                    });
            });
        };
    };
})();