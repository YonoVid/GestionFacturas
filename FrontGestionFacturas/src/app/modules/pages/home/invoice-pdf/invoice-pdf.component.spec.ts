import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SafePipe } from 'src/app/pipes/safe.pipe';

import { InvoicePdfComponent } from './invoice-pdf.component';

describe('InvoicePdfComponent', () => {
  let component: InvoicePdfComponent;
  let fixture: ComponentFixture<InvoicePdfComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatDialogModule
      ],
      declarations: [ 
        InvoicePdfComponent,
        SafePipe
      ],
      providers: [{
        provide: MatDialogRef,
        useValue: {updateSize: () => null } }
      ,{
        provide: MAT_DIALOG_DATA,
        useValue: {url: '', invoice: 0}
    }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoicePdfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
