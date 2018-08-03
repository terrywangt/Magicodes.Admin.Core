import { Component, Injector } from '@angular/core';
import { IonicPage } from 'ionic-angular';
import { AppbaseComponent } from '@app/appbase';

@IonicPage()

@Component({
  selector: 'page-user',
  templateUrl: 'user.html'
})
export class UserPage extends AppbaseComponent {

  constructor(injector: Injector) {
    super(injector)
  }
  ionViewDidLoad(){
   
  }
}
