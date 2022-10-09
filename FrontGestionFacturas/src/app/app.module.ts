import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './components/nav/nav.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MaterialModule } from './modules/material/material.module';
import { AuthFormsModule } from './modules/auth-forms/auth-forms.module';
import { HttpClientModule } from '@angular/common/http';
import { InvoicePdfComponent } from './modules/pages/home/invoice-pdf/invoice-pdf.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    InvoicePdfComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    HttpClientModule,
    //Importación de componentes de formularios
    AuthFormsModule,
    //Importación a través de material de módulos
    MaterialModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
