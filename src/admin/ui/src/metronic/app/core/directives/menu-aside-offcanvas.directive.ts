import { Directive, AfterViewInit, ElementRef } from '@angular/core';

@Directive({
    selector: '[mMenuAsideOffcanvas]'
})
export class MenuAsideOffcanvasDirective implements AfterViewInit {
    menuOffcanvas: any;

    constructor(private el: ElementRef) { }

    ngAfterViewInit(): void {
        const offcanvasClass = mUtil.hasClass(this.el.nativeElement, 'm-aside-left--offcanvas-default') ? 'm-aside-left--offcanvas-default' : 'm-aside-left';

        this.menuOffcanvas = new mOffcanvas(this.el.nativeElement, {
            baseClass: offcanvasClass,
            overlay: true,
            closeBy: 'm_aside_left_close_btn',
            toggleBy: {
                target: 'm_aside_left_offcanvas_toggle',
                state: 'm-brand__toggler--active'
            }
        });
    }
}
