import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IInvoiceLine } from '../models/interfaces/invoiceLine.interface';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private http: HttpClient) { }

  getEnterprises(): Observable<any>
  {
    return this.http.get('/api/Enterprise', this.generateOptions())
  }

  getInvoices(): Observable<any>
  {
    return this.http.get('/api/Invoice', this.generateOptions())
  }

  getInvoice(id: number): Observable<any>
  {
    return this.http.get('/api/Invoice/' + id, this.generateOptions())
  }

  getInvoiceLines(id: number): Observable<any>
  {
    return this.http.get('/api/InvoiceLine/GetInvoiceLines/' + id, this.generateOptions())
  }

  createInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    return this.http.post('/api/InvoiceLine/', invoiceLine, this.generateOptions());
  }

  updateInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    return this.http.put('/api/InvoiceLine/' + invoiceLine.id, invoiceLine, this.generateOptions());
  }

  deleteInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    console.log("DELETE LINE SERVICE");
    console.table(invoiceLine);
    return this.http.delete('/api/InvoiceLine/' + invoiceLine.id, this.generateOptions());
  }

  getInvoicePdf(invoice: number): Observable<any>
  {
    console.log("GET PDF SERVICE");
    return this.http.get<any>('/api/Invoice/GetInvoicePdf/' + invoice,this.generateOptions(true));
  }

  generateOptions(isPdf: boolean = false): any
  {
    let token = sessionStorage.getItem('token');
    if(isPdf)
    {
      return {
        headers: new HttpHeaders({
          'Authorization':  'Bearer ' + token,
          'Access-Control-Allow-Origin':  '*',
          'Access-Control-Allow-Methods': '*',
          'Access-Control-Allow-Headers': '*',
          Accept : 'application/pdf',
          observe : 'response'
        }),
        'responseType' : 'arraybuffer'
      }
    }
    return {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token,
        'Access-Control-Allow-Origin':  '*',
        'Access-Control-Allow-Methods': '*',
        'Access-Control-Allow-Headers': '*'
      })
    }
  }

}
