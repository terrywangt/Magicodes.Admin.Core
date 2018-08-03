import { Injector } from '@angular/core';
import {  DialogService,StorageService } from '@services/services';
import { NavController, ViewController,ModalController } from 'ionic-angular';
import { ServiceProxy } from '@services/services';
export abstract class AppbaseComponent {

  serviceProxy: ServiceProxy;
  navCtrl: NavController;
  dialog: DialogService;
  storage:StorageService;
  viewCtrl: ViewController;
  modalCtrl:ModalController;
  
  constructor(injector: Injector) {
    this.navCtrl = injector.get(NavController);
    this.viewCtrl = injector.get(ViewController);
    this.serviceProxy = injector.get(ServiceProxy);
    this.dialog = injector.get(DialogService);
    this.storage = injector.get(StorageService);
    this.modalCtrl = injector.get(ModalController);
  }
  /**
   * 跳转
   * @param page 跳转页面
   * @param params 参数
   */
  goPage(page, params=null) {
    this.navCtrl.push(page, params)
  }

  /**
   * 关闭弹窗
   */
  close() {
    this.viewCtrl.dismiss();
  }
}
