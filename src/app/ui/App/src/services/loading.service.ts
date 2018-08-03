import { Injectable } from '@angular/core';
import { LoadingController,Loading } from 'ionic-angular';

@Injectable()
export class LoadingService {
    constructor(
        private loadingCtrl:LoadingController
    ){}
    /**
   * 加载提示
   */ 
  loading():Promise<Loading> {
    return new Promise(resolve=>{
      let loading = this.loadingCtrl.create({
        showBackdrop:false,
        spinner:'dots'
      });
      loading.present();
      resolve(loading)
    })
  }
}