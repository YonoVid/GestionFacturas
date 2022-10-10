import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { InvoiceService } from 'src/app/services/invoice.service';

@Component({
  selector: 'app-invoice-pdf',
  templateUrl: './invoice-pdf.component.html',
  styleUrls: ['./invoice-pdf.component.scss']
})
export class InvoicePdfComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              public invoiceService: InvoiceService,
              public dialogRef: MatDialogRef<any>) { }

  ngOnInit( ): void {
    // Update modal dialog size
    this.dialogRef.updateSize('80%', '80%');
  }
}