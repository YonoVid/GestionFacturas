import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { RegisterFormComponent } from './components/register-form/register-form.component';
import { MaterialModule } from '../material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    LoginFormComponent,
    RegisterFormComponent
  ],
  imports: [
    CommonModule,
    //Importación de módulos para formularios
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    //Importación de módulo para reedirección de router
    RouterModule
  ],
  exports: [
    LoginFormComponent,
    RegisterFormComponent
  ]
})
export class AuthFormsModule { }
