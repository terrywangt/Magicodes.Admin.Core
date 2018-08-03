import { NgModule, ErrorHandler } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { BrowserModule } from '@angular/platform-browser';
import { IonicApp, IonicModule, IonicErrorHandler } from 'ionic-angular';
import { MyApp } from './app.component';

import { IonicStorageModule } from '@ionic/storage';
import { ServicesModule } from "@services/servieces.module";
import { NativeModule } from "./native.module";
@NgModule({
  declarations: [
    MyApp
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    IonicModule.forRoot(MyApp, {
      backButtonText: "",
      backButtonColor: "",
      swipeBackEnabled: true,
      mode: "ios",
      tabsHideOnSubPages: "true",
      preloadModules: true
    }),
    IonicStorageModule.forRoot({
      name: 'fenxiao',
      driverOrder: ['indexeddb', 'sqlite', 'websql']
    }),
    NativeModule,
    ServicesModule
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp
  ],
  providers: [
    { provide: ErrorHandler, useClass: IonicErrorHandler }
  ]
})
export class AppModule { }
