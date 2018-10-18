import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { Component, Injector, OnInit, AfterViewInit, ViewEncapsulation, ElementRef, ViewChild } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppMenu } from './app-menu';
import { AppNavigationService } from './app-navigation.service';
import * as objectPath from 'object-path';
import { filter } from 'rxjs/operators';
import { MenuHorizontalDirective } from '@metronic/app/core/directives/menu-horizontal.directive';
import { MenuHorizontalOffcanvasDirective } from '@metronic/app/core/directives/menu-horizontal-offcanvas.directive';

@Component({
    templateUrl: './top-bar-menu.component.html',
    selector: 'top-bar-menu',
    encapsulation: ViewEncapsulation.None
})
export class TopBarMenuComponent extends AppComponentBase implements OnInit, AfterViewInit {

    menu: AppMenu = null;
    currentRouteUrl: any = '';

    @ViewChild('m_header_menu') el: ElementRef;

    mMenuHorizontal: MenuHorizontalDirective;
    mMenuHorOffcanvas: MenuHorizontalOffcanvasDirective;
    constructor(
        injector: Injector,
        private router: Router,
        public permission: PermissionCheckerService,
        private _appNavigationService: AppNavigationService) {
        super(injector);
    }

    ngOnInit() {
        this.menu = this._appNavigationService.getMenu();
        this.currentRouteUrl = this.router.url;

        this.router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(event => {
                this.currentRouteUrl = this.router.url;
            });
    }

    ngAfterViewInit(): void {
        this.mMenuHorOffcanvas = new MenuHorizontalOffcanvasDirective(this.el);
        this.mMenuHorOffcanvas.ngAfterViewInit();

        this.mMenuHorizontal = new MenuHorizontalDirective(this.el);
        this.mMenuHorizontal.ngAfterViewInit();
    }

    showMenuItem(menuItem): boolean {
        return this._appNavigationService.showMenuItem(menuItem);
    }

    getItemCssClasses(item) {
        let cssClasses = 'm-menu__item';

        if (objectPath.get(item, 'items.length')) {
            cssClasses += ' m-menu__item--submenu';
        }

        if (objectPath.get(item, 'icon-only')) {
            cssClasses += ' m-menu__item--icon-only';
        }

        if (item.items.length && this.isMenuItemIsActive(item)) {
            cssClasses += ' m-menu__item--active';
        }

        return cssClasses + ' m-menu__item--rel';
    }

    isMenuItemIsActive(item): boolean {
        if (item.items.length) {
            return this.isMenuRootItemIsActive(item);
        }

        if (!item.page) {
            return false;
        }

        return item.page === this.currentRouteUrl;
    }

    isMenuRootItemIsActive(item): boolean {
        if (item.items) {
            for (const subItem of item.items) {
                if (this.isMenuItemIsActive(subItem)) {
                    return true;
                }
            }
        }

        return false;
    }

    getItemAttrSubmenuToggle(menuItem) {
        return 'click';
    }
}
