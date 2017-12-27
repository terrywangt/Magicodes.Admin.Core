$(function () {
    var demoUiComponentsService = abp.services.app.demoUiComponents;

    // 
    // date picker 
    //
    $('.date-picker').datetimepicker({
        locale: abp.localization.currentLanguage.name,
        format: 'L'
    });

    $('.test-btn-date-picker').click(function () {
        var $dateInput = $(this).closest('.input-group').find('input');

        demoUiComponentsService.sendAndGetDate(
            $dateInput.data("DateTimePicker").date().format("YYYY-MM-DDTHH:mm:ssZ")
        ).done(function (result) {
            abp.message.info(result.dateString, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });

    // 
    // datetime picker 
    //
    $('.datetime-picker').datetimepicker({
        locale: abp.localization.currentLanguage.name,
        format: 'L LT'
    });

    $('.test-btn-datetime-picker').click(function () {
        var $datetimeInput = $(this).closest('.input-group').find('input');

        demoUiComponentsService.sendAndGetDateTime(
            $datetimeInput.data("DateTimePicker").date().format("YYYY-MM-DDTHH:mm:ssZ")
        ).done(function (result) {
            abp.message.info(result.dateString, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });

    // 
    // daterange picker 
    //
    var selectedDateRange = {
        startDate: moment().add(-7, 'days').startOf('day'),
        endDate: moment().endOf('day')
    };

    $('.daterange-picker').daterangepicker(
        $.extend(true,
            app.createDateRangePickerOptions({
                allowFutureDate: true
            }),
            selectedDateRange),
        function (start, end, label) {
            selectedDateRange.startDate = start;
            selectedDateRange.endDate = end;
        });

    $('.test-btn-daterange-picker').click(function () {
        demoUiComponentsService.sendAndGetDateRange(
            selectedDateRange.startDate,
            selectedDateRange.endDate
        ).done(function (result) {
            abp.message.info(result.dateString, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });
});
