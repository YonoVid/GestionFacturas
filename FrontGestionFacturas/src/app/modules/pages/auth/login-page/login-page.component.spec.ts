import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { Router, Routes } from '@angular/router';
import { Location } from '@angular/common';

import { LoginPageComponent } from './login-page.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('LoginPageComponent', () => {
  let store: { [id: string]: any;}
  let routerSpy = {navigate: jasmine.createSpy('navigate')};
  let httpMock: HttpTestingController;

  let component: LoginPageComponent;
  let fixture: ComponentFixture<LoginPageComponent>;


  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        // Import of modules for http
        RouterTestingModule.withRoutes(routes),
        HttpClientTestingModule
      ],
      declarations: [ LoginPageComponent ],
      providers: [ {provide: Router, useValue: routerSpy} ]
    })
    .compileComponents();

    
    store = {};
    const mockLocalStorage = {
      getItem: (key: string): string => {
        return key in store ? store[key] : null;
      },
      setItem: (key: string, value: string) => {
        store[key] = `${value}`;
      },
      removeItem: (key: string) => {
        delete store[key];
      },
      clear: () => {
        store = {};
      }
    };
    spyOn(sessionStorage, 'getItem')
    .and.callFake(mockLocalStorage.getItem);
    spyOn(sessionStorage, 'setItem')
    .and.callFake(mockLocalStorage.setItem);
    spyOn(sessionStorage, 'removeItem')
    .and.callFake(mockLocalStorage.removeItem);
    spyOn(sessionStorage, 'clear')
    .and.callFake(mockLocalStorage.clear);
    
    httpMock = TestBed.inject(HttpTestingController)
    
    fixture = TestBed.createComponent(LoginPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call http service and store token', waitForAsync(() => {
    const mockToken = {token:
    {
      'token':'keykeykeykey',
      'username': 'example',
      'userRol': 0
    }}

    expect(sessionStorage.getItem('token')).toBeNull();

    component.login({email: 'example@mail.com', password: 'pwd'});

    const httpRequets = httpMock.expectOne('/api/Users/Login');
    expect(httpRequets.cancelled).toBeFalsy();
    expect(httpRequets.request.responseType).toEqual('json');
    httpRequets.flush(mockToken);

    fixture.detectChanges();
    
    expect(sessionStorage.getItem('token')).not.toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['home']);
  }));

  it('should keep storage data unmodified', waitForAsync(() => {
    const mockToken = {token:
    {
      'token':'keykeykeykey',
      'username': 'example',
      'userRol': 0
    }}

    expect(sessionStorage.getItem('token')).toBeNull();

    component.login({email: 'example@mail.com', password: 'pwd'});

    const httpRequets = httpMock.expectOne('/api/Users/Login');
    expect(httpRequets.cancelled).toBeFalsy();
    expect(httpRequets.request.responseType).toEqual('json');
    httpRequets.error(new ProgressEvent('Error example'));

    fixture.detectChanges();
    
    expect(sessionStorage.getItem('token')).toBeNull();
  }));
});

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: 'home', component: LoginPageComponent},
  {path: 'auth', component: LoginPageComponent}
];