'use strict';

(function () {
    //定义Tab组件
    Vue.component('mc-tab', {
        props: ['tabs'],
        template: '<div class="tabs-container tabbable-custom" style="margin-bottom:0px">' + '<ul class="nav nav-tabs">' + '<li v-bind:class="item.active?\'active\':\'\'" v-for="(item, index) in tabs">' + '<a data-toggle="tab" v-bind:href="\'#\'+item.id" v-bind:id="\'a-\'+item.id" >' + '<i class="" v-bind:class="item.icon"></i><span>{{item.title}}</span><span v-if="item.allowClose"><i class="fa fa-times tabIcon" v-on:click="remove(tabs,item,index)"></i></span>' + '</a>' + '</li>' + '</ul>' + '<div class="tab-content" style="padding-bottom:0px;">' + '<div class="tab-pane" v-bind:class="item.active?\'active\':\'\'" v-for="item in tabs" v-bind:id="item.id">' + '<iframe v-bind:src="item.url" v-bind:id="\'i-\'+item.id" v-bind:name="\'i-\'+item.id" v-bind:style="{height:item.height+\'px\'}" class="iframetab h-30"></iframe>' + '</div>' + '</div>' + '</div>',
        methods: {
            remove: function remove(tabs, item, index) {
                if (tabs.length > 1) {
                    if (tabs[index].active) tabs[0].active = true;
                    tabs.splice(index, 1);
                }
            }
        }
    });
    $(function () {
        var height = document.body.offsetHeight - 180;
        var tabData = [{ title: '  仪表盘', url: '/Admin/Welcome', id: '仪表盘', height: height, icon: 'icon-speedometer', active: true, allowClose: false }];

        var tabVM = new Vue({
            el: '#tab-container',
            data: {
                tabs: tabData
            },
            methods: {}
        });
        //清除聚焦的菜单
        function clearActive() {
            $.each(tabData, function (i, v) {
                if (v.active) {
                    v.active = false;
                    return;
                };
            });
        }
        //设置导航菜单点击事件
        $('#siteMenu a[data-href]').click(function () {
            var $a = $(this);
            var title = $a.text();
            var icon = '';
            if (typeof $a.find('i').attr('class') === "string") {
                icon = $a.find('i').attr('class').replace('sub-menu-icon ', '');
            }
            var url = $a.data('href');
            var id = $a.data('id');
            var isExist = false;

            clearActive();

            $.each(tabData, function (i, v) {
                if (v.id == id) {
                    v.active = true;
                    $('#a-' + id).tab('show');
                    //Vue.set(tabData, i, v);
                    isExist = true;
                    if ($('#i-' + id).attr('src') != url) {
                        $('#i-' + id).attr('src', url);
                    }
                }
            });
            if (!isExist) {
                Vue.set(tabData, tabData.length, { title: title, url: url, id: id, height: height, icon: icon, active: true, allowClose: true });
            }

            //高亮显示点击的菜单项
            var $currentLi = $a.closest("li");
            $('#siteMenu li.active').removeClass('active');
            var $parentLi = $currentLi.closest("li");
            if ($parentLi) {
                $parentLi.addClass("active");
                var $root = $parentLi.closest("li");
                if ($root) {
                    $root.addClass("active");
                }
            }
            $currentLi.addClass("active");
        });
    });
})();

