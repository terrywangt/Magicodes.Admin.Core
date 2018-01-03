import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { TenantRegistrationServiceProxy, SubscriptionServiceProxy, EditionsSelectOutput, EditionSelectDto, FlatFeatureSelectDto, EditionWithFeaturesDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EditionPaymentType, SubscriptionStartType } from '@shared/AppEnums';
import { AppSessionService } from '@shared/common/session/app-session.service';

import * as _ from 'lodash';

@Component({
    templateUrl: './select-edition.component.html',
    styleUrls: ['./select-edition.component.less', './pricing.min.css'],
    encapsulation: ViewEncapsulation.None,
    animations: [accountModuleAnimation()]
})
export class SelectEditionComponent extends AppComponentBase implements OnInit {

    editionsSelectOutput: EditionsSelectOutput = new EditionsSelectOutput();
    isUserLoggedIn = false;
    isSetted = false;
    editionPaymentType: typeof EditionPaymentType = EditionPaymentType;
    subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;

    constructor(
        injector: Injector,
        private _tenantRegistrationService: TenantRegistrationServiceProxy,
        private _subscriptionService: SubscriptionServiceProxy,
        private _appSessionService: AppSessionService,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.isUserLoggedIn = abp.session.userId > 0;

        this._tenantRegistrationService.getEditionsForSelect()
            .subscribe((result) => {
                this.editionsSelectOutput = result;

                if (!this.editionsSelectOutput.editionsWithFeatures || this.editionsSelectOutput.editionsWithFeatures.length <= 0) {
                    this._router.navigate(['/account/register-tenant']);
                }
            });
    }

    isFree(edition: EditionSelectDto): boolean {
        return !edition.monthlyPrice && !edition.annualPrice;
    }

    isTrueFalseFeature(feature: FlatFeatureSelectDto): boolean {
        return feature.inputType.name === 'CHECKBOX';
    }

    featureEnabledForEdition(feature: FlatFeatureSelectDto, edition: EditionWithFeaturesDto): boolean {
        const featureValues = _.filter(edition.featureValues, { name: feature.name });
        if (!featureValues || featureValues.length <= 0) {
            return false;
        }

        const featureValue = featureValues[0];
        return featureValue.value.toLowerCase() === 'true';
    }

    getFeatureValueForEdition(feature: FlatFeatureSelectDto, edition: EditionWithFeaturesDto): string {
        const featureValues = _.filter(edition.featureValues, { name: feature.name });
        if (!featureValues || featureValues.length <= 0) {
            return '';
        }

        const featureValue = featureValues[0];
        return featureValue.value;
    }

    canUpgrade(edition: EditionSelectDto): boolean {
        if (this._appSessionService.tenant.edition.id === edition.id) {
            return false;
        }

        const currentMonthlyPrice = this._appSessionService.tenant.edition.monthlyPrice
            ? this._appSessionService.tenant.edition.monthlyPrice
            : 0;

        const targetMonthlyPrice = edition && edition.monthlyPrice ? edition.monthlyPrice : 0;

        return this.isUserLoggedIn &&
            this._appSessionService.tenant.edition &&
            currentMonthlyPrice <= targetMonthlyPrice;
    }

    upgrade(upgradeEdition: EditionSelectDto, editionPaymentType: EditionPaymentType): void {
        if (editionPaymentType === EditionPaymentType.Upgrade && this.upgradeIsFree(upgradeEdition)) {
            this._subscriptionService.upgradeTenantToEquivalentEdition(upgradeEdition.id)
                .subscribe(() => {
                    this._router.navigate(['app/admin/subscription-management']);
                });
        } else {
            this._router.navigate(['/account/upgrade'], { queryParams: { upgradeEditionId: upgradeEdition.id, editionPaymentType: editionPaymentType }});
        }
    }

    upgradeIsFree(upgradeEdition: EditionSelectDto): boolean {
        if (!this._appSessionService.tenant || !this._appSessionService.tenant.edition) {
            return false;
        }

        const bothEditionsAreFree = this._appSessionService.tenant.edition.isFree && upgradeEdition.isFree;

        const bothEditionsHasSamePrice = !upgradeEdition.isFree &&
            upgradeEdition.monthlyPrice === this._appSessionService.tenant.edition.monthlyPrice &&
            upgradeEdition.annualPrice === this._appSessionService.tenant.edition.annualPrice;

        return bothEditionsAreFree || bothEditionsHasSamePrice;
    }
}
