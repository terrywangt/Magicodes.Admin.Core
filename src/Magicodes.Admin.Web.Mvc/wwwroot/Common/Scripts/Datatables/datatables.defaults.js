/************************************************************************
* Overrides default settings for datatables                             *
*************************************************************************/
(function ($) {
    if (!$.fn.dataTable) {
        return;
    }

    var translationsUrl = '/Common/Scripts/Datatables/Translations/' +
        abp.localization.currentCulture.displayNameEnglish +
        '.json';

    $.ajax(translationsUrl)
        .fail(function () {
            translationsUrl = '/Common/Scripts/Datatables/Translations/English.json';
            console.log('Language is set to English for datatables, because ' + abp.localization.currentCulture.displayNameEnglish + ' is not found!');
        }).always(function () {
            $.extend(true, $.fn.dataTable.defaults, {
                language: {
                    url: translationsUrl
                },
                lengthMenu: [5, 10, 25, 50, 100, 250, 500],
                pageLength: 10,
                responsive: true,
                searching: false,
                pagingType: "bootstrap_full_number",
                dom: 'rt<"bottom"ilp><"clear">',
                order: []
            });
        });

})(jQuery);