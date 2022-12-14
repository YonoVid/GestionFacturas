import { Component, EventEmitter, OnInit, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.scss']
})
export class RegisterFormComponent implements OnInit {
  // Form data
  registerForm: FormGroup = new FormGroup({});
  // Form directive reference
  @ViewChild(FormGroupDirective) registerFormDirective!: FormGroupDirective;
  // Page to be redirected to login
  @Input() loginPage: string = '';
  // Event to emit on submit
  @Output() onSubmit: EventEmitter<{}> = new EventEmitter<{}>();
  
  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    // Init of form
    this.registerForm = this.formBuilder.group({
      name: ['', Validators.compose([Validators.maxLength(30), Validators.required])],
      email : ['', Validators.compose([Validators.email, Validators.required])],
      password: ['', Validators.compose([Validators.maxLength(30), Validators.required])]
    });
  }
  // Get the values of the form
  get formName() {return this.registerForm.get('name')}
  get formEmail() {return this.registerForm.get('email')}
  get formPassword() {return this.registerForm.get('password')}
  /**
   * Emits the data of the form with a event.
   * @param formDirective The reference of the form to be submited.
   */
  submitForm()
  {
    // Check if the form is valid
    if(this.registerForm.valid)
    {
      // Values of the form are emitted
      this.onSubmit.emit(this.registerForm.value);
      
      // Form is reseted
      this.registerFormDirective.resetForm();
      this.registerForm.reset();
    }
  }

}
