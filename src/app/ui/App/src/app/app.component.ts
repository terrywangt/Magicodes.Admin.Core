import { Component } from '@angular/core';
import { Platform, ModalController } from 'ionic-angular';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { StorageService } from '@services/services';

@Component({
  templateUrl: 'app.html'
})
export class MyApp {
  tab1Root = 'HomePage';
  tab3Root = 'UserPage';
  isShow: boolean = false;
  constructor(platform: Platform, statusBar: StatusBar, splashScreen: SplashScreen, private storage: StorageService, private modalCtrl: ModalController) {
    platform.ready().then(() => {
      this.getToken();
      // Okay, so the platform is ready and our plugins are available.
      // Here you can do any higher level native things you might need.
      statusBar.backgroundColorByHexString('#59aff9');
      statusBar.overlaysWebView(false);
      statusBar.show();
      splashScreen.hide();
    });
  }
  /**
   * 显示登陆
   */
  showLogin() {
    let modal = this.modalCtrl.create('LoginPage', null, { enableBackdropDismiss: false });
    modal.present();
    modal.onWillDismiss(() => {
      this.isShow = true;
    })
  }
  /**
   * 获取token
   */
  getToken() {
    this.storage.getToken().then(data => {
      console.log(data);
      this.isShow = data.authenticated;
      if (!data.authenticated) {
        this.showLogin();
      }
    })
  }
}
