import { Component, OnInit } from '@angular/core';
import { IUser } from 'src/app/models/interfaces/user.interface';
import { InvoiceService } from 'src/app/services/invoice.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {

  users: IUser[] = [];
  constructor(private invoiceService: InvoiceService) { }

  ngOnInit(): void {
    this.invoiceService.getInvoices().subscribe({
      next: (response) => {
        this.users = response;
        console.table(response);
      },
      error: (error) => console.log(error),
      complete: () => console.log("Obtener Facturas desde API:: Finalizado")
    });
  }

}
