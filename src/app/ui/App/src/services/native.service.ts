import { Injectable, Inject } from "@angular/core";
import { Platform } from 'ionic-angular';
import { SessionService } from './session.service';
import { DialogService } from './dialog.service';
@Injectable()
export class NativeService {
  constructor(public platform:Platform,public session:SessionService,public dialog:DialogService) { }

  /**
   * 判断是否是终端
   * @param key 系统名称
   */
  is(key) {
    return this.platform.is(key);
  }
}
