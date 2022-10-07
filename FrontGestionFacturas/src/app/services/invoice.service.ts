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

  getEnterpriseInvoices(id: number): Observable<any>
  {
    return this.http.get('/api/Invoice/GetEnterpriseInvoices/' + id, this.generateOptions())
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
    return this.http.post('/api/InvoiceLine/' + invoiceLine.invoiceId, this.generateOptions());
  }

  updateInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    return this.http.put('/api/InvoiceLine/' + invoiceLine.id, this.generateOptions());
  }

  deleteInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    console.log("DELETE LINE SERVICE");
    console.table(invoiceLine);
    return this.http.delete('/api/InvoiceLine/' + invoiceLine.id, this.generateOptions());
  }

  generateOptions(): any
  {
    let token = sessionStorage.getItem('token');
    return {
      headers: new HttpHeaders({
        'Authorization':  'Bearer ' + token
      })
    }
  }

}
