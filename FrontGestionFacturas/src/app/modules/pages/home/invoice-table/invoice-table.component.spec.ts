import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, fakeAsync, flushMicrotasks, TestBed } from '@angular/core/testing';
import { AbstractControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { of, throwError } from 'rxjs';
import { IInvoiceLine } from 'src/app/models/interfaces/invoiceLine.interface';
import { InvoiceService } from 'src/app/services/invoice.service';

import { InvoiceTableComponent } from './invoice-table.component';

describe('InvoiceTableComponent', () => {
  const invoiceServiceSpy = jasmine.createSpyObj('InvoiceService', ['']);
  let httpMock: HttpTestingController;

  let component: InvoiceTableComponent;
  let fixture: ComponentFixture<InvoiceTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatDialogModule
      ],
      declarations: [ InvoiceTableComponent ],
      providers: [{
          provide: InvoiceService,
          useValue: invoiceServiceSpy
        },
        {
          provide: MatDialogRef,
          useValue: {updateSize: () => null } 
        },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {invoiceLines: [], invoice: {}}
      }]
    })
    .compileComponents();

    httpMock = TestBed.inject(HttpTestingController);

    fixture = TestBed.createComponent(InvoiceTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('add row form interaction', ()=>{
    let form: FormGroup;
    let item: AbstractControl <any, any>;
    let quantity: AbstractControl <any, any>;
    let itemValue: AbstractControl <any, any>;

    beforeEach(() => {
      form = component.rowForm;
      item = form.controls['item'];
      quantity = form.controls['quantity'];
      itemValue = form.controls['itemValue'];
    });

    afterEach(() =>{
      component.dataSource.data = [];
    });

    it('should get values of form', () => {
      expect(component.formItem).toEqual(item);
      expect(component.formQuantity).toEqual(quantity);
      expect(component.formItemValue).toEqual(itemValue);
    });
    
    it('should include mockItem in dataSource', fakeAsync(() => {
      const mockItem: IInvoiceLine = {
        id: -1,
        item: 'example',
        quantity: 2,
        itemValue: 1,
        invoiceId: 0
      }
  
      item.setValue('example');
      quantity.setValue(2);
      itemValue.setValue(1);
      component.data.invoice.id = 0;
      component.dataSource.data = [];
  
      component['invoiceService'].createInvoiceLine = () =>of({success: true});
      
      expect(component.dataSource.data.length == 0).toBeTrue();

      component.createRow();
      flushMicrotasks();
  
      expect(component.dataSource.data.length > 0).toBeTrue();
    }));

    it('should unmodify dataSource', fakeAsync(() => {
      const mockItem: IInvoiceLine = {
        id: -1,
        item: 'example',
        quantity: 2,
        itemValue: 1,
        invoiceId: 0
      }
  
      item.setValue('example');
      quantity.setValue(2);
      itemValue.setValue(1);
      component.data.invoice.id = 0;
      component.dataSource.data = [];
  
      
      expect(component.dataSource.data.length == 0).toBeTrue();
      
      component['invoiceService'].createInvoiceLine = () => throwError(() => new Error(''));
      component.createRow();
      flushMicrotasks();
  
      expect(component.dataSource.data.length > 0).toBeFalse();
    }));
  });

  describe('edit row form interaction', ()=>{
    const mockRow: IInvoiceLine =
    {
      id: -1,
      item: 'example',
      quantity: 2,
      itemValue: 1,
      invoiceId: 0
    };

    beforeEach(()=>{
      component.dataSource.data = [];
    });

    it('should cancel function excecution', ()=>{
      spyOn(component.dataSource.data, 'indexOf');

      component.editStop(mockRow);

      expect(component.dataSource.data.indexOf).not.toHaveBeenCalled();
    });

    it('should restore row data', ()=>{
      let row = {
        id: mockRow.id,
        item: mockRow.item,
        quantity: mockRow.quantity,
        itemValue: mockRow.itemValue,
        invoiceId: mockRow.invoiceId
      };
      const index = component.dataSource.data.push(row) - 1;

      expect(component.dataSource.data[index]).toEqual(mockRow);

      component.editStart(row);

      row.item = 'edit';
      row.quantity = 20;
      row.itemValue= 11;
      expect(component.storedRow).toEqual(mockRow);
      expect(component.dataSource.data[index]).not.toEqual(mockRow);

      component.editStop(row);

      expect(component.dataSource.data[index]).toEqual(mockRow);
    });

    it('should send data of row to update', ()=>{
      let row = {
        id: mockRow.id,
        item: mockRow.item,
        quantity: mockRow.quantity,
        itemValue: mockRow.itemValue,
        invoiceId: mockRow.invoiceId
      };
      const index = component.dataSource.data.push(row) - 1;

      expect(component.dataSource.data[index]).toEqual(mockRow);

      component.editStart(row);

      row.item = 'edit';
      row.quantity = 20;
      row.itemValue= 11;
      expect(component.dataSource.data[index]).not.toEqual(mockRow);
      expect(component.dataSource.data[index]).toEqual(row);

      component['invoiceService'].updateInvoiceLine = () => of({success: true});
      component.editedRow(row);

      expect(component.storedRow).toBeNull();
    });

    it('should keep stored row', ()=>{
      let row = {
        id: mockRow.id,
        item: mockRow.item,
        quantity: mockRow.quantity,
        itemValue: mockRow.itemValue,
        invoiceId: mockRow.invoiceId
      };
      const index = component.dataSource.data.push(row) - 1;

      expect(component.dataSource.data[index]).toEqual(mockRow);

      component.editStart(row);

      row.item = 'edit';
      row.quantity = 20;
      row.itemValue= 11;
      expect(component.dataSource.data[index]).not.toEqual(mockRow);
      expect(component.dataSource.data[index]).toEqual(row);

      component['invoiceService'].updateInvoiceLine = () => throwError(() => new Error('error'));
      component.editedRow(row);

      expect(component.storedRow).not.toBeNull();
    });
  });

  describe('delete row form interaction', ()=>{
    const mockRow: IInvoiceLine =
    {
      id: -1,
      item: 'example',
      quantity: 2,
      itemValue: 1,
      invoiceId: 0
    };

    it('should send data of row to update', ()=>{
      const index = component.dataSource.data.push(mockRow) - 1;

      expect(component.dataSource.data[index]).toEqual(mockRow);

      component['invoiceService'].deleteInvoiceLine = () => of({success: true});
      component.deleteRow(mockRow);

      expect(component.dataSource.data.indexOf(mockRow)).not.toEqual(index);
    });

    it('should keep stored row', ()=>{
      const index = component.dataSource.data.push(mockRow) - 1;

      expect(component.dataSource.data[index]).toEqual(mockRow);

      component['invoiceService'].deleteInvoiceLine = () => throwError(() => new Error('error'));
      component.deleteRow(mockRow);

      expect(component.dataSource.data.indexOf(mockRow)).toEqual(index);
    });
  });

  describe('calculate extra table variables', ()=>{
    it('should return "small-input"', ()=>{
      expect(component.getClass('quantity')).toEqual('small-input');
    });

    it('should return "medium-input"', ()=>{
      expect(component.getClass('itemValue')).toEqual('medium-input');
    });

    it('should return ""', ()=>{
      expect(component.getClass('example')).toEqual('');
    });

    it('should return sum of invoice lines', ()=>{
      component.dataSource.data.push({id: -1, item: '', quantity: 5, itemValue: 100, invoiceId: -1});
      component.dataSource.data.push({id: -1, item: '', quantity: 9, itemValue: 10, invoiceId: -1});

      expect(component.getTotal()).toEqual(590);

      component.dataSource.data = [];
    });
  });
});
