(function ($) {
    $.validator.setDefaults({
        errorElement: 'div',
        errorClass: 'form-control-feedback',
        focusInvalid: false,
        submitOnKeyPress: true,
        ignore:':hidden',
        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-danger');
        },

        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-danger');
        },

        errorPlacement: function (error, element) {
            if (element.closest('.input-icon').length === 1) {
                error.insertAfter(element.closest('.input-icon'));
            } else {
                error.insertAfter(element);
            }
        },

        success: function (label) {
            label.closest('.form-group').removeClass('has-danger');
            label.remove();
        },

        submitHandler: function (form) {
            $(form).find('.alert-danger').hide();
        }
    });
})(jQuery);