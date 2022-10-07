import { Component, Inject, OnInit } from '@angular/core';
import { Sort } from '@angular/material/sort';
import { IEnterprise } from 'src/app/models/interfaces/enterprise.interface';
import { IInvoice } from 'src/app/models/interfaces/invoice.interface';
import { IInvoiceLine } from 'src/app/models/interfaces/invoiceLine.interface';
import { InvoiceService } from 'src/app/services/invoice.service';

import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { IFilter, ITableFilter } from 'src/app/models/table/table-filter';
import { InvoiceTableComponent } from '../invoice-table/invoice-table.component';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {

  displayedColumns: string[] = ['id', 'name',
                                'home-enterprise', 'createdDate',
                                'totalAmount', 'home-edit'];
  dataSource: MatTableDataSource<IInvoice> = new MatTableDataSource<IInvoice>();

  invoices: { [id: number]: IInvoice;} = [];
  enterprises: { [id: number]: IEnterprise;} = [];


  filters = new Map<string, string>();

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

  sortData(sort: Sort)
  {
    const data = Object.values(this.invoices);
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
          return this.compare(this.getEnterpriseName(a.enterpriseId),
                              this.getEnterpriseName(b.enterpriseId),
                              isAsc);
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

  applyFilter(ob:MatSelectChange, filter: IFilter) {

    this.filters.set(filter.name,ob.value);


    var jsonString = JSON.stringify(Array.from(this.filters.entries()));
    
    this.dataSource.filter = jsonString;
    //console.log(this.filterValues);
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
