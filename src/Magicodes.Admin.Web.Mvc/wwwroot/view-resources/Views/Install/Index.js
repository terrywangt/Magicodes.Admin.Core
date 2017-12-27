(function ($) {
    $(function () {
        var _installService = abp.services.app.install;

        var _$smtpCredentialFormGroups = $('input[name=smtpDomain],input[name=smtpUserName],input[name=smtpPassword]')
            .closest('.form-group');

        function toggleSmtpCredentialFormGroups() {
            if ($('#Settings_SmtpUseDefaultCredentials').is(':checked')) {
                _$smtpCredentialFormGroups.slideUp('fast');
                _$smtpCredentialFormGroups.enabled = false;
            } else {
                _$smtpCredentialFormGroups.slideDown('fast');
                _$smtpCredentialFormGroups.enabled = true;
            }
        }

        toggleSmtpCredentialFormGroups();

        $('#Settings_SmtpUseDefaultCredentials').change(function () {
            toggleSmtpCredentialFormGroups();
        });

        $('#SaveButton').click(function () {
            var form = $('#installForm').serializeFormToObject();

            if (!$('#installForm').valid()) {
                return;
            }

            abp.ui.setBusy();

            _installService.setup({
                connectionString: form.connectionString,
                adminPassword: form.adminPassword,
                webSiteUrl: form.webSiteUrl,
                defaultLanguage: form.defaultLanguage,
                smtpSettings: {
                    defaultFromAddress: form.defaultFromAddress,
                    defaultFromDisplayName: form.defaultFromDisplayName,
                    smtpHost: form.smtpHost,
                    smtpPort: form.smtpPort ? form.smtpPort : 0,
                    SmtpEnableSsl: form.SmtpEnableSsl,
                    SmtpUseDefaultCredentials: form.SmtpUseDefaultCredentials,
                    SmtpDomain: form.SmtpDomain,
                    SmtpUserName: form.SmtpUserName,
                    SmtpPassword: form.SmtpPassword
                },
                billInfo: {
                    legalName: form.legalName,
                    address: form.billAddress
                }
            }).done(function () {
                window.location.href = abp.appPath + 'Install/Restart';
            }).always(function() {
                abp.ui.clearBusy();
            });
        });

        $('#installForm').validate({
            debug: true,
            rules: {
                adminPasswordRepeat: {
                    equalTo: "#adminPassword"
                }
            },
            submitHandler: function () {
                return;
            }
        });
    });
})(jQuery);