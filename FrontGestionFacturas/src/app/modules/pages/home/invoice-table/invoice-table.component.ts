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
  // Columns to be shown
  displayedColumns: string[] = ['item', 'quantity', 'itemValue',
                                'invoice-totalAmount', 'invoice-actions'];
  // Data source of the table
  dataSource: MatTableDataSource<IInvoiceLine> = new MatTableDataSource<IInvoiceLine>();

  // Form to add rows
  rowForm: FormGroup = new FormGroup({});
  // Saved row data
  storedRow: IInvoiceLine | null = null;
  //Definition of the editable columns
  columnsSchema = COLUMNS_SCHEMA;

  constructor(@Inject(MAT_DIALOG_DATA) public data: IInvoiceData,
              private formBuilder: FormBuilder,
              private invoiceService: InvoiceService,
              public dialogRef: MatDialogRef<any>) { }

  ngOnInit( ): void {
    this.dialogRef.updateSize('80%', '80%');
    this.dataSource.data = this.data.invoiceLines;

    //Init of form
    this.rowForm = this.formBuilder.group(
      {
        item : ['Nueva linea', Validators.compose([Validators.maxLength(30), Validators.required])],
        quantity: [1, Validators.compose([Validators.min(1), Validators.required])],
        itemValue: [0, Validators.compose([Validators.min(0), Validators.required])]
      }
    )
  }
  // Get to access the form values
  get formItem() {return this.rowForm.get('item');}
  get formQuantity() {return this.rowForm.get('quantity');}
  get formItemValue() {return this.rowForm.get('itemValue');}
  /**
   * Call a service to add a new invoice line,
   * if the action is successful update the interface.
   */
  createRow()
  {
    // Create a invoice line object with the form data
    let row: IInvoiceLine = 
    {
      id: -1,
      item: this.formItem?.value,
      quantity: this.formQuantity?.value,
      itemValue: this.formItemValue?.value,
      invoiceId: this.data.invoice.id
    }
    //console.log("CREATING NEW ROW");

    // Call the service
    this.invoiceService.createInvoiceLine(row).subscribe({
      next: (result)=>{
        this.dataSource.data.push(result);
        row = result;
        this.dataSource._updateChangeSubscription();
      },
      error: (error)=> console.error(error),
      complete: ()=> console.info(`CREATED:: ${row.id} ${row.item}`)
    })
  }
  /**
   * Save the initial data of the invoice line.
   * @param row Data of the invoice line to be modified.
   */
  editStart(row: IInvoiceLine)
  {
    // Save the invoice line data
    this.storedRow = {
      id: row.id,
      item: row.item,
      quantity: row.quantity,
      itemValue: row.itemValue,
      invoiceId: row.invoiceId
    };
    console.info(`EDITING:: ${row.id} ${row.item}`);
  }
  /**
   * Undo the changes made to a invoice line in the
   * table.
   * @param row Data of the modified invoice line.
   */
  editStop(row: IInvoiceLine)
  {
    // Check if data was stored
    if(this.storedRow == null) return;

    //Undo the changes to the invoice line
    const index: number = this.dataSource.data.indexOf(row, 0);
    if (index > -1) {
      console.info('CANCEL EDITING');
      this.dataSource.data[index].item = this.storedRow.item;
      this.dataSource.data[index].quantity = this.storedRow.quantity;
      this.dataSource.data[index].itemValue = this.storedRow.itemValue;

      this.storedRow = null;
    }
  }
  /**
   * Call a service to update the line of a invoice,
   * if the action is successful update the interface. 
   * @param row Data of the modified invoice line.
   */
  editedRow(row: IInvoiceLine)
  {
    // Call the service
    this.invoiceService.updateInvoiceLine(row).subscribe({
      next: (result)=>{
          this.storedRow = null;
      },
      error: (error)=> console.error(error),
      complete: ()=> console.info(`UPDATED:: ${row.id} ${row.item}`)
    })
  }
  /**
   * Call a service to delete the line of a invoice,
   * if the action is successful update the interface.
   * @param row Data of the new invoice line.
   */
  deleteRow(row: IInvoiceLine)
  {
    // Call the service
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
  /**
   * Get the style class of a field for the form of creating
   * invoice lines.
   * @param key Key of the column. 
   * @returns String with the class of the field.
   */
  getClass(key: string)
  {
    if(key === 'quantity') return 'small-input';
    if(key === 'itemValue') return 'medium-input';
    // If no key is identified return empty string
    return '';
  }
  /**
   * Calculate the sum of each row subtotal.
   * @returns Total import without taxes.
   */
  getTotal()
  {
    let total: number = 0;
    this.data.invoiceLines.forEach((line) => total += line.itemValue * line.quantity);
    
    return total;
  }

}

// Definition of the editable columns
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