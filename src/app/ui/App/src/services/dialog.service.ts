import { Injectable } from '@angular/core';
import { AlertController, ToastController, Alert } from 'ionic-angular';

@Injectable()
export class DialogService {
    constructor(
        private alertCtrl: AlertController,
        private toastCtrl: ToastController
    ) {
    }

    /**
     * 弹窗提示
     * @param msg 提示内容
     * @param duration 显示时间（毫秒）
     */
    tips(msg: string, duration = 3000) {
        let toast = this.toastCtrl.create({
            message: msg,
            cssClass:'text-center',
            duration: duration,
            position: 'middle'
        });
        toast.present();
    }
       /**
     * 单确认弹框
     * @param msg 提示信息
     * @param okText 确认按钮文字
     * @param okHandler 确认事件
     * @param cancelText 取消按钮文字
     * @param cancelHandler 取消事件
     * @param title 提示title
     */
    onlyOk(msg: string, okText?: string, okHandler?: any, title?:string) {
        let alert = this.alertCtrl.create({
            title:title?title:'提示',
            cssClass:'alert-confirm',
            message: `<h6>${msg}</h6>`,
            buttons: [
                {
                    text: okText ? okText : '确定',
                    handler: okHandler
                }
            ],
            enableBackdropDismiss: false
        });
        alert.present();
    }
    /**
     * 确认弹框
     * @param msg 提示信息
     * @param okText 确认按钮文字
     * @param okHandler 确认事件
     * @param cancelText 取消按钮文字
     * @param cancelHandler 取消事件
     * @param title 提示title
     */
    confirm(msg: string, okText?: string, cancelText?: string, okHandler?: any, cancelHandler?: any, title?: string) {
        let alert = this.alertCtrl.create({
            title: title ? title : '确认',
            cssClass: 'alert-confirm',
            message: `<h6>${msg}</h6>`,
            buttons: [
                {
                    text: cancelText ? cancelText : '取消',
                    handler: cancelHandler
                },
                {
                    text: okText ? okText : '确定',
                    handler: okHandler
                }
            ],
            enableBackdropDismiss: false
        });
        alert.present();
    }
    /**
     * 弹出单选
     * @param list 列表
     * @param okText 确定按钮文本
     * @param cancelText 取消按钮文本
     * @param okHandler 确定事件
     * @param title 弹窗标题 
     */
    radio(list: Array<any>, okText?: string, cancelText?: string, okHandler?: any, title?: any, css?: string) {
        let alert = this.alertCtrl.create({
            cssClass: css ? `dialog-radio ${css}` : 'dialog-radio'
        });
        alert.setTitle(title);
        list.forEach((data, index, array) => {
            data.checked = false;
            if (!index) {
                data.checked = true;
            }
            alert.addInput({
                type: 'radio',
                label: data.value,
                value: data.value,
                checked: data.checked
            });
        })

        alert.addButton(cancelText);
        alert.addButton({
            text: okText,
            handler: data => {
                okHandler(data)
            }
        });
        alert.present();
    }
}
