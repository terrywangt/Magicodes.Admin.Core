$(function () {
    var demoUiComponentsService = abp.services.app.demoUiComponents;

    // 
    // input mask initialize
    //
    $(".input-mask").inputmask({
        showMaskOnHover: false
    });

    // 
    // tag select - post
    //
    $('.test-btn-input-mask').click(function () {
        var $inputElement = $(this).closest('.input-group').find('.input-mask');

        demoUiComponentsService.sendAndGetValue(
            $inputElement.val()
        ).done(function (data) {
            abp.message.info(data.output, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });
});