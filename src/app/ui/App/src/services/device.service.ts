import { Injectable, Inject } from "@angular/core";
import { Platform } from 'ionic-angular';
import { SessionService } from '@services/session.service';
import { DialogService } from '@services/dialog.service';
import { ImagePicker } from '@ionic-native/image-picker';
@Injectable()
export class DeviceService {
  constructor(public platform:Platform,public session:SessionService,public dialog:DialogService) { }
  
  /**
   * 判断是否是终端
   * @param key 系统名称
   */
  is(key) {
    return this.platform.is(key);
  }
}
