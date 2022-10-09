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
    this.dialogRef.updateSize('80%', '80%');
  }

  onLoadPdf(ev: Event) {
    let iframe = document.getElementById("pdf-iframe");
    //let content = (<HTMLIFrameElement> iframe).contentWindow;

    let content = document.parentElement;
    if(iframe != undefined && content != null)
    {
      document.parentElement?.clientHeight;
      let max = Math.max(content.clientHeight,
                         content.offsetHeight,
                         content.scrollHeight
                         )
      iframe.style.height = max + 'px';
      console.log(iframe.style.height);
    }
    
  }

}