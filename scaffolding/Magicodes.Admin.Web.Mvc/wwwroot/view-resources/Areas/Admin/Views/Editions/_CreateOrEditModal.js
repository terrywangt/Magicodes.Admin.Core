(function () {

    app.modals.CreateOrEditEditionModal = function () {

        var _modalManager;
        var editionService = abp.services.app.edition;
        var $editionInformationForm = null;
        var featuresTree;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var $modal = _modalManager.getModal();

            featuresTree = new FeaturesTree();
            featuresTree.init($modal.find('.feature-tree'));

            var $editionItemsDiv = $modal.find('.edition-list');
            var $priceDivs = $modal.find('.SubscriptionPrice');
            var $trialDayCountDiv = $modal.find('.trial-day-count');
            var $waitingDayAfterExpireDiv = $modal.find('.waiting-day-after-expire');
            var $paidFeatures = $modal.find('.paid-features');
            var $monthlyPrice = $modal.find('.paid-features > .SubscriptionPrice input[name="MonthlyPrice"]');
            var $annualPrice = $modal.find('.paid-features > .SubscriptionPrice input[name="AnnualPrice"]');

            function toggleEditionItems() {
                if (!$modal.find('#EditEdition_ExpireAction_AssignEdition').is(':checked')) {
                    $editionItemsDiv.slideUp('fast');
                } else {
                    $editionItemsDiv.slideDown('fast');
                }
            }

            function togglePriceDivs() {
                if (!$modal.find('#EditEdition_IsPaid').is(':checked')) {
                    $priceDivs.slideUp('fast');
                    $paidFeatures.slideUp('fast');

                    $priceDivs.find('input').attr('required', false);
                } else {
                    $priceDivs.slideDown('fast');
                    $paidFeatures.slideDown('fast');

                    $priceDivs.find('input').attr('required', true);
                }
            }

            function toggleTrialDayCount() {
                if (!$modal.find('#EditEdition_IsTrialActive').is(':checked')) {
                    $trialDayCountDiv.slideUp('fast');
                } else {
                    $trialDayCountDiv.slideDown('fast');
                }
            }

            function toggleWaitingDayAfterExpire() {
                if (!$modal.find('#EditEdition_IsWaitingDayActive').is(':checked')) {
                    $waitingDayAfterExpireDiv.slideUp('fast');
                } else {
                    $waitingDayAfterExpireDiv.slideDown('fast');
                }
            }

            function createCurrencyInputs() {
                var opts = {
                    radixPoint: ".",
                    groupSeparator: ",",
                    digits: 2,
                    autoGroup: true,
                    prefix: '$ ',
                    rightAlign: false,
                    showMaskOnHover: false,
                    showMaskOnFocus: false,
                    placeholder: "_",
                    removeMaskOnSubmit: true,
                    autoUnmask: true
                };

                $monthlyPrice.inputmask("numeric", $.extend({}, opts));
                $annualPrice.inputmask("numeric", $.extend({}, opts));
            }

            $modal.find('input[name=ExpireAction]').change(function () {
                toggleEditionItems();
            });

            $modal.find('input[name=SubscriptionPrice]').change(function () {
                togglePriceDivs();
            });

            $modal.find('#EditEdition_IsTrialActive').change(function () {
                toggleTrialDayCount();
            });

            $modal.find('#EditEdition_IsWaitingDayActive').change(function () {
                toggleWaitingDayAfterExpire();
            });

            toggleEditionItems();
            togglePriceDivs();
            toggleTrialDayCount();
            toggleWaitingDayAfterExpire();
            createCurrencyInputs();

            $editionInformationForm = _modalManager.getModal().find('form[name=EditionInformationsForm]');
            $editionInformationForm.validate();
        };

        this.save = function () {
            if (!$editionInformationForm.valid()) {
                return;
            }

            if (!featuresTree.isValid()) {
                abp.message.warn(app.localize('InvalidFeaturesWarning'));
                return;
            }

            var edition = $editionInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            editionService.createOrUpdateEdition({
                edition: edition,
                featureValues: featuresTree.getFeatureValues()
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditEditionModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();