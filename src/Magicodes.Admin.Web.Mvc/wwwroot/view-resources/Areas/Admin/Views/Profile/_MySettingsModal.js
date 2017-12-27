(function () {
    app.modals.MySettingsModal = function () {

        var _profileService = abp.services.app.profile;
        var _initialTimezone = null;

        var _modalManager;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var $modal = _modalManager.getModal();

            _$form = $modal.find('form[name=MySettingsModalForm]');
            _$form.validate();

            _initialTimezone = _$form.find("[name='Timezone']").val();

            var $btnEnableGoogleAuthenticator = $modal.find('#btnEnableGoogleAuthenticator');

            $btnEnableGoogleAuthenticator.click(function () {
                _profileService.updateGoogleAuthenticatorKey()
                    .done(function (result) {
                        $modal.find('.google-authenticator-enable').toggle();
                        $modal.find('img').attr('src', result.qrCodeSetupImageUrl);
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            });

            var $SmsVerification = $modal.find('#btnSmsVerification');
            var smsVerificationModal = new app.ModalManager({
                viewUrl: abp.appPath + 'Admin/Profile/SmsVerificationModal',
                scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Profile/_SmsVerificationModal.js',
                modalClass: 'SmsVerificationModal'
            });

            $SmsVerification.click(function () {
                _profileService.sendVerificationSms()
                    .done(function () {
                        smsVerificationModal.open({}, function () {
                            $('#SpanSmsVerificationVerified').show();
                            $('#SpanSmsVerificationUnverified').hide();
                        });
                    });
            });

            _$form.find(".tooltips").tooltip();
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var profile = _$form.serializeFormToObject();

            _modalManager.setBusy(true);
            _profileService.updateCurrentUserProfile(profile)
                .done(function () {
                    $('#HeaderCurrentUserName').text(profile.UserName);
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();

                    var newTimezone = _$form.find("[name='Timezone']").val();

                    if (abp.clock.provider.supportsMultipleTimezone && _initialTimezone !== newTimezone) {
                        abp.message.info(app.localize('TimeZoneSettingChangedRefreshPageNotification')).done(function () {
                            window.location.reload();
                        });
                    }

                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})();