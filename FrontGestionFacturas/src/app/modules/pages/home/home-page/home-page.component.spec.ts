import { EventListenerFocusTrapInertStrategy } from '@angular/cdk/a11y';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { of, throwError } from 'rxjs';
import { IEnterprise } from 'src/app/models/interfaces/enterprise.interface';
import { IInvoiceTableData } from 'src/app/models/interfaces/invoice-table-data.interface';
import { IInvoice } from 'src/app/models/interfaces/invoice.interface';
import { UserRol } from 'src/app/models/interfaces/user.interface';
import { InvoiceService } from 'src/app/services/invoice.service';

import { HomePageComponent } from './home-page.component';

describe('HomePageComponent', () => {
  const invoiceServiceSpy = jasmine.createSpyObj('InvoiceService', ['']);

  let component: HomePageComponent;
  let fixture: ComponentFixture<HomePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        // Import of modules for http
        HttpClientTestingModule,
        MatDialogModule
      ],
      declarations: [ HomePageComponent, MatPaginator ],
      providers: [ 
        DatePipe,
        {
          provide: InvoiceService,
          useValue: invoiceServiceSpy
        }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomePageComponent);
    component = fixture.componentInstance;

    component['invoiceService'].getEnterprises = () =>of([mockEnterprise]);
    component['invoiceService'].getInvoices = () =>of([mockInvoice]);
    fixture.detectChanges();
  });

  it('should create and set paginator to data source', () => {

    expect(component).toBeTruthy();
    expect(component.paginator).toEqual(component.dataSource.paginator);
  });

  describe('obtain data', () => {
    afterEach(()=>{
        component.invoices = [];
        component.enterprises = [];
        component['invoiceService'].getEnterprises = () =>of([mockEnterprise]);
        component['invoiceService'].getInvoices = () =>of([mockInvoice]);
    });
    it('should store data from service', () => {
      component.getInvoiceData();

      expect(component.enterprises[mockEnterprise.id]).toEqual(mockEnterprise);
      expect(component.invoices[mockInvoice.id]).toEqual(mockInvoice);
    });

    it('should only store data from enterprise', () => {
      
      component['invoiceService'].getInvoices = () => throwError(() => new Error());

      component.getInvoiceData();

      expect(component.enterprises[mockEnterprise.id]).toEqual(mockEnterprise);
      expect(component.invoices[mockInvoice.id]).not.toEqual(mockInvoice);
    });

    it('should not store data', () => {
      
      component['invoiceService'].getEnterprises = () => throwError(() => new Error());

      component.getInvoiceData();

      expect(component.enterprises[mockEnterprise.id]).not.toEqual(mockEnterprise);
      expect(component.invoices[mockInvoice.id]).not.toEqual(mockInvoice);
    });
  });

  describe('sort data', () => {
    beforeEach(()=>{
      component.invoicesData = [mockSortInvoiceB, mockSortInvoiceA];
    })
    afterEach(()=>{
        component.invoicesData = [];
        component.dataSource.data = [];
    });

    it('should sort descending by id', () => {
      component.sortData({active: '', direction: ''});

      expect(component.dataSource.data).toEqual(component.invoicesData);
    });

    it('should sort descending by id', () => {
      component.invoicesData = [mockSortInvoiceA, mockSortInvoiceB];
      component.sortData({active: 'id', direction: 'desc'});

      expect(component.dataSource.data[0]).toEqual(mockSortInvoiceB);
    });

    it('should sort ascending by id', () => {
      component.sortData({active: 'id', direction: 'asc'});

      expect(component.dataSource.data[0]).toEqual(mockSortInvoiceA);
    });

    it('should sort ascending by name', () => {
      component.sortData({active: 'name', direction: 'asc'});

      expect(component.dataSource.data[0]).toEqual(mockSortInvoiceA);
    });

    it('should sort ascending by enterpriseId', () => {
      component.sortData({active: 'enterpriseId', direction: 'asc'});

      expect(component.dataSource.data[0]).toEqual(mockSortInvoiceA);
    });

    it('should sort ascending by totalAmount', () => {
      component.sortData({active: 'totalAmount', direction: 'asc'});

      expect(component.dataSource.data[0]).toEqual(mockSortInvoiceA);
    });

    it('should keep order', () => {
      component.sortData({active: 'default', direction: 'asc'});

      expect(component.dataSource.data).toEqual(component.invoicesData);
    });
  });

  describe('stored enterprises', () => {
    beforeEach(()=>{
        component.enterprises = [];
    });
    afterEach(()=>{
      component.enterprises = [];
    });
    it('should find enterprise stored', () => {
      component.enterprises[mockEnterprise.id] = mockEnterprise;

      expect(component.getEnterpriseName(mockEnterprise.id)).toEqual(mockEnterprise.name);
    });

    it('should not find a enterprise stored', () => {
      expect(component.getEnterpriseName(mockEnterprise.id)).not.toEqual(mockEnterprise.name);
    });
  });

  describe('open mat dialog', () => {
    
    describe('dialog with invoice table', () => {
      beforeEach(()=>{
        component.invoices[mockEnterprise.id] = mockInvoice;
        component.enterprises[mockEnterprise.id] = mockEnterprise;
      });

      afterEach(() => {
        component.invoices = [];
        component.enterprises = [];
      });

      it('should open dialog from invoice table call', () => {
        spyOn(component.dialog, 'open');
        component['invoiceService'].getInvoiceLines = () =>of({success: true});
  
        component.openInvoiceTable(mockEnterprise.id);
  
        expect(component.dialog.open).toHaveBeenCalled();
      });
  
      it('should not open dialog from invoice table call', () => {
        spyOn(component.dialog, 'open');
        component['invoiceService'].getInvoiceLines = () => throwError(() => new Error('error'));
  
        component.openInvoiceTable(mockEnterprise.id);
  
        expect(component.dialog.open).not.toHaveBeenCalledWith("");
      });

    });
    describe('dialog with invoice pdf', () =>{
      afterEach(() => {
        component.invoices = [];
        component.enterprises = [];
      });
      it('should open dialog from invoice pdf call', () => {
        spyOn(component.dialog, 'open');
        component['invoiceService'].getInvoicePdf = () =>of({success: true});
  
        component.openInvoicePdf(0);
  
        expect(component.dialog.open).toHaveBeenCalled();
      });

      it('should not open dialog from invoice pdf call', () => {
        spyOn(component.dialog, 'open');
        component['invoiceService'].getInvoicePdf = () => throwError(() => new Error('error'));
  
        component.openInvoicePdf(0);
  
        expect(component.dialog.open).not.toHaveBeenCalled();
      });
    });

  });
});

const mockEnterprise: IEnterprise = {
  id: -1,
  name: 'enterprise',
  user: {id: -1, name: '', email: '', password: '',
         userRol: UserRol.USER,
         createdBy: '', createdDate: new Date(),
         updatedBy: '', updatedDate: new Date(),
         deletedBy: '', deletedDate: new Date(), isDeleted: false},
  userId: -1,
  createdBy: '', createdDate: new Date(),
  updatedBy: '', updatedDate: new Date(),
  deletedBy: '', deletedDate: new Date(), isDeleted: false
};
const mockInvoice: IInvoice = {
  id: -1,
  name: 'example',
  taxPercentage: 19,
  totalAmount: 1000,
  invoiceLines: [],
  enterprise: mockEnterprise,
  enterpriseId: -1,
  createdBy: '', createdDate: new Date(),
  updatedBy: '', updatedDate: new Date(),
  deletedBy: '', deletedDate: new Date(), isDeleted: false
};

const mockSortInvoiceA: IInvoiceTableData = {
  id: 1,
  name: 'a',
  taxPercentage: 1,
  totalAmount: 1,
  enterprise: 'a',
  createdDate: 'a'
};

const mockSortInvoiceB: IInvoiceTableData = {
  id: 2,
  name: 'z',
  taxPercentage: 10,
  totalAmount: 10,
  enterprise: 'z',
  createdDate: 'z'
};
