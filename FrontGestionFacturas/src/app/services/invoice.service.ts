import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private http: HttpClient) { }

  getInvoices(): Observable<any>
  {
    let token = sessionStorage.getItem('token');
    let options = {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
    return this.http.get('/api/Users', options)
  }
}
