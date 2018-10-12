import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    CommonLookupServiceProxy, CreateTenantInput,
    PasswordComplexitySetting, ProfileServiceProxy,
    TenantServiceProxy, SubscribableEditionComboboxItemDto
} from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'createTenantModal',
    templateUrl: './create-tenant-modal.component.html'
})
export class CreateTenantModalComponent extends AppComponentBase {

    @ViewChild('createModal') modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    setRandomPassword = true;
    useHostDb = true;
    editions: SubscribableEditionComboboxItemDto[] = [];
    tenant: CreateTenantInput;
    passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
    isUnlimited = false;
    isSubscriptionFieldsVisible = false;
    isSelectedEditionFree = false;

    constructor(
        injector: Injector,
        private _tenantService: TenantServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy,
        private _profileService: ProfileServiceProxy
    ) {
        super(injector);
    }

    show() {
        this.active = true;
        this.init();

        this._profileService.getPasswordComplexitySetting().subscribe(result => {
            this.passwordComplexitySetting = result.setting;
            this.modal.show();
        });
    }

    onShown(): void {
        document.getElementById('TenancyName').focus();
    }

    init(): void {
        this.tenant = new CreateTenantInput();
        this.tenant.isActive = true;
        this.tenant.shouldChangePasswordOnNextLogin = true;
        this.tenant.sendActivationEmail = true;
        this.tenant.editionId = 0;
        this.tenant.isInTrialPeriod = false;

        this._commonLookupService.getEditionsForCombobox(false)
            .subscribe((result) => {
                this.editions = result.items;

                let notAssignedItem = new SubscribableEditionComboboxItemDto();
                notAssignedItem.value = '';
                notAssignedItem.displayText = this.l('NotAssigned');

                this.editions.unshift(notAssignedItem);

                this._commonLookupService.getDefaultEditionName().subscribe((getDefaultEditionResult) => {
                    let defaultEdition = _.filter(this.editions, { 'displayText': getDefaultEditionResult.name });
                    if (defaultEdition && defaultEdition[0]) {
                        this.tenant.editionId = parseInt(defaultEdition[0].value);
                        this.toggleSubscriptionFields();
                    }
                });
            });
    }

    getEditionValue(item): number {
        return parseInt(item.value);
    }

    selectedEditionIsFree(): boolean {
        let selectedEditions = _.filter(this.editions, { 'value': this.tenant.editionId })
            .map(u => Object.assign(new SubscribableEditionComboboxItemDto(), u));

        if (selectedEditions.length !== 1) {
            this.isSelectedEditionFree = true;
        }

        let selectedEdition = selectedEditions[0];
        this.isSelectedEditionFree = selectedEdition.isFree;
        return this.isSelectedEditionFree;
    }

    subscriptionEndDateIsValid(): boolean {
        if (this.tenant.editionId <= 0) {
            return true;
        }

        if (this.isUnlimited) {
            return true;
        }

        if (!this.tenant.subscriptionEndDateUtc) {
            return false;
        }

        return this.tenant.subscriptionEndDateUtc !== undefined;
    }

    save(): void {
        this.saving = true;

        if (this.setRandomPassword) {
            this.tenant.adminPassword = null;
        }

        if (this.tenant.editionId === 0) {
            this.tenant.editionId = null;
        }

        if (this.isUnlimited || this.tenant.editionId <= 0) {
            this.tenant.subscriptionEndDateUtc = null;
        }

        this._tenantService.createTenant(this.tenant)
            .pipe(finalize(() => this.saving = false))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    onEditionChange(): void {
        this.tenant.isInTrialPeriod = this.tenant.editionId > 0 && !this.selectedEditionIsFree();
        this.toggleSubscriptionFields();
    }

    toggleSubscriptionFields() {
        if (this.tenant.editionId <= 0 || this.isSelectedEditionFree) {
            this.isSubscriptionFieldsVisible = false;

            if (this.isSelectedEditionFree) {
                this.isUnlimited = true;
            } else {
                this.isUnlimited = false;
            }
        } else {
            this.isSubscriptionFieldsVisible = true;
        }
    }
}
