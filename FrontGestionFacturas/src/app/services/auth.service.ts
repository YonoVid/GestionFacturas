import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IUserAuth } from '../models/interfaces/user-auth.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }
  /**
  * Make a POST request to external API to get login JWT token
  * @param email User email
  * @param password User password
  * @returns Observable to use the response of the HTTP petition
  */
  login(email: string, password: string): Observable<any>
  {
    let options = {
      headers: new HttpHeaders({
        'Access-Control-Allow-Origin':  '*',
        'Access-Control-Allow-Methods': '*',
        'Access-Control-Allow-Headers': '*'
      })
    }
    let body: IUserAuth = {
      email: email,
      password: password
    }
    return this.http.post('/api/Users/Login', body, options)
  }
  /**
   * Make a POST request  to external API to register a new user
   * @param name User name
   * @param email User email
   * @param password User password
   * @returns Observable to use the response of the HTTP petition
   */
  register(name: string, email: string, password: string): Observable<any>
  {
    let body: IUserAuth = {
      name: name,
      email: email,
      password: password
    }
    return this.http.post('/api/Users/Register', body)
  }
}
