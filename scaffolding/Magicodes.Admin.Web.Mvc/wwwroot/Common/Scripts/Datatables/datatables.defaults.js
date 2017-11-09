/************************************************************************
* Overrides default settings for datatables                             *
*************************************************************************/
(function ($) {
    if (!$.fn.dataTable) {
        return;
    }

    $.extend(true, $.fn.dataTable.defaults, {
        language: {
            url: '/Common/Scripts/Datatables/Translations/' + abp.localization.currentCulture.displayNameEnglish + '.json'
        },
        lengthMenu: [5, 10, 25, 50, 100, 250, 500],
        pageLength: 10,
        responsive: true,
        searching: false,
        pagingType: "bootstrap_full_number",
        dom: 'rt<"bottom"ilp><"clear">',
        order: []
    });

})(jQuery);