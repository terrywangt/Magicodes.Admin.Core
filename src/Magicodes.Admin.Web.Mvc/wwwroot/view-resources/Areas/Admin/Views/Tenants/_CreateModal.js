(function ($) {
    //Custom validation type for tenancy name
    $.validator.addMethod("tenancyNameRegex", function (value, element, regexpr) {
        return regexpr.test(value);
    }, app.localize('TenantName_Regex_Description'));

    app.modals.CreateTenantModal = function () {
        var _tenantService = abp.services.app.tenant;
        var _$tenantInformationForm = null;
        var _passwordComplexityHelper = new app.PasswordComplexityHelper();

        var _modalManager;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var modal = _modalManager.getModal();

            _$tenantInformationForm = modal.find('form[name=TenantInformationsForm]');
            _$tenantInformationForm.validate({
                rules: {
                    TenancyName: {
                        tenancyNameRegex: new RegExp(_$tenantInformationForm.find('input[name=TenancyName]').attr('regex'))
                    }
                }
            });

            //Show/Hide password inputs when "random password" checkbox is changed.

            var passwordInputs = modal.find('input[name=AdminPassword],input[name=AdminPasswordRepeat]');
            var passwordInputGroups = passwordInputs.closest('.form-group');

            _passwordComplexityHelper.setPasswordComplexityRules(passwordInputs, window.passwordComplexitySetting);

            $('#CreateTenant_SetRandomPassword').change(function () {
                if ($(this).is(':checked')) {
                    passwordInputGroups.slideUp('fast');
                    passwordInputs.removeAttr('required');
                } else {
                    passwordInputGroups.slideDown('fast');
                    passwordInputs.attr('required', 'required');
                }
            });

            //Show/Hide connection string input when "use host db" checkbox is changed.

            var connStringInput = modal.find('input[name=ConnectionString]');
            var connStringInputGroup = connStringInput.closest('.form-group');

            $('#CreateTenant_UseHostDb').change(function () {
                if ($(this).is(':checked')) {
                    connStringInputGroup.slideUp('fast');
                    connStringInput.removeAttr('required');
                } else {
                    connStringInputGroup.slideDown('fast');
                    connStringInput.attr('required', 'required');
                }
            });

            modal.find('.date-time-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            var $subscriptionEndDateDiv = modal.find('input[name=SubscriptionEndDateUtc]').parent('div');
            var isUnlimitedInput = modal.find('#CreateTenant_IsUnlimited');
            var subscriptionEndDateUtcInput = modal.find('input[name=SubscriptionEndDateUtc]');
            function toggleSubscriptionEndDateDiv() {
                if (isUnlimitedInput.is(':checked')) {
                    $subscriptionEndDateDiv.slideUp('fast');
                    subscriptionEndDateUtcInput.removeAttr('required');
                } else {
                    $subscriptionEndDateDiv.slideDown('fast');
                    subscriptionEndDateUtcInput.attr('required', 'required');
                }
            }

            isUnlimitedInput.change(function () {
                toggleSubscriptionEndDateDiv();
            });

            toggleSubscriptionEndDateDiv();

            var $editionCombobox = modal.find('#EditionId');
            var $isInTrialCheckbox = modal.find('#CreateTenant_IsInTrialPeriod');
            $editionCombobox.change(function () {

                var isFree = $('option:selected', this).attr('data-isfree') === "True";
                var selectedValue = $('option:selected', this).val();
                if (isFree) {
                    $isInTrialCheckbox.closest('div').slideUp('fast');
                } else {
                    $isInTrialCheckbox.closest('div').slideDown('fast');
                }

                if (selectedValue <= 0) {
                    modal.find('.subscription-component').slideUp('fast');
                } else {
                    modal.find('.subscription-component').slideDown('fast');
                }
            });

            $editionCombobox.trigger('change');
        };

        this.save = function () {
            if (!_$tenantInformationForm.valid()) {
                return;
            }

            var tenant = _$tenantInformationForm.serializeFormToObject();

            //take selected date as UTC
            if ($('#CreateTenant_IsUnlimited').is(':visible') && !$('#CreateTenant_IsUnlimited').is(':checked')) {
                tenant.SubscriptionEndDateUtc = moment($('.date-time-picker').datadatetimepicker('getDate')).format("YYYY-MM-DDTHH:mm:ss") + 'Z';
            } else {
                tenant.SubscriptionEndDateUtc = null;
            }

            if (tenant.SetRandomPassword) {
                tenant.Password = null;
            }

            if (tenant.UseHostDb) {
                tenant.ConnectionString = null;
            }

            _modalManager.setBusy(true);
            _tenantService.createTenant(
                tenant
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createTenantModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);