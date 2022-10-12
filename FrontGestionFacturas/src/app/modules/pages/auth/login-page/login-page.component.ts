import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IUserAuth } from 'src/app/models/interfaces/user-auth.interface';
import { IToken } from 'src/app/models/interfaces/user-token.interface';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {

  constructor(private _authService: AuthService,
              private _router: Router) { }

  ngOnInit(): void {
  }

  login(value: any)
  {
    let {email, password} = value;

    this._authService.login(email, password).subscribe({
      next:(response: IToken) =>
      {
        if(response.token){
          sessionStorage.setItem('token', response.token.token);
          sessionStorage.setItem('userName', response.token.userName);
          sessionStorage.setItem('userRol', response.token.userRol.toString());
          this._router.navigate(['home']);
        }
      },
      error:(error) => console.error('ERROR CALLING LOGIN API'),
      complete:() => console.log("LOGIN SERVICE CALL ENDED")
      });
  }

}
