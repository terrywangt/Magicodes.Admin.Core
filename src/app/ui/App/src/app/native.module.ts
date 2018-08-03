import { NgModule } from '@angular/core';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { ImagePicker } from '@ionic-native/image-picker';
import { File } from '@ionic-native/file';
import { FileTransfer } from '@ionic-native/file-transfer';
import { FileOpener } from '@ionic-native/file-opener';
import { AppMinimize } from '@ionic-native/app-minimize';
@NgModule({
    providers:[
        StatusBar,
        SplashScreen,
        ImagePicker,
        AppMinimize,
        File,
        FileTransfer,
        FileOpener
    ]
})
export class NativeModule{}
