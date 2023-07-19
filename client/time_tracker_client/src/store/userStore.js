import { makeAutoObservable } from "mobx";

export default class UserStore {
  constructor() {
    this._isAuth = false;
    this._isAdmin = false;
    this._user = {
        id: '',
        userName: ''
    };
    makeAutoObservable(this);
  }

  setIsAuth(bool) {
    this._isAuth = bool;
  }

  setIsAdmin(bool) {
    this._isAdmin = bool;
  }

  
 setUser(response) {
        this._user = {
          userName: response['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
          id: response.sub
        };
      }
      
   
  

  get isAdmin() {
    return this._isAdmin;
  }

  get isAuth() {
    return this._isAuth;
  }

  get user() {
    return this._user;
  }
}
