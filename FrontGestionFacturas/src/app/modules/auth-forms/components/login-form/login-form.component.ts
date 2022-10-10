import { Component, EventEmitter, OnInit, Input, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormGroupDirective } from '@angular/forms';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {
  // Form data
  loginForm: FormGroup = new FormGroup({});
  // Page to be redirected on register
  @Input() registerPage: string = '';
  // Event to emit on submit
  @Output() onSubmit: EventEmitter<{}> = new EventEmitter<{}>(); 

  constructor(private formBuilder: FormBuilder) { }
  
  ngOnInit(): void {
    // Init of form
    this.loginForm = this.formBuilder.group(
      {
        email : ['', Validators.compose([Validators.email, Validators.required])],
        password: ['', Validators.compose([Validators.maxLength(30), Validators.required])]
      }
    )
  }
  // Get the values of the form
  get formEmail() {return this.loginForm.get('email')}
  get formPassword() {return this.loginForm.get('password')}
  /**
   * Emits the data of the form with a event.
   * @param formDirective The reference of the form to be submited.
   */
  submitForm(formDirective: FormGroupDirective)
  {
    // Check if the form is valid
    if(this.loginForm.valid)
    {
      // Values of the form are emitted
      this.onSubmit.emit(this.loginForm.value);
      
      // Form is reseted
      formDirective.resetForm()
      this.loginForm.reset(); 
    }
  }
}
