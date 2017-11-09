$(function () {
    'use strict';
    // Change this to the location of your server-side upload handler:
    var url = abp.appPath + 'Admin/DemoUiComponents/UploadFile';
    $('#fileupload').fileupload({
        url: url,
        dataType: 'json',
        maxFileSize: 999000,
        done: function (e, response) {

            var jsonResult = response.result;

            if (jsonResult.success) {
                var fileUrl = abp.appPath + 'Admin/DemoUiComponents/GetFile?id=' + jsonResult.result.id + '&contentType=' + jsonResult.result.contentType;
                var uploadedFile = '<a href="' + fileUrl + '" target="_blank">' + app.localize('UploadedFile') + '</a><br/><br/>' + ' Free text: ' + jsonResult.result.defaultFileUploadTextInput;

                abp.libs.sweetAlert.config.info.html = true;
                abp.message.info(uploadedFile, app.localize('PostedData'));
                abp.notify.info(app.localize('SavedSuccessfully'));
            } else {
                abp.message.error(jsonResult.error.message);
            }
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
});
