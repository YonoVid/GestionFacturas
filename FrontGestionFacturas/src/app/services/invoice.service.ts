import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IInvoiceLine } from '../models/interfaces/invoiceLine.interface';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private http: HttpClient) { }

  /**
   * Send a GET request to external API to obtain available enterprises data.
   * @returns Observable to use the response of the HTTP petition.
   */
  getEnterprises(): Observable<any>
  {
    return this.http.get('/api/Enterprise', this.generateOptions())
  }
  /**
   * Send a GET request to external API to obtain available invoices data.
   * @returns Observable to use the response of the HTTP petition.
   */
  getInvoices(): Observable<any>
  {
    return this.http.get('/api/Invoice', this.generateOptions())
  }
  /**
   * Send a GET request to external API to obtain the data of a invoice
   * if available.
   * @param id Id of the invoice to look up for.
   * @returns Observable to use the response of the HTTP petition.
   */
  getInvoice(id: number): Observable<any>
  {
    return this.http.get('/api/Invoice/' + id, this.generateOptions())
  }
  /**
   * Send a GET request to external API to obtain the data of a invoice
   * line if available.
   * @param id Id of the invoice line to look up for.
   * @returns Observable to use the response of the HTTP petition.
   */
  getInvoiceLines(id: number): Observable<any>
  {
    return this.http.get('/api/InvoiceLine/GetInvoiceLines/' + id, this.generateOptions())
  }
  /**
   * Send a POST request to external API to create a new invoice line
   * in the database.
   * @param invoiceLine Data of the invoice line to be created.
   * @returns Observable to use the response of the HTTP petition.
   */
  createInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    return this.http.post('/api/InvoiceLine/', invoiceLine, this.generateOptions());
  }
  /**
   * Send a PUT request to external API to modify the data of a
   * invoice line in the database.
   * @param invoiceLine Data of the invoice line be modified.
   * @returns Observable to use the response of the HTTP petition.
   */
  updateInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    return this.http.put('/api/InvoiceLine/' + invoiceLine.id, invoiceLine, this.generateOptions());
  }
  /**
   * Send a DELETE request to external API to remove a invoice
   * line from the database
   * @param invoiceLine 
   * @returns Observable to use the response of the HTTP petition.
   */
  deleteInvoiceLine(invoiceLine: IInvoiceLine): Observable<any>
  {
    console.log("DELETE LINE SERVICE");
    console.table(invoiceLine);
    return this.http.delete('/api/InvoiceLine/' + invoiceLine.id, this.generateOptions());
  }
  /**
   * Send a GET request to external API to generate a pdf from
   * the related data of a invoice and return the result. 
   * @param invoice 
   * @returns Observable to use the response of the HTTP petition
   * with the pdf(data as arraybuffer).
   */
  getInvoicePdf(invoice: number): Observable<any>
  {
    console.log("GET PDF SERVICE");
    return this.http.get<any>('/api/Invoice/GetInvoicePdf/' + invoice,this.generateOptions(true));
  }
  /**
   * Generate the neccesary options to make a call to the
   * Backend API.
   * @param isPdf If the options should be for a pdf creation
   * request.
   * @returns Return a object with the required options to make a
   * successful call to external API.
   */
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
