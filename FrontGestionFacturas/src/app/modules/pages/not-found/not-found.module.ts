import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NotFoundRoutingModule } from './not-found-routing.module';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { MaterialModule } from '../../material/material.module';


@NgModule({
  declarations: [
    NotFoundPageComponent
  ],
  imports: [
    CommonModule,
    NotFoundRoutingModule,
    MaterialModule
  ]
})
export class NotFoundModule { }
