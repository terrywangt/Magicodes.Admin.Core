import { Component, Injector } from '@angular/core';
import { IonicPage } from 'ionic-angular';
import { AppbaseComponent } from '@app/appbase';

@IonicPage()
@Component({
  selector: 'page-login',
  templateUrl: 'login.html',
})
export class LoginPage extends AppbaseComponent {

  constructor(
    injector: Injector
  ) {
    super(injector)
  }

  ionViewDidLoad() {
    this.storage.resetToken()
  }

  /**
   * 登录
   */
  login() {
    this.storage.setToken("");
    this.close();
  }
}
