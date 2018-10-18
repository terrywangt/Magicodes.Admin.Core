import { Component, Injector, OnInit } from '@angular/core';
import { AppTimezoneScope } from '@shared/AppEnums';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ComboboxItemDto, CommonLookupServiceProxy, DefaultTimezoneScope, HostSettingsEditDto, HostSettingsServiceProxy, SendTestEmailInput } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './host-settings.component.html',
    animations: [appModuleAnimation()]
})
export class HostSettingsComponent extends AppComponentBase implements OnInit {

    loading = false;
    hostSettings: HostSettingsEditDto;
    editions: ComboboxItemDto[] = undefined;
    testEmailAddress: string = undefined;
    showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;
    defaultTimezoneScope: DefaultTimezoneScope = AppTimezoneScope.Application;

    usingDefaultTimeZone = false;
    initialTimeZone: string = undefined;

    constructor(
        injector: Injector,
        private _hostSettingService: HostSettingsServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy
    ) {
        super(injector);
    }

    loadHostSettings(): void {
        const self = this;
        self._hostSettingService.getAllSettings()
            .subscribe(setting => {
                self.hostSettings = setting;
                self.initialTimeZone = setting.general.timezone;
                self.usingDefaultTimeZone = setting.general.timezoneForComparison === self.setting.get('Abp.Timing.TimeZone');
            });
    }

    loadEditions(): void {
        const self = this;
        self._commonLookupService.getEditionsForCombobox(false).subscribe((result) => {
            self.editions = result.items;

            const notAssignedEdition = new ComboboxItemDto();
            notAssignedEdition.value = null;
            notAssignedEdition.displayText = self.l('NotAssigned');

            self.editions.unshift(notAssignedEdition);
        });
    }

    init(): void {
        const self = this;
        self.testEmailAddress = self.appSession.user.emailAddress;
        self.showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;
        self.loadHostSettings();
        self.loadEditions();
    }

    ngOnInit(): void {
        const self = this;
        self.init();
    }

    sendTestEmail(): void {
        const self = this;
        const input = new SendTestEmailInput();
        input.emailAddress = self.testEmailAddress;
        self._hostSettingService.sendTestEmail(input).subscribe(result => {
            self.notify.info(self.l('TestEmailSentSuccessfully'));
        });
    }

    saveAll(): void {
        const self = this;
        self._hostSettingService.updateAllSettings(self.hostSettings).subscribe(result => {
            self.notify.info(self.l('SavedSuccessfully'));

            if (abp.clock.provider.supportsMultipleTimezone && self.usingDefaultTimeZone && self.initialTimeZone !== self.hostSettings.general.timezone) {
                self.message.info(self.l('TimeZoneSettingChangedRefreshPageNotification')).then(() => {
                    window.location.reload();
                });
            }
        });
    }
}
