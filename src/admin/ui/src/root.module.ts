import { AbpModule } from '@abp/abp.module';
import { AbpHttpInterceptor } from '@abp/abpHttpInterceptor';
import { PlatformLocation, registerLocaleData } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, Injector, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { AppConsts } from '@shared/AppConsts';
import { CommonModule } from '@shared/common/common.module';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import * as localForage from 'localforage';
import * as _ from 'lodash';
import { AppPreBootstrap } from './AppPreBootstrap';
import { AppModule } from './app/app.module';
import { RootRoutingModule } from './root-routing.module';
import { RootComponent } from './root.component';
import { DomHelper } from '@shared/helpers/DomHelper';

export function appInitializerFactory(
    injector: Injector,
    platformLocation: PlatformLocation) {
    return () => {
        abp.ui.setBusy();

        handleLogoutRequest(injector.get(AppAuthService));

        return new Promise<boolean>((resolve, reject) => {
            AppConsts.appBaseHref = getBaseHref(platformLocation);
            let appBaseUrl = getDocumentOrigin() + AppConsts.appBaseHref;

            AppPreBootstrap.run(appBaseUrl, () => {
                // Initialize local Forage
                localForage.config({
                    driver: localForage.LOCALSTORAGE,
                    name: 'Admin',
                    version: 1.0,
                    storeName: 'abpzerotemplate_local_storage',
                    description: 'Cached data for Admin'
                });

                let appSessionService: AppSessionService = injector.get(AppSessionService);
                let ui: AppUiCustomizationService = injector.get(AppUiCustomizationService);
                appSessionService.init().then(
                    (result) => {

                        //Css classes based on the layout
                        let appUiCustomizationService: AppUiCustomizationService = injector.get(AppUiCustomizationService);
                        if (abp.session.userId) {
                            document.body.className = appUiCustomizationService.getAppModuleBodyClass();
                        } else {
                            document.body.className = appUiCustomizationService.getAccountModuleBodyClass();
                        }

                        //tenant specific custom css
                        if (appSessionService.tenant && appSessionService.tenant.customCssId) {
                            document.head.appendChild(
                                DomHelper.createElement('link', [
                                    {
                                        key: 'id',
                                        value: 'TenantCustomCss'
                                    },
                                    {
                                        key: 'rel',
                                        value: 'stylesheet'
                                    },
                                    {
                                        key: 'href',
                                        value: AppConsts.remoteServiceBaseUrl + '/TenantCustomization/GetCustomCss?id=' + appSessionService.tenant.customCssId
                                    }])
                            );
                        }

                        let metaImage = DomHelper.getElementByAttributeValue('meta', 'property', 'og:image');
                        if (metaImage) {
                            //set og share image meta tag
                            if (!appSessionService.tenant || !appSessionService.tenant.logoId) {
                                metaImage.setAttribute('content', window.location.origin + '/assets/common/images/app-logo-on-' + ui.getAsideSkin() + '.png');
                            } else {
                                metaImage.setAttribute('content', AppConsts.remoteServiceBaseUrl + '/TenantCustomization/GetLogo?id=' + appSessionService.tenant.logoId);
                            }
                        }

                        abp.ui.clearBusy();

                        if (shouldLoadLocale()) {
                            let angularLocale = convertAbpLocaleToAngularLocale(abp.localization.currentLanguage.name);
                            import(`@angular/common/locales/${angularLocale}.js`)
                                .then(module => {
                                    registerLocaleData(module.default);
                                    resolve(result);
                                }, reject);
                        } else {
                            resolve(result);
                        }
                    },
                    (err) => {
                        abp.ui.clearBusy();
                        reject(err);
                    }
                );
            }, resolve, reject);
        });
    };
}

function getDocumentOrigin() {
    if (!document.location.origin) {
        return document.location.protocol + '//' + document.location.hostname + (document.location.port ? ':' + document.location.port : '');
    }

    return document.location.origin;
}

export function shouldLoadLocale(): boolean {
    return abp.localization.currentLanguage.name && abp.localization.currentLanguage.name !== 'en-US';
}

export function convertAbpLocaleToAngularLocale(locale: string): string {
    if (!AppConsts.localeMappings) {
        return locale;
    }

    let localeMapings = _.filter(AppConsts.localeMappings, { from: locale });
    if (localeMapings && localeMapings.length) {
        return localeMapings[0]['to'];
    }

    return locale;
}

export function getRemoteServiceBaseUrl(): string {
    return AppConsts.remoteServiceBaseUrl;
}

export function getCurrentLanguage(): string {
    return abp.localization.currentLanguage.name;
}

export function getBaseHref(platformLocation: PlatformLocation): string {
    let baseUrl = platformLocation.getBaseHrefFromDOM();
    if (baseUrl) {
        return baseUrl;
    }

    return '/';
}

function handleLogoutRequest(authService: AppAuthService) {
    let currentUrl = UrlHelper.initialUrl;
    let returnUrl = UrlHelper.getReturnUrl();
    if (currentUrl.indexOf(('account/logout')) >= 0 && returnUrl) {
        authService.logout(true, returnUrl);
    }
}

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        AppModule,
        CommonModule.forRoot(),
        AbpModule,
        ServiceProxyModule,
        HttpClientModule,
        RootRoutingModule
    ],
    declarations: [
        RootComponent
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true },
        { provide: API_BASE_URL, useFactory: getRemoteServiceBaseUrl },
        {
            provide: APP_INITIALIZER,
            useFactory: appInitializerFactory,
            deps: [Injector, PlatformLocation],
            multi: true
        },
        {
            provide: LOCALE_ID,
            useFactory: getCurrentLanguage
        }
    ],
    bootstrap: [RootComponent]
})
export class RootModule {

}
