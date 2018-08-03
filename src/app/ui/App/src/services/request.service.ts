import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { NameValueCollection } from '../types/types';
@Injectable()
export class RequestService {
    constructor(public http: HttpClient) {
    }
    /**
     * get
     * @param url 请求地址
     * @param query 请求参数
     * @param headers headers
     */
    get(url: string, query?: NameValueCollection, headers?: NameValueCollection): Observable<any> {
        return this.send('GET', url, query, null, headers);
    }
    /**
     * post
     * @param url 请求地址
     * @param data 提交数据
     * @param headers headers
     * @param query 请求参数
     */
    post(url: string, data?: any, headers?: NameValueCollection, query?: NameValueCollection): Observable<any> {
        return this.send('POST', url, query, data, headers);
    }
    /**
     * put
     * @param url 请求地址
     * @param query 请求参数
     * @param data 上传数据
     * @param headers headers
     */
    put(url: string, data?: any, headers?: NameValueCollection, query?: NameValueCollection): Observable<any> {
        return this.send('PUT', url, query, data, headers);
    }
    /**
     * patch
     * @param url 请求地址
     * @param data 上传数据
     * @param headers headers
     * @param query 请求参数
     */
    patch(url: string, data?: any, headers?: NameValueCollection, query?: NameValueCollection): Observable<any> {
        return this.send('PATCH', url, query, data, headers);
    }
    /**
     * delete
     * @param url 请求地址
     * @param query 请求参数
     * @param headers headers
     */
    delete(url: string, query?: NameValueCollection, headers?: NameValueCollection): Observable<any>{
        return this.send('DELETE', url, query, null, headers);
    }
    /**
     * 发送请求
     * @param method 方法
     * @param url 请求的url
     * @param query 请求参数
     * @param data 请求数据
     * @param header header
     */
    private send(method: string, url: string, query?: NameValueCollection, data?: any, header?: NameValueCollection): Observable<{}> {
        return this.http.request(method,url, {
            body:data,
            headers: this.getHeaders(header),
            params:this.getSearch(query),
            responseType: 'json',
            observe:'body'
        })
    }
    /**
     * 搜索
     * @param query 请求参数
     */
    private getSearch(query?: NameValueCollection): HttpParams {
        let params: string;
        if (query) {
            for (let key in query) {
                if (query.hasOwnProperty(key)) {
                    if(query[key]!==''){
                        params?params += `&${key}=${query[key]}`:params=`${key}=${query[key]}`;
                    }
                }
            }
        }
        return new HttpParams({fromString:params});
    }
    /**
     * header
     * @param header header
     */
    private getHeaders(header?: NameValueCollection): HttpHeaders {
        let headers: HttpHeaders = new HttpHeaders(header).append('Content-Type', 'application/json');
        return headers;
    }
    /**
     * 数据主体
     * @param data 提交数据
     */
    private getBody(data: any): string {
        return data ? JSON.stringify(data) : '';
    }

}