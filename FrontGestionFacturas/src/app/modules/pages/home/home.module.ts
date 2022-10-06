import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HomeRoutingModule } from './home-routing.module';
import { HomeInvoiceTable, HomePageComponent } from './home-page/home-page.component';
import { MaterialModule } from '../../material/material.module';


@NgModule({
  declarations: [
    HomePageComponent,
    HomeInvoiceTable
  ],
  imports: [
    CommonModule,
    HomeRoutingModule,
    MaterialModule
  ]
})
export class HomeModule { }
