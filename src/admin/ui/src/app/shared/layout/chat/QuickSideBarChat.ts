import { Injectable } from '@angular/core';


@Injectable()
export class QuickSideBarChat {

    initOffcanvas(): void {
        let $offCanvas = ($('#m_quick_sidebar') as any).mOffcanvas({
            class: 'm-quick-sidebar',
            //overlay: false,
            close: $('#m_quick_sidebar_close'),
            toggle: $('#m_quick_sidebar_toggle')
        });

        // run once on first time dropdown shown
        ($('#m_quick_sidebar') as any).mOffcanvas().one('afterShow', () => {
            mApp.block($('#m_quick_sidebar'));

            setTimeout(() => {
                mApp.unblock($('#m_quick_sidebar'));
                ($('#m_quick_sidebar').find('.m-quick-sidebar__content') as any).removeClass('m--hide');
            }, 1000);
        });
    }

    hide(): void {
        $('body, #m_quick_sidebar').removeClass('m-quick-sidebar--on');
        ($('#m_quick_sidebar') as any).mOffcanvas().hide();
    }

    show(): void {
        $('body, #m_quick_sidebar').addClass('m-quick-sidebar--on');
        ($('#m_quick_sidebar') as any).mOffcanvas().show();
    }

    init(scrollEvent: any): void {
        this.initOffcanvas();
        if (scrollEvent) {
            scrollEvent();
        }
    }
}
