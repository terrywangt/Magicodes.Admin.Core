import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class AppAuthService {

    logout(reload?: boolean, returnUrl?: string): void {
        abp.ajax({
            url: AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/LogOut',
            method: 'GET',
            headers: {
                'Abp.TenantId': abp.multiTenancy.getTenantIdCookie(),
                'Authorization': 'Bearer ' + abp.auth.getToken()
    }
        }).done(result => {
            abp.auth.clearToken();
            if (reload !== false) {
                if (returnUrl) {
                    location.href = returnUrl;
                } else {
                    location.href = "";
                }
            }
        });
    }
}
