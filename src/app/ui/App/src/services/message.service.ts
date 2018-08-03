import { Injectable } from "@angular/core";

@Injectable()
export class MessageService {
    private userKey = "user";
    private tokenKey='token';
    constructor(){}
    get(key: string) {
        let item: string = sessionStorage.getItem(key);
        return item ? JSON.parse(item) : null;
    }
    set(key: string, data: any): void {
        sessionStorage.setItem(key, JSON.stringify(data));
    }
    getUser(){
        return this.get(this.userKey);
    }
    setUser(user): void {
        this.set(this.userKey, user);
    };
    remove(key):void{
        sessionStorage.removeItem(key);
    }
    /**
     * 设置token 
     * @param token 存储的token
     */ 
    setToken(token){
        this.set(this.tokenKey,token)
    }
    /**
     * 获取token
     */ 
    getToken(){
        return this.get(this.tokenKey)
    }
}
