import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private http: HttpClient) { }

  getEnterprises(): Observable<any>
  {
    let token = sessionStorage.getItem('token');
    let options = {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
    return this.http.get('/api/Enterprise', options)
  }

  getInvoices(): Observable<any>
  {
    let token = sessionStorage.getItem('token');
    let options = {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
    return this.http.get('/api/Invoice', options)
  }

  getEnterpriseInvoices(id: number): Observable<any>
  {
    let token = sessionStorage.getItem('token');
    let options = {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
    return this.http.get('/api/Invoice/GetEnterpriseInvoices/' + id, options)
  }

  getInvoice(id: number): Observable<any>
  {
    let token = sessionStorage.getItem('token');
    let options = {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
    return this.http.get('/api/Invoice/' + id, options)
  }

  getInvoiceLines(id: number): Observable<any>
  {
    let token = sessionStorage.getItem('token');
    let options = {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
    return this.http.get('/api/InvoiceLine/GetInvoiceLines/' + id, options)
  }
}
