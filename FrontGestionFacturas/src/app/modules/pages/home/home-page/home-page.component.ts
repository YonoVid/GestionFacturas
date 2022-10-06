import { Component, Inject, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { IEnterprise } from 'src/app/models/interfaces/enterprise.interface';
import { IInvoice } from 'src/app/models/interfaces/invoice.interface';
import { IInvoiceLine } from 'src/app/models/interfaces/invoiceLine.interface';
import { InvoiceService } from 'src/app/services/invoice.service';

import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {

  displayedColumns: string[] = ['home-id', 'home-name', 'home-enterprise', 'home-date', 'home-edit'];
  dataSource: MatTableDataSource<IInvoice> = new MatTableDataSource<IInvoice>();

  invoices: { [id: number]: IInvoice;} = [];
  enterprises: { [id: number]: IEnterprise;} = [];

  loadingData: boolean = true;
  
  constructor(private invoiceService: InvoiceService,
              public dialog: MatDialog) { }

  ngOnInit(): void {
    this.getInvoiceData();
  }

  openInvoiceTable(invoiceId: number) {
    let invoice = this.invoices[invoiceId];
    console.info(`${invoiceId} || ${invoice?.name}`);
    this.invoiceService.getInvoiceLines(invoice.id).subscribe({
      next: (response: IInvoiceLine[]) =>
      {
        let total = 0;
        response.forEach((line) => total += line.quantity * line.itemValue);

        this.dialog.open(HomeInvoiceTable, {
          data:
          {
            invoice: invoice,
            enterprise: this.enterprises[invoice.enterpriseId],
            invoiceLines: response,
            total: total
          } 
        });
        
        console.table(response);
      },
      error: (error) => console.error(error),
      complete: () => console.info('Datos de tabla recopilados')
    })
    
  }

  getEnterpriseName(id: number): string
  {
    if(id in this.enterprises)
    {
      return this.enterprises[id].name;
    }
    return "";
  }

  getInvoiceData()
  {
    this.loadingData = true;
    this.enterprises = [];
    this.invoices = [];

    this.invoiceService.getEnterprises().subscribe({
      next: (response: IEnterprise[]) => {
        response.forEach((enterprise) => this.enterprises[enterprise.id] = enterprise);

        console.table(response);

        this.invoiceService.getInvoices().subscribe({
          next: (response: IInvoice[]) => {
            response.forEach((invoice) => this.invoices[invoice.id] = invoice);
            console.table(response);

          },
          error: (error) => console.log(error),
          complete: () => 
          {
            this.dataSource.data = Object.values(this.invoices);
            this.loadingData = false;
            console.log("Obtener Facturas desde API:: Finalizado")
          }
        });

      },
      error: (error) => console.log(error),
      complete: () => 
      {
        console.log("Obtener Empresas desde API:: Finalizado")
      }
    });
  }

}

interface IInvoiceData
{
  invoice: IInvoice,
  enterprise: IEnterprise,
  invoiceLines: IInvoiceLine[],
  total: number
}

@Component({
  selector: 'home-invoice-table',
  templateUrl: 'home-invoice-table.html',
})
export class HomeInvoiceTable {
  constructor(@Inject(MAT_DIALOG_DATA) public data: IInvoiceData) {}
}
