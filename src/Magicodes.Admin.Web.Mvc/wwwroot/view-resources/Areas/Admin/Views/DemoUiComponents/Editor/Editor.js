$(function () {
    var demoUiComponentsService = abp.services.app.demoUiComponents;

    // 
    // summernote initialize
    //
    $('#summernote').summernote({
        height: "300px"
    });

    // 
    // tag select - post
    //
    $('.test-btn-summernote').click(function () {
        demoUiComponentsService.sendAndGetValue(
            $("#summernote").summernote('code')
        ).done(function (data) {
            abp.libs.sweetAlert.config.info.html = true;
            abp.message.info(data.output, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });
});