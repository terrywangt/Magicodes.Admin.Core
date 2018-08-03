import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage';

@Injectable()
export class StorageService {
    constructor(public storage: Storage) {
        this.getToken()
    };

    private userKey = "userInfo";
    authUser = {
        authenticated: false,
        token: '',
        user: null
    };
    /**
     * Storage 存储
     * @param key 存储名
     * @param data 存储数据
     */
    set(key, data) {
        return this.storage.set(key, data)
    }
    /**
     * 获取存储信息
     * @param key 存储名
     */
    get(key) {
        return this.storage.get(key);
    }

    /**
     * 获取默认存储信息
     */
    getUser() {
        return this.get(this.userKey);
    }
    /**
     * 设置默认存储信息
     * @param user 默认用户
     */
    setUser(user) {

        return this.set(this.userKey, user);
    }
    /**
     * 删除存储信息
     * @param key 存储名
     */
    remove(key) {
        return this.storage.remove(key);
    }
    /**
     * 删除用户信息
     */
    removeUser() {
        return this.resetToken();
    }
    /**
     *获取token
     */
    getToken() {
        return this.getUser()
            .then((data) => {
                if (data) {
                    this.authUser = data;
                }
                return this.authUser;
            })
    }
    /**
     * 存储token
     */ 
    setToken(token){
            this.authUser.token=token;
            if(token){
                this.authUser.authenticated=true;
                return this.setUser(this.authUser);
            }
        
    }
    /**
     * 重置token
     */
    resetToken() {
        this.authUser = {
            authenticated: false,
            token: '',
            user: null
        }
        this.setUser(this.authUser);
    }
}