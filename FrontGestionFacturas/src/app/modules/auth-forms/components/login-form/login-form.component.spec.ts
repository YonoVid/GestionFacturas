import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormGroupDirective, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { MaterialModule } from 'src/app/modules/material/material.module';

import { LoginFormComponent } from './login-form.component';

describe('(1) TESTING LoginFormComponent', () => {
  let component: LoginFormComponent;
  let fixture: ComponentFixture<LoginFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        // Import of modules for forms
        FormsModule,
        ReactiveFormsModule,
        MaterialModule,
        // Import of modules to redirect
        RouterTestingModule,
        BrowserAnimationsModule,
        RouterModule
      ],
      declarations: [ LoginFormComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginFormComponent);
    component = fixture.componentInstance;
    component.ngOnInit();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get form values', () => {
    const form = component.loginForm;
    const email = form.controls['email'];
    const password = form.controls['password'];

    expect(component.formEmail).toEqual(email);
    expect(component.formPassword).toEqual(password);
  });

  it('should return a invalid form', () => {
    const form = component.loginForm;
    const email = form.controls['email'];

    email.setValue('example@mail.com');

    expect(form.invalid).toBeTrue();
  });

  it('should return a invalid form', () => {
    const form = component.loginForm;
    const password = form.controls['password'];

    password.setValue('pwd');

    expect(form.invalid).toBeTrue();
  });

  it('should return a valid form', () => {
    const form = component.loginForm;
    const email = form.controls['email'];
    const password = form.controls['password'];

    email.setValue('example@mail.com');
    password.setValue('pwd');

    expect(form.valid).toBeTrue();
  });

  it('should not emmit the form data', () => {
    const form = component.loginForm;
    const email = form.controls['email'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();

    email.setValue('example@mail.com');

    fixture.debugElement.query(By.css('form')).triggerEventHandler('submit', {});

    fixture.detectChanges();

    expect(instance.onSubmit.emit).not.toHaveBeenCalled();
  });

  it('should emmit the form data', () => {
    const form = component.loginForm;
    const email = form.controls['email'];
    const password = form.controls['password'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();

    email.setValue('example@mail.com');
    password.setValue('pwd');

    fixture.debugElement.query(By.css('form')).triggerEventHandler('submit', {});

    fixture.detectChanges();

    expect(instance.onSubmit.emit).toHaveBeenCalledWith({
      'email': 'example@mail.com',
      'password': 'pwd'
    });
  });

  it('should not send the form data', () => {
    const form = component.loginForm;
    const email = form.controls['email'];
    const password = form.controls['password'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();


    email.setValue('example@mail.com');

    component.submitForm();

    fixture.detectChanges();

    expect(form.value).toEqual({
      'email': 'example@mail.com',
      'password': ''
    });
    expect(form.valid).not.toBeTruthy();
  });

  it('should send and reset the form data', () => {
    const form = component.loginForm;
    const email = form.controls['email'];
    const password = form.controls['password'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();


    email.setValue('example@mail.com');
    password.setValue('pwd');

    component.submitForm();

    fixture.detectChanges();

    expect(form.value).toEqual({
      'email': null,
      'password': null
    });
  });
});
