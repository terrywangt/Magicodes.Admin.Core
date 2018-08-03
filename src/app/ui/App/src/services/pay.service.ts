import { Injectable } from '@angular/core';
import { DialogService } from './dialog.service';

declare let window;
@Injectable()
export class PayService {
  constructor(
    private dialog: DialogService
   
  ) { }

  /**
     * 调起支付宝
     * @param data 支付数据
     */
  goAliPay(data) {
    return new Promise((resolve, reject) => {
      window.Alipay.pay(data, (response) => {
        if (response.resultStatus == '9000') {
          resolve(response)
        } else {
          reject(response)
          this.dialog.tips(response.memo ? response.memo : '支付失败，请重新支付')
        }
      }, (error) => {
        reject(error)
        this.dialog.tips(error.memo)
      })
    })
  }
  /**
   * 
   * @param data 支付数据
   */
  goWechatPay(data) {
    let wechat = (<any>window).Wechat;
    //let paramData = this.urlToJson(data);
    // debug 签名   2d55831d673dfd1523f1b5e370429965
    // release 签名 bc6737ce17e58019af519b9d348349f6 
                    
    let params = {
      appid:data.appId,
      partnerid: data.mchId, // merchant id
      prepayid: data.prepayId, // prepay id
      noncestr: data.nonceStr, // nonce
      timestamp: data.timeStamp, // timestamp
      sign: data.paySign, // signed string
    };
    return new Promise((resolve, reject) => {

      wechat.sendPaymentRequest(params, response => {
        resolve(response);
      }, error => {
        reject(error)
        this.dialog.tips(error)
      })
    })
  }
  /**
 * URL转JSON
 * @param url 待转码的URL
 */
  urlToJson(url: string): any {
    let search = url.substring(url.lastIndexOf("?") + 1);
    let obj = {};
    let reg = /([^?&=]+)=([^?&=]*)/g;
    search.replace(reg, (rs, $1, $2) => {
      let name = decodeURIComponent($1);
      let val = decodeURIComponent($2);
      val = String(val);
      obj[name] = val;
      return rs;
    });
    return obj;
  }
  /**
   * 对象转url params
   * @param data 当前对象
   */
  jsonToUrl(data) {
    let params;
    if (data) {

      for (let key in data) {
        if (data.hasOwnProperty(key)) {
          if (data[key] !== '') {
            params ? params += `&${key}=${data[key]}` : params = `${key}=${data[key]}`;
          }
        }
      }
    }
    return params;
  }
}