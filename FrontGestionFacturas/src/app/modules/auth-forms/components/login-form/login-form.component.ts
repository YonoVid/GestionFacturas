import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormGroupDirective } from '@angular/forms';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {

  loginForm: FormGroup = new FormGroup({});
  @Input() registerPage: string = '';
  @Output() onSubmit: EventEmitter<{}> = new EventEmitter<{}>(); 

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    //Inicialización de formulario
    this.loginForm = this.formBuilder.group(
      {
        email : ['', Validators.compose([Validators.email, Validators.required])],
        password: ['', Validators.compose([Validators.maxLength(30), Validators.required])]
      }
    )
  }

  get formEmail() {return this.loginForm.get('email')}
  get formPassword() {return this.loginForm.get('password')}

  submitForm(formDirective: FormGroupDirective)
  {
    //Se verifica que el formulario sea válido
    if(this.loginForm.valid)
    {
      //Se emiten valores del formulario
      this.onSubmit.emit(this.loginForm.value);
      
      //Se reinicia formulario
      formDirective.resetForm()
      this.loginForm.reset(); 

    }
  }

}
