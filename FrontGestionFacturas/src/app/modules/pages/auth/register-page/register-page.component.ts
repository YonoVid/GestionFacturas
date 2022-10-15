import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent implements OnInit {

  constructor(private _authService: AuthService,
              private _router: Router) { }

  ngOnInit(): void {
  }

  register(value: any)
  {
    let {name, email, password} = value;

    this._authService.register(name, email, password).subscribe({
      next:(response) =>
      {
        alert("Registro exitoso ¡Inicia sesión!");
        this._router.navigate(['auth/login']);
      },
      error:(error) => console.error('ERROR CALLING LOGIN API'),
      complete:() => console.log("LOGIN SERVICE CALL ENDED")
      });
  }

}
