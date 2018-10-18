import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { Injector, ElementRef, Component, OnInit, AfterViewInit, ViewEncapsulation, Inject, HostBinding, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { AppMenu } from './app-menu';
import { AppNavigationService } from './app-navigation.service';
import { DOCUMENT } from '@angular/common';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';

import { LayoutRefService } from '@metronic/app/core/services/layout/layout-ref.service';
import { MenuAsideOffcanvasDirective } from '@metronic/app/core/directives/menu-aside-offcanvas.directive';

@Component({
    templateUrl: './side-bar-menu.component.html',
    selector: 'side-bar-menu',
    encapsulation: ViewEncapsulation.None,
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class SideBarMenuComponent extends AppComponentBase implements OnInit, AfterViewInit {

    menu: AppMenu = null;

    @HostBinding('class') classes = 'm-grid__item m-aside-left m-aside-left--skin-' + this.s('App.UiManagement.Left.AsideSkin');
    @HostBinding('id') id = 'm_aside_left';

    @HostBinding('attr.mMenuAsideOffcanvas') mMenuAsideOffcanvas: MenuAsideOffcanvasDirective;

    currentRouteUrl = '';
    insideTm: any;
    outsideTm: any;

    constructor(
        injector: Injector,
        private el: ElementRef,
        private router: Router,
        private layoutRefService: LayoutRefService,
        public permission: PermissionCheckerService,
        private _uiCustomizationService: AppUiCustomizationService,
        private _appNavigationService: AppNavigationService,
        @Inject(DOCUMENT) private document: Document) {
        super(injector);
    }

    ngOnInit() {
        this.menu = this._appNavigationService.getMenu();

        this.currentRouteUrl = this.router.url.split(/[?#]/)[0];

        this.router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(event => this.currentRouteUrl = this.router.url.split(/[?#]/)[0]);
    }

    ngAfterViewInit(): void {
        setTimeout(() => {
            this.mMenuAsideOffcanvas = new MenuAsideOffcanvasDirective(this.el);
            this.mMenuAsideOffcanvas.ngAfterViewInit();

            this.layoutRefService.addElement('asideLeft', this.el.nativeElement);
        });
    }

    showMenuItem(menuItem): boolean {
        return this._appNavigationService.showMenuItem(menuItem);
    }

    isMenuItemIsActive(item): boolean {
        if (item.items.length) {
            return this.isMenuRootItemIsActive(item);
        }

        if (!item.route) {
            return false;
        }

        // dashboard
        if (item.route !== '/' && this.currentRouteUrl.startsWith(item.route)) {
            return true;
        }

        return this.currentRouteUrl === item.route;
    }

    isMenuRootItemIsActive(item): boolean {
        let result = false;

        for (const subItem of item.items) {
            result = this.isMenuItemIsActive(subItem);
            if (result) {
                return true;
            }
        }

        return false;
    }

    /**
	 * Use for fixed left aside menu, to show menu on mouseenter event.
	 * @param e Event
	 */
    mouseEnter(e: Event) {
        // check if the left aside menu is fixed
        if (this.document.body.classList.contains('m-aside-left--fixed')) {
            if (this.outsideTm) {
                clearTimeout(this.outsideTm);
                this.outsideTm = null;
            }

            this.insideTm = setTimeout(() => {
                // if the left aside menu is minimized
                if (this.document.body.classList.contains('m-aside-left--minimize') && mUtil.isInResponsiveRange('desktop')) {
                    // show the left aside menu
                    this.document.body.classList.remove('m-aside-left--minimize');
                    this.document.body.classList.add('m-aside-left--minimize-hover');
                }
            }, 300);
        }
    }

    /**
     * Use for fixed left aside menu, to show menu on mouseenter event.
     * @param e Event
     */
    mouseLeave(e: Event) {
        if (this.document.body.classList.contains('m-aside-left--fixed')) {
            if (this.insideTm) {
                clearTimeout(this.insideTm);
                this.insideTm = null;
            }

            this.outsideTm = setTimeout(() => {
                // if the left aside menu is expand
                if (this.document.body.classList.contains('m-aside-left--minimize-hover') && mUtil.isInResponsiveRange('desktop')) {
                    // hide back the left aside menu
                    this.document.body.classList.remove('m-aside-left--minimize-hover');
                    this.document.body.classList.add('m-aside-left--minimize');
                }
            }, 500);
        }
    }
}
