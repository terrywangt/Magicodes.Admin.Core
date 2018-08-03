import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpErrorResponse, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { Loading, ModalController } from 'ionic-angular';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { environment } from '@env';
import { DialogService } from './dialog.service';
import { SessionService } from './session.service';
import { LoadingService } from './loading.service';
import { StorageService } from './storage.service';
export interface IValidationErrorInfo {
    message: string;
    members: string[];
}

export interface IErrorInfo {
    code: number;
    message: string;
    details: string;
    validationErrors: IValidationErrorInfo[];
}

export interface IAjaxResponse {
    success: boolean;
    result?: any;
    targetUrl?: string;
    error?: IErrorInfo;
    unAuthorizedRequest: boolean;
    __abp: boolean;

}



@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private requestNum: number = 0;
    private isLoding: boolean = true;
    private loadingTemp: Loading;
    constructor(
        private dialog: DialogService,
        private session: SessionService,
        private loading: LoadingService,
        private stoage: StorageService,
        private modalCtrl: ModalController
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        let interceptObservable = new Subject<HttpEvent<any>>();

        let token = this.stoage.authUser.token;
        const reqUrl = req.url;
        const clonedRequest = req.clone({
            url: req.url.replace(reqUrl, `${environment.domain}${reqUrl}`),
            setHeaders: { 'Authorization': `Bearer ${token}` }
        });
        if (!this.requestNum) {
            this.loading.loading().then(loading => {
                this.loadingTemp = loading;
            });
        }
        this.requestNum++;
        next.handle(clonedRequest)
            .subscribe((event: HttpEvent<any>) => {
                this.handleSuccessResponse(event, interceptObservable);
            }, (error: any) => {
                this.requestNum--;
                if (!this.requestNum) {
                    this.loadingTemp.dismissAll();
                }
                
                switch (error.status) {
                    case 400:
                        this.errorMsg(error)
                        break;
                    case 401:
                        this.dialog.tips('请重新登录');
                        let modal = this.modalCtrl.create('LoginPage');
                        modal.present();
                        break;
                    case 403:
                        this.dialog.tips('无权限访问');
                        break;
                    case 404:
                        this.dialog.tips('未找到对应的数据');
                        break;
                    case 500:
                        this.errorMsg(error)
                        break;
                    default:
                        this.dialog.tips('未知错误')
                }


            });

        return interceptObservable;





        // let token = this.stoage.authUser.token;
        // const reqUrl = req.url;
        // const clonedRequest = req.clone({
        //     url: req.url.replace(reqUrl, `${environment.domain}${reqUrl}`),
        //     setHeaders: { 'Authorization': `Bearer ${token}` }
        // });

        // if (!this.requestNum) {
        //     this.loading.loading().then(loading => {
        //         this.loadingTemp = loading;
        //     });
        // }
        // this.requestNum++;
        // return next.handle(clonedRequest).do((response) => {
        //     if (response.type == 4) {
        //         this.requestNum--;
        //         if (!this.requestNum) {
        //             this.loadingTemp.dismissAll();
        //         }
        //     }
        // }, (error: HttpErrorResponse) => {
        //     this.requestNum--;
        //     if (!this.requestNum) {
        //         this.loadingTemp.dismissAll();
        //     }
        //     switch (error.status) {
        //         case 400:
        //             let errorMsg = error.error;
        //             let msg = errorMsg.error.message;
        //             if (errorMsg.error.details) {
        //                 msg = errorMsg.error.details.replace(`[Validation narrative title]`, '').replace('-', '');
        //             }
        //             this.dialog.tips(msg);
        //             break;

        //     }
        // });
    }
    errorMsg(error) {
        this.blobToText(error.error).subscribe((data)=>{
            let errorMsg = JSON.parse(data);
            let msg = errorMsg.error.message;
            if (errorMsg.error.details) {
                msg = errorMsg.error.details.replace(`[Validation narrative title]`, '').replace('-', '');
            }
            this.dialog.tips(msg);
        })
    }
    protected handleSuccessResponse(event: HttpEvent<any>, interceptObservable: Subject<HttpEvent<any>>): void {

        if (event instanceof HttpResponse) {
            this.requestNum--;
            if (!this.requestNum) {
                this.loadingTemp.dismissAll();
            }
            if (event.body instanceof Blob && event.body.type && event.body.type.indexOf("application/json") >= 0) {
                var clonedResponse = event.clone();

                this.blobToText(event.body).subscribe(json => {
                    const responseBody = json == "null" ? {} : JSON.parse(json);

                    var modifiedResponse = this.handleResponse(event.clone({
                        body: responseBody
                    }));

                    interceptObservable.next(modifiedResponse.clone({
                        body: new Blob([JSON.stringify(modifiedResponse.body)], { type: 'application/json' })
                    }));

                    interceptObservable.complete();
                });
            } else {
                interceptObservable.next(event);
                interceptObservable.complete();
            }
        }
    }


    logError(error: any): void {
        console.error(error);
    }



    handleNonAbpErrorResponse(error) {
        console.log(error)

        // switch (error.status) {
        //     case 401:
        //         this.dialog.tips('请重新登录');
        //         break;
        //     case 403:
        //         this.dialog.tips('无权限访问');
        //         break;
        //     case 404:
        //         this.dialog.tips('未找到对应的数据');
        //         break;
        //     default:
        //         this.dialog.tips('未知错误')
        // }
    }

    handleAbpResponse(response: HttpResponse<any>, ajaxResponse: IAjaxResponse): HttpResponse<any> {
        var newResponse: HttpResponse<any>;

        if (ajaxResponse.success) {

            newResponse = response.clone({
                body: ajaxResponse.result
            });

        } else {

            newResponse = response.clone({
                body: ajaxResponse.result
            });
            this.handleNonAbpErrorResponse(ajaxResponse);

        }

        return newResponse;
    }

    getAbpAjaxResponseOrNull(response: HttpResponse<any>): IAjaxResponse | null {
        if (!response || !response.headers) {
            return null;
        }

        var contentType = response.headers.get('Content-Type');
        if (!contentType) {
            this.logError('Content-Type is not sent!');
            return null;
        }

        if (contentType.indexOf("application/json") < 0) {
            this.logError('Content-Type is not application/json: ' + contentType);
            return null;
        }

        var responseObj = JSON.parse(JSON.stringify(response.body));
        if (!responseObj.__abp) {
            return null;
        }

        return responseObj as IAjaxResponse;
    }

    handleResponse(response: HttpResponse<any>): HttpResponse<any> {
        var ajaxResponse = this.getAbpAjaxResponseOrNull(response);
        if (ajaxResponse == null) {
            return response;
        }

        return this.handleAbpResponse(response, ajaxResponse);
    }

    blobToText(blob: any): Observable<string> {
        return new Observable<string>((observer: any) => {
            if (!blob) {
                observer.next("");
                observer.complete();
            } else {
                let reader = new FileReader();
                reader.onload = function () {
                    observer.next(this.result);
                    observer.complete();
                }
                reader.readAsText(blob);
            }
        });
    }

}