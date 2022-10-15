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
    // Import of modules for forms
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    // Import of modules to redirect
    RouterModule
  ],
  exports: [
    // Export component of forms
    LoginFormComponent,
    RegisterFormComponent
  ]
})
export class AuthFormsModule { }
