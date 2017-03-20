var CurrentPage = function () {

    var handleLogin = function () {

        var $loginForm = $('.login-form');
        //设置验证规则
        $loginForm.validate({
            rules: {
                username: {
                    required: true
                },
                password: {
                    required: true
                }
            }
        });
        //设置回车提交
        $loginForm.find('input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.login-form').valid()) {
                    $('.login-form').submit();
                }
                return false;
            }
        });
        //提交表单
        $loginForm.submit(function (e) {
            e.preventDefault();

            if (!$('.login-form').valid()) {
                return;
            }

            abp.ui.setBusy(
                null,
                abp.ajax({
                    contentType: app.consts.contentTypes.formUrlencoded,
                    url: $loginForm.attr('action'),
                    data: $loginForm.serialize()
                })
            );
        });
        //触发第三方登录程序
        $('a.social-login-icon').click(function() {
            var $a = $(this);
            var $form = $a.closest('form');
            $form.find('input[name=provider]').val($a.attr('data-provider'));
            $form.submit();
        });

        $loginForm.find('input[name=returnUrlHash]').val(location.hash);
        //聚焦到用户名输入框
        $('input[type=text]').first().focus();
        //设置每日一图
        $('body').css('background-image', 'url("http://www.dujin.org/sys/bing/1920.php")');
    }

    return {
        init: function () {
            handleLogin();
        }
    };

}();