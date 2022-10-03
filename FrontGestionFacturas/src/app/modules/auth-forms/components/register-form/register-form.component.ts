import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.scss']
})
export class RegisterFormComponent implements OnInit {

  registerForm: FormGroup = new FormGroup({});
  @Input() loginPage: string = '';
  @Output() onSubmit: EventEmitter<{}> = new EventEmitter<{}>();
  
  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      name: ['', Validators.compose([Validators.maxLength(30), Validators.required])],
      email : ['', Validators.compose([Validators.email, Validators.required])],
      password: ['', Validators.compose([Validators.maxLength(30), Validators.required])]
    });
  }

  get formName() {return this.registerForm.get('name')}
  get formEmail() {return this.registerForm.get('email')}
  get formPassword() {return this.registerForm.get('password')}

  submitForm(formDirective: FormGroupDirective)
  {
    //Se verifica que el formulario sea válido
    if(this.registerForm.valid)
    {
      //Se emiten valores del formulario
      this.onSubmit.emit(this.registerForm.value);
      
      //Se reinicia formulario
      formDirective.resetForm();
      this.registerForm.reset();
    }
  }

}
