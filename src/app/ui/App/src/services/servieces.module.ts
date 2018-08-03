import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';

import { RequestService, DeviceService, DialogService, LoadingService, SessionService, AuthInterceptor, NativeService, StorageService, PayService ,ServiceProxy} from '@services/services';

@NgModule({
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        ServiceProxy,
        RequestService,
        DeviceService,
        DialogService,
        LoadingService,
        SessionService,
        AuthInterceptor,
        NativeService,
        StorageService,
        PayService
    ]
})
export class ServicesModule { }
