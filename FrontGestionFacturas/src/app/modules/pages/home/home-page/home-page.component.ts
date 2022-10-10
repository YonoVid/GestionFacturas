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
  // Columns of the table to be shown
  displayedColumns: string[] = ['id', 'name',
                                'home-enterprise', 'createdDate',
                                'totalAmount', 'home-edit'];
  // Data source of the table
  dataSource: MatTableDataSource<IInvoiceTableData> = new MatTableDataSource<IInvoiceTableData>();

  // Stored data of the external API
  invoices: { [id: number]: IInvoice;} = [];
  invoicesData: IInvoiceTableData[] = [];
  enterprises: { [id: number]: IEnterprise;} = [];

  // Determines if the data is still loading
  loadingData: boolean = true;
  
  // Paginator
  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  
  constructor(private invoiceService: InvoiceService,
              public dialog: MatDialog,
              private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.getInvoiceData();
  }

  ngAfterViewInit() {
    // Asign paginator if founded to the data source
    if(this.paginator != null)
    {  
      this.dataSource.paginator = this.paginator;
    }
  }

  sortData(sort: Sort)
  {
    // Get data
    const data = this.invoicesData;
    if (!sort.active || sort.direction === '') {
      this.dataSource.data = data;
      return;
    }
    // Sort the data with the selected value
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
  /**
   * Get a int to indicate the order of two elements.
   * @param a First element to compare.
   * @param b Second element to compare.
   * @param isAsc Determines if order is ascending or descending.
   * @returns Return 1 if b is the element, else -1.
   */
  compare(a: number | string | Date, b: number | string |Date, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }
  /**
   * Determines the name of the selected enterprise.
   * @param id Id of the enterprise.
   * @returns Name of the enterprise if founded, else a empty string.
   */
  getEnterpriseName(id: number): string
  {
    if(id in this.enterprises)
    {
      return this.enterprises[id].name;
    }
    return "";
  }
  /**
   * Load all the data needed to show the content to the user.
   */
  getInvoiceData()
  {
    this.loadingData = true;
    this.enterprises = [];
    this.invoices = [];
    this.invoicesData = [];
    // Get data of the enterprises 
    this.invoiceService.getEnterprises().subscribe({
      next: (response: IEnterprise[]) => {
        // Map data to it's id
        response.forEach((enterprise) => this.enterprises[enterprise.id] = enterprise);

        //console.table(response);
        // Get data of invoices
        this.invoiceService.getInvoices().subscribe({
          next: (response: IInvoice[]) => {
            // Map data to it's id
            response.forEach((invoice) => this.invoices[invoice.id] = invoice);

            // Generate data to be shown in the main table
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
            //console.table(response);

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
  /**
   * Open a modal dialog with the data of the selected invoice.
   * @param invoiceId Id of the invoice data to open in a modal dialog.
   */
  openInvoiceTable(invoiceId: number) {
    let invoice = this.invoices[invoiceId];
    console.info(`${invoiceId} || ${invoice?.name}`);
    this.invoiceService.getInvoiceLines(invoice.id).subscribe({
      next: (response: IInvoiceLine[]) =>
      {
        // Calculate total of invoice
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
  /**
   * Open a modal dialog with the data of the generated pdf invoice.
   * @param invoiceId Id of the invoice data to generate a pdf
   * and open it in a modal dialog. 
   */
  openInvoicePdf(invoiceId: number)
  {
    this.invoiceService.getInvoicePdf(invoiceId).subscribe({
      next:(response) => 
      {
        let file = new Blob([response], { type: 'application/pdf' });     
        var fileURL = URL.createObjectURL(file);
        this.dialog.open(InvoicePdfComponent, {
          data:
          {
            pdf: fileURL
          }
        });
      },
      error: (error) => console.error(error),
      complete: () => console.info("PDF GENERATION ENDED")
    });
  }
}
