import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { Sort } from '@angular/material/sort';
import { IEnterprise } from 'src/app/models/interfaces/enterprise.interface';
import { IInvoice } from 'src/app/models/interfaces/invoice.interface';
import { IInvoiceLine } from 'src/app/models/interfaces/invoiceLine.interface';
import { InvoiceService } from 'src/app/services/invoice.service';

import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { IFilter, ITableFilter } from 'src/app/models/table/table-filter';
import { InvoiceTableComponent } from '../invoice-table/invoice-table.component';
import { MatPaginator } from '@angular/material/paginator';
import { IInvoiceTableData } from 'src/app/models/interfaces/invoice-table-data.interface';
import { DatePipe } from '@angular/common';
import { InvoicePdfComponent } from '../invoice-pdf/invoice-pdf.component';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {

  displayedColumns: string[] = ['id', 'name',
                                'home-enterprise', 'createdDate',
                                'totalAmount', 'home-edit'];
  dataSource: MatTableDataSource<IInvoiceTableData> = new MatTableDataSource<IInvoiceTableData>();

  invoices: { [id: number]: IInvoice;} = [];
  invoicesData: IInvoiceTableData[] = [];
  enterprises: { [id: number]: IEnterprise;} = [];

  loadingData: boolean = true;
  
  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  
  constructor(private invoiceService: InvoiceService,
              public dialog: MatDialog,
              private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.getInvoiceData();
  }

  ngAfterViewInit() {
    if(this.paginator != null)
    {  
      this.dataSource.paginator = this.paginator;
    }
  }

  
  sortData(sort: Sort)
  {
    const data = this.invoicesData;
    if (!sort.active || sort.direction === '') {
      this.dataSource.data = data;
      return;
    }

    this.dataSource.data = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'id':
          return this.compare(a.id, b.id, isAsc);
        case 'name':
          return this.compare(a.name, b.name, isAsc);
        case 'enterpriseId':
          return this.compare(a.enterprise, b.enterprise, isAsc);
        case 'totalAmount':
          return this.compare(a.totalAmount, b.totalAmount, isAsc);
        default:
          return 0;
      }
    });
  }

  compare(a: number | string | Date, b: number | string |Date, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
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
    this.invoicesData = [];

    this.invoiceService.getEnterprises().subscribe({
      next: (response: IEnterprise[]) => {
        response.forEach((enterprise) => this.enterprises[enterprise.id] = enterprise);

        console.table(response);

        this.invoiceService.getInvoices().subscribe({
          next: (response: IInvoice[]) => {
            response.forEach((invoice) => this.invoices[invoice.id] = invoice);
            Object.values(this.invoices).forEach((invoice) =>
              this.invoicesData.push({
                id: invoice.id,
                name: invoice.name,
                taxPercentage: invoice.taxPercentage,
                totalAmount: invoice.totalAmount,
                enterprise: this.getEnterpriseName(invoice.enterpriseId),
                createdDate: this.datePipe.transform(invoice.createdDate)!
              })
             );
            console.table(response);

          },
          error: (error) => console.log(error),
          complete: () => 
          {
            this.dataSource.data = this.invoicesData;
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
  
  openInvoiceTable(invoiceId: number) {
    let invoice = this.invoices[invoiceId];
    console.info(`${invoiceId} || ${invoice?.name}`);
    this.invoiceService.getInvoiceLines(invoice.id).subscribe({
      next: (response: IInvoiceLine[]) =>
      {
        let total = 0;
        response.forEach((line) => total += line.quantity * line.itemValue);

        this.dialog.open(InvoiceTableComponent, {
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

  openInvoicePdf(id: number)
  {
    this.invoiceService.getInvoicePdf(id).subscribe({
      next:(response) => 
      {
        this.dialog.open(InvoicePdfComponent, {
          data:
          {
            pdf: response
          }
        });
      },
      error: (error) => console.error(error),
      complete: () => console.info("PDF GENERATION ENDED")
    });
  }

}
