import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { IInvoiceLine } from 'src/app/models/interfaces/invoiceLine.interface';
import { IInvoiceData } from 'src/app/models/table/invoice-data';
import { InvoiceService } from 'src/app/services/invoice.service';

@Component({
  selector: 'app-invoice-table',
  templateUrl: './invoice-table.component.html',
  styleUrls: ['./invoice-table.component.scss']
})
export class InvoiceTableComponent implements OnInit {

  displayedColumns: string[] = ['item', 'quantity', 'itemValue',
                                'invoice-totalAmount', 'invoice-actions'];
  dataSource: MatTableDataSource<IInvoiceLine> = new MatTableDataSource<IInvoiceLine>();

  
  rowForm: FormGroup = new FormGroup({});
  storedRow: IInvoiceLine | null = null;

  columnsSchema = COLUMNS_SCHEMA;

  constructor(@Inject(MAT_DIALOG_DATA) public data: IInvoiceData,
              private formBuilder: FormBuilder,
              private invoiceService: InvoiceService,
              public dialogRef: MatDialogRef<any>) { }

  ngOnInit( ): void {
    this.dialogRef.updateSize('80%', '80%');
    this.dataSource.data = this.data.invoiceLines;

    //Inicialización de formulario
    this.rowForm = this.formBuilder.group(
      {
        item : ['Nueva linea', Validators.compose([Validators.maxLength(30), Validators.required])],
        quantity: [1, Validators.compose([Validators.min(1), Validators.required])],
        itemValue: [0, Validators.compose([Validators.min(0), Validators.required])]
      }
    )
  }

  get formItem() {return this.rowForm.get('item');}
  get formQuantity() {return this.rowForm.get('quantity');}
  get formItemValue() {return this.rowForm.get('itemValue');}

  createRow()
  {
    let row: IInvoiceLine = 
    {
      id: -1,
      item: this.formItem?.value,
      quantity: this.formQuantity?.value,
      itemValue: this.formItemValue?.value,
      invoiceId: this.data.invoice.id,
      isEdit: false
    }

    console.log("CREATING NEW ROW");

    this.invoiceService.createInvoiceLine(row).subscribe({
      next: (result)=>{
        this.dataSource.data.push(row);
          
        this.dataSource._updateChangeSubscription();
      },
      error: (error)=> console.error(error),
      complete: ()=> console.info(`CREATED:: ${row.id} ${row.item}`)
    })
  }

  editStart(row: IInvoiceLine)
  {
    this.storedRow = {
      id: row.id,
      item: row.item,
      quantity: row.quantity,
      itemValue: row.itemValue,
      invoiceId: row.invoiceId,
      isEdit: false
    };
    console.info(`EDITING:: ${row.id} ${row.item}`);
  }

  editStop(row: IInvoiceLine)
  {
    if(this.storedRow == null) return;

    const index: number = this.dataSource.data.indexOf(row, 0);
    if (index > -1) {
      console.info('CANCEL EDITING');
      this.dataSource.data[index].item = this.storedRow.item;
      this.dataSource.data[index].quantity = this.storedRow.quantity;
      this.dataSource.data[index].itemValue = this.storedRow.itemValue;

      this.storedRow = null;
    }
  }

  editedRow(row: IInvoiceLine)
  {
    this.invoiceService.updateInvoiceLine(row).subscribe({
      next: (result)=>{
          this.storedRow = null;
      },
      error: (error)=> console.error(error),
      complete: ()=> console.info(`UPDATED:: ${row.id} ${row.item}`)
    })
  }

  deleteRow(row: IInvoiceLine)
  {
    this.invoiceService.deleteInvoiceLine(row).subscribe(
    {
      next: ()=>{
        const index: number = this.dataSource.data.indexOf(row, 0);
        if (index > -1) {
          
          this.dataSource.data.splice(index, 1);
          
          this.dataSource._updateChangeSubscription();
        }
      },
      error: (error)=> console.error(error),
      complete: ()=> console.info(`DELETE:: ${row.id} ${row.item}`)
    })
    
  }

}

const COLUMNS_SCHEMA = [
  {
    key: "item",
    type: "text",
    label: "Descripción"
  },
  {
    key: "quantity",
    type: "number",
    label: "Cantidad"
  },
  {
    key: "itemValue",
    type: "number",
    label: "Valor"
  }
]