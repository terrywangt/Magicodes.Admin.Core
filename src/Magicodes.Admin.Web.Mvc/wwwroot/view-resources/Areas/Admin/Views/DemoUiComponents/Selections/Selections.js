$(function () {
    var demoUiComponentsService = abp.services.app.demoUiComponents;

    // 
    // select2 initialize
    //
    $(".m-select2").select2({
        placeholder: 'Select',
        ajax: {
            url: abp.appPath + "api/services/app/DemoUiComponents/GetCountries",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term, // search term
                    page: params.page
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;

                return {
                    results: $.map(data.result, function (item) {
                        return {
                            text: item.name,
                            id: item.value
                        }
                    }),
                    pagination: {
                        more: (params.page * 30) < data.result.length
                    }
                };
            },
            cache: true
        },
        minimumInputLength: 1,
        language: abp.localization.currentCulture.name
    });

    // 
    // select2 initialize for tagging support
    //
    $(".select2-tagging").select2({
        ajax: {
            url: abp.appPath + "api/services/app/DemoUiComponents/GetCountries",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term, // search term
                    page: params.page
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;

                return {
                    results: $.map(data.result, function (item) {
                        return {
                            text: item.name,
                            id: item.value
                        }
                    }),
                    pagination: {
                        more: (params.page * 30) < data.result.length
                    }
                };
            },
            cache: true
        },
        minimumInputLength: 1,
        language: abp.localization.currentCulture.name,
        tags: true,
        tokenSeparators: [',', ' ']
    });

    // 
    // single select - post
    //
    $('.test-btn-select2-single').click(function () {
        var $selectElement = $('#select2SingleSelect');

        var input = [];
        input.push({
            name: $selectElement.text(),
            value: $selectElement.val()
        });

        demoUiComponentsService.sendAndGetSelectedCountries(
            input
        ).done(function (data) {
            var message = "";

            $.each(data, function (index, item) {
                message += '<div><strong>id</strong>: ' + item.value + ' - <strong>name</strong>: ' + item.name + '</div>';
            });

            abp.libs.sweetAlert.config.info.html = true;
            abp.message.info(message, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });

    // 
    // multi select - post
    //
    $('.test-btn-select2-multiple').click(function () {
        var $selectElement = $('#select2MultiSelect');

        var input = [];

        $.each($selectElement.select2('data'), function (index, item) {
            input.push({
                name: item.text,
                value: item.id
            });
        });

        demoUiComponentsService.sendAndGetSelectedCountries(
            input
        ).done(function (data) {
            var message = "";

            $.each(data, function (index, item) {
                message += '<div><strong>id</strong>: ' + item.value + ' - <strong>name</strong>: ' + item.name + '</div>';
            });

            abp.libs.sweetAlert.config.info.html = true;
            abp.message.info(message, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });

    // 
    // tag select - post
    //
    $('.test-btn-select2-tagging').click(function () {
        var $selectElement = $('#select2Tagging');

        var input = [];

        $.each($selectElement.select2('data'), function (index, item) {
            input.push({
                name: item.text,
                value: item.id
            });
        });

        demoUiComponentsService.sendAndGetSelectedCountries(
            input
        ).done(function (data) {
            var message = "";

            $.each(data, function (index, item) {
                message += '<div><strong>id</strong>: ' + item.value + ' - <strong>name</strong>: ' + item.name + '</div>';
            });

            abp.libs.sweetAlert.config.info.html = true;
            abp.message.info(message, app.localize('PostedValue'));
            abp.notify.info(app.localize('SavedSuccessfully'));
        });
    });
});