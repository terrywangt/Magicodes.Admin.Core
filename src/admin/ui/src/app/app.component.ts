import { AfterViewInit, Component, Injector, OnInit, ViewContainerRef } from '@angular/core';
import { Router } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { SubscriptionStartType } from '@shared/AppEnums';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { ChatSignalrService } from 'app/shared/layout/chat/chat-signalr.service';
import * as moment from 'moment';
import { AppComponentBase } from 'shared/common/app-component-base';
import { SignalRHelper } from 'shared/helpers/SignalRHelper';

@Component({
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less']
})
export class AppComponent extends AppComponentBase implements OnInit, AfterViewInit {

    private viewContainerRef: ViewContainerRef;
    private router: Router;

    subscriptionStartType = SubscriptionStartType;
    installationMode = true;


    public constructor(
        injector: Injector,
        viewContainerRef: ViewContainerRef,
        private _router: Router,
        private _chatSignalrService: ChatSignalrService) {
        super(injector);
        this.viewContainerRef = viewContainerRef; // You need this small hack in order to catch application root view container ref (required by ng2 bootstrap modal)
        this.router = _router;
    }

    ngOnInit(): void {
        if (this.appSession.application) {
            SignalRHelper.initSignalR(() => { this._chatSignalrService.init(); });
        }

        this.installationMode = UrlHelper.isInstallUrl(location.href);
    }

    subscriptionStatusBarVisible(): boolean {
        return this.appSession.tenantId > 0 &&
            (this.appSession.tenant.isInTrialPeriod ||
                this.subscriptionIsExpiringSoon());
    }

    subscriptionIsExpiringSoon(): boolean {
        if (this.appSession.tenant.subscriptionEndDateUtc) {
            return moment().utc().add(AppConsts.subscriptionExpireNootifyDayCount, 'days') >= moment(this.appSession.tenant.subscriptionEndDateUtc);
        }

        return false;
    }

    ngAfterViewInit(): void {
        abp.signalr.autoConnect = false;
    }
}
