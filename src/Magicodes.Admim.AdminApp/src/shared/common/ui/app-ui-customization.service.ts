import { Injectable } from '@angular/core';

@Injectable()
export class AppUiCustomizationService {

    getTheme() {
      return  this.getSetting("App.UiManagement.Theme");
    }

    getContainerClass() {
        return this.getSetting('App.UiManagement.LayoutType') === 'boxed'
            ? 'm-container--responsive'
            : 'm-container--fluid';
    }

    getAsideSkin() {
        return this.getSetting('App.UiManagement.Left.AsideSkin');
    }

    allowAsideHiding() {
        return this.getSetting('App.UiManagement.Left.AllowAsideHiding') === 'true';
    }

    allowAsideMinimizing() {
        return this.getSetting('App.UiManagement.Left.AllowAsideMinimizing') === 'true';
    }

    leftMenuUsed(): boolean {
        return this.getSetting('App.UiManagement.Left.Position') === 'left';
    }

    topMenuUsed(): boolean {
        return this.getSetting('App.UiManagement.Left.Position') === 'top';
    }

    getAppModuleBodyClass(): string {
        return 'm-page--' + this.getSetting('App.UiManagement.LayoutType') + ' m--skin-' + this.getSetting('App.UiManagement.ContentSkin') + ' ' +
            (this.getSetting('App.UiManagement.ContentSkin') !== '' ? ('m-content--skin-' + this.getSetting('App.UiManagement.ContentSkin')) : '') + ' ' +
            'm-header--' + (this.getSetting('App.UiManagement.Header.DesktopFixedHeader') === 'true' ? 'fixed' : 'static') + ' ' +
            (this.getSetting('App.UiManagement.Header.MobileFixedHeader') === 'true' ? 'm-header--fixed-mobile' : '') + ' ' +
            ((this.getSetting('App.UiManagement.Left.FixedAside') === 'true' && this.getSetting('App.UiManagement.Menu.IsTopMenuUsed') === 'false') ? 'm-aside-left--fixed' : '') + ' ' +
            (this.getSetting('App.UiManagement.Left.DefaultMinimizedAside') === 'true' ? 'm-aside-left--minimize m-brand--minimize' : '') + ' ' +
            (this.getSetting('App.UiManagement.Left.DefaultHiddenAside') === 'true' || this.getSetting('App.UiManagement.Left.Position') === 'top' ? 'm-aside-left--hide' : '') + ' ' +
            'm-aside-left--enabled' + ' ' +
            'm-aside-left--skin-' + this.getSetting('App.UiManagement.Left.AsideSkin') + ' ' +
            'm-aside-left--offcanvas' + ' ' +
            (this.getSetting('App.UiManagement.Footer.FixedFooter') === 'true' && this.getSetting('App.UiManagement.LayoutType') !== 'boxed' ? 'm-footer--fixed' : '');
    }

    getAccountModuleBodyClass() {
        return 'm--skin- m-header--fixed m-header--fixed-mobile m-aside-left--enabled m-aside-left--skin-dark m-aside-left--offcanvas m-footer--push m-aside--offcanvas-default';
    }

    getSelectEditionBodyClass() {
        return 'm--skin-';
    }

    getHeaderClass() {
        return '';
    }

    //User navigation menu
    getSideBarMenuClass(): string {
        let menuCssClass = 'm-aside-menu m-aside-menu--skin-' + this.getSetting('App.UiManagement.Left.AsideSkin');
        menuCssClass += ' m-aside-menu--submenu-skin-';

        if (this.getSetting('App.UiManagement.Left.DropdownSubmenuSkin') === 'inherit') {
            menuCssClass += this.getSetting('App.UiManagement.Left.AsideSkin');
        } else {
            menuCssClass += this.getSetting('App.UiManagement.Left.DropdownSubmenuSkin');
        }

        if (this.getSetting('App.UiManagement.Left.SubmenuToggle') === 'dropdown') {
            menuCssClass += ' m-aside-menu--dropdown';
        }

        return menuCssClass;
    }

    getMenuListClass(): string {
        if (this.getSetting('App.UiManagement.Left.DropdownSubmenuArrow')) {
            return 'm-menu__nav--dropdown-submenu-arrow';
        }

        return '';
    }

    getTopBarMenuClass(): string {
        let menuCssClass = 'm-container m-container--responsive m-container--full-height m-page__container';
        if (this.getSetting('App.UiManagement.LayoutType') === 'boxed') {
            return menuCssClass + ' m-container--xxl';
        }

        return menuCssClass;
    }

    getIsMenuDropdown(): boolean {
        return this.getSetting('App.UiManagement.Left.SubmenuToggle') === 'dropdown';
    }

    getIsMenuScrollable(): boolean {
        return this.getSetting('App.UiManagement.Left.FixedAside') === 'true';
    }

    getIsMenuMinimizable(): boolean {
        return this.getSetting('App.UiManagement.Header.DesktopMinimizeMode') !== '';
    }

    getIsMenuMinimizableMobile(): boolean {
        return this.getSetting('App.UiManagement.Header.MobileFixedHeader') === 'true';
    }

    private getSetting(key: string): string {
        let setting = abp.setting.get(key);
        if (!setting) {
            return null;
        }

        return abp.setting.get(key).toLocaleLowerCase();
    }
}
