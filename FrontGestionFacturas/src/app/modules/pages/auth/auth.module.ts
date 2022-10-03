import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { RegisterPageComponent } from './register-page/register-page.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { MaterialModule } from '../../material/material.module';
import { AuthFormsModule } from '../../auth-forms/auth-forms.module';


@NgModule({
  declarations: [
    RegisterPageComponent,
    LoginPageComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    //Importación de formularios para las páginas
    AuthFormsModule,
    MaterialModule
  ]
})
export class AuthModule { }
