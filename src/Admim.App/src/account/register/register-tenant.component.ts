import { Component, Injector, OnInit, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';

import {
    TenantRegistrationServiceProxy,
    RegisterTenantOutput,
    PasswordComplexitySetting,
    ProfileServiceProxy,
    EditionSelectDto,
    PaymentServiceProxy
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { TenantRegistrationHelperService } from './tenant-registration-helper.service';
import { RegisterTenantModel } from './register-tenant.model';
import {
    SubscriptionStartType,
    PaymentPeriodType,
    SubscriptionPaymentGatewayType
} from '@shared/AppEnums';

@Component({
    templateUrl: './register-tenant.component.html',
    animations: [accountModuleAnimation()]
})
export class RegisterTenantComponent extends AppComponentBase implements OnInit, AfterViewInit {

    model: RegisterTenantModel = new RegisterTenantModel();
    passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
    subscriptionStartType = SubscriptionStartType;
    paymentPeriodType = PaymentPeriodType;
    selectedPaymentPeriodType: PaymentPeriodType = PaymentPeriodType.Monthly;
    subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
    paymentId = '';
    recaptchaSiteKey: string = AppConsts.recaptchaSiteKey;

    saving = false;

    constructor(
        injector: Injector,
        private _tenantRegistrationService: TenantRegistrationServiceProxy,
        private _router: Router,
        private _profileService: ProfileServiceProxy,
        private _tenantRegistrationHelper: TenantRegistrationHelperService,
        private _activatedRoute: ActivatedRoute,
        private _paymentService: PaymentServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.model.editionId = this._activatedRoute.snapshot.queryParams['editionId'];
        if (this.model.editionId) {
            this.model.subscriptionStartType = this._activatedRoute.snapshot.queryParams['subscriptionStartType'];
            this.model.gateway = this._activatedRoute.snapshot.queryParams['gateway'];
            this.model.paymentId = this._activatedRoute.snapshot.queryParams['paymentId'];
        }

        //Prevent to create tenant in a tenant context
        if (this.appSession.tenant != null) {
            this._router.navigate(['account/login']);
            return;
        }

        this._profileService.getPasswordComplexitySetting().subscribe(result => {
            this.passwordComplexitySetting = result.setting;
        });
    }

    ngAfterViewInit() {
        if (this.model.editionId) {
            this._tenantRegistrationService.getEdition(this.model.editionId)
                .subscribe((result: EditionSelectDto) => {
                    this.model.edition = result;
                });
        }
    }

    get useCaptcha(): boolean {
        return this.setting.getBoolean('App.TenantManagement.UseCaptchaOnRegistration');
    }

    save(): void {
        if (this.useCaptcha && !this.model.captchaResponse) {
            this.message.warn(this.l('CaptchaCanNotBeEmpty'));
            return;
        }

        this.saving = true;
        this._tenantRegistrationService.registerTenant(this.model)
            .finally(() => { this.saving = false; })
            .subscribe((result: RegisterTenantOutput) => {
                this.notify.success(this.l('SuccessfullyRegistered'));

                this._tenantRegistrationHelper.registrationResult = result;
                this._router.navigate(['account/register-tenant-result']);
            });
    }

    captchaResolved(captchaResponse: string): void {
        this.model.captchaResponse = captchaResponse;
    }

    onPaymentPeriodChangeChange(selectedPaymentPeriodType) {
        this.selectedPaymentPeriodType = selectedPaymentPeriodType;
    }
}
