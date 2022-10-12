import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { InvoiceService } from './invoice.service';

describe('InvoiceService', () => {
  let httpClientSpy: {post: jasmine.Spy,
                      put:jasmine.Spy,
                      delete: jasmine.Spy,
                      get: jasmine.Spy};
  let service: InvoiceService;

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['post', 'get', 'delete', 'put']);

    service = new InvoiceService(httpClientSpy as any);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call post function', (done: DoneFn) => {
    const mockResult = {id: 0}
    const mockInvoiceLine = {id: 0, item: '', quantity: 0, itemValue: 0, invoiceId: 0}

    httpClientSpy.post.and.returnValue(of(mockResult));

    service.createInvoiceLine(mockInvoiceLine).subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call put function', (done: DoneFn) => {
    const mockResult = {id: 0}
    const mockInvoiceLine = {id: 0, item: '', quantity: 0, itemValue: 0, invoiceId: 0}

    httpClientSpy.put.and.returnValue(of(mockResult));

    service.updateInvoiceLine(mockInvoiceLine).subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call delete function', (done: DoneFn) => {
    const mockResult = {id: 0}
    const mockInvoiceLine = {id: 0, item: '', quantity: 0, itemValue: 0, invoiceId: 0}

    httpClientSpy.delete.and.returnValue(of(mockResult));

    service.deleteInvoiceLine(mockInvoiceLine).subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call get function', (done: DoneFn) => {
    const mockResult = {id: 0}

    httpClientSpy.get.and.returnValue(of(mockResult));

    service.getInvoiceLines(0).subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call get function', (done: DoneFn) => {
    const mockResult = {id: 0}

    httpClientSpy.get.and.returnValue(of(mockResult));

    service.getInvoice(0).subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call get function', (done: DoneFn) => {
    const mockResult = {id: 0}

    httpClientSpy.get.and.returnValue(of(mockResult));

    service.getEnterprises().subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call get function', (done: DoneFn) => {
    const mockResult = {id: 0}

    httpClientSpy.get.and.returnValue(of(mockResult));

    service.getInvoices().subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });

  it('should call get function', (done: DoneFn) => {
    const mockResult = {id: 0}

    httpClientSpy.get.and.returnValue(of(mockResult));

    service.getInvoicePdf(0).subscribe((result)=>
    {
      expect(result).toEqual(mockResult);
      done();
    });

  });
});
