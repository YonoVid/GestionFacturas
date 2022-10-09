import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IInvoiceData } from 'src/app/models/table/invoice-data';
import { InvoiceService } from 'src/app/services/invoice.service';

@Component({
  selector: 'app-invoice-table',
  templateUrl: './invoice-pdf.component.html',
  styleUrls: ['./invoice-pdf.component.scss']
})
export class InvoicePdfComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private invoiceService: InvoiceService,
              public dialogRef: MatDialogRef<any>) { }

  ngOnInit( ): void {
    this.dialogRef.updateSize('80%', '80%');
  }

}