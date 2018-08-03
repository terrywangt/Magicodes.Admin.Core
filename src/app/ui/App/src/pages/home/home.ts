import { Component, Injector } from '@angular/core';
import { IonicPage } from 'ionic-angular';
import { AppbaseComponent } from '@app/appbase';

@IonicPage()

@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})
export class HomePage extends AppbaseComponent {

  constructor(injector: Injector) {
    super(injector)
  }

  ionViewDidLoad() {
  }
}
