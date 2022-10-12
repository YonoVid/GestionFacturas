import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { MaterialModule } from 'src/app/modules/material/material.module';

import { RegisterFormComponent } from './register-form.component';

describe('RegisterFormComponent', () => {
  let component: RegisterFormComponent;
  let fixture: ComponentFixture<RegisterFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        // Import of modules for forms
        FormsModule,
        ReactiveFormsModule,
        MaterialModule,
        // Import of modules to redirect
        RouterTestingModule,
        BrowserAnimationsModule,
        RouterModule
      ],
      declarations: [ RegisterFormComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return a invalid form', () => {
    const form = component.registerForm;
    const name = form.controls['name'];

    name.setValue('example');

    expect(form.invalid).toBeTrue();
  });

  it('should return a invalid form', () => {
    const form = component.registerForm;
    const email = form.controls['email'];

    email.setValue('example@mail.com');

    expect(form.invalid).toBeTrue();
  });

  it('should return a invalid form', () => {
    const form = component.registerForm;
    const password = form.controls['password'];

    password.setValue('pwd');

    expect(form.invalid).toBeTrue();
  });

  it('should return a valid form', () => {
    const form = component.registerForm;
    const name = form.controls['name'];
    const email = form.controls['email'];
    const password = form.controls['password'];

    name.setValue('example');
    email.setValue('example@mail.com');
    password.setValue('pwd');

    expect(form.valid).toBeTrue();
  });

  it('should emmit the form data', () => {
    const form = component.registerForm;
    const name = form.controls['name'];
    const email = form.controls['email'];
    const password = form.controls['password'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();

    name.setValue('example');
    email.setValue('example@mail.com');
    password.setValue('pwd');

    fixture.debugElement.query(By.css('form')).triggerEventHandler('submit', {});

    fixture.detectChanges();

    expect(instance.onSubmit.emit).toHaveBeenCalled();
    expect(instance.onSubmit.emit).toHaveBeenCalledWith({
      'name': 'example',
      'email': 'example@mail.com',
      'password': 'pwd'
    });
  });

  it('should not send the form data', () => {
    const form = component.registerForm;
    const email = form.controls['email'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();

    email.setValue('example@mail.com');

    component.submitForm();

    fixture.detectChanges();

    expect(form.value).toEqual({
      'name': '',
      'email': 'example@mail.com',
      'password': ''
    });
    expect(form.valid).not.toBeTruthy();
  });

  it('should send and reset the form data', () => {
    const form = component.registerForm;
    const name = form.controls['name'];
    const email = form.controls['email'];
    const password = form.controls['password'];

    const instance = fixture.componentInstance; 
    spyOn(instance.onSubmit, 'emit').and.callThrough();

    name.setValue('example');
    email.setValue('example@mail.com');
    password.setValue('pwd');

    component.submitForm();

    fixture.detectChanges();

    expect(form.value).toEqual({
      'name': null,
      'email': null,
      'password': null
    });
  });
});
