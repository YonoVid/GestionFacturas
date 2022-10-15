import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { Router } from '@angular/router';

import { RegisterPageComponent } from './register-page.component';

describe('RegisterPageComponent', () => {
  let store: { [id: string]: any;}
  const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
  let httpMock: HttpTestingController;

  let component: RegisterPageComponent;
  let fixture: ComponentFixture<RegisterPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        // Import of modules for http
        HttpClientTestingModule
      ],
      declarations: [ RegisterPageComponent ],
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

    httpMock = TestBed.inject(HttpTestingController);

    fixture = TestBed.createComponent(RegisterPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('register', ()=>{
    beforeEach(()=>{
      component.register({name:'example', email: 'example@mail.com', password: 'pwd'});
    });

    afterEach(() => {
      routerSpy.navigate.calls.reset();
    });
    
    it('should call http service and navigate to "auth/login"', waitForAsync(() => {
      const mockToken = {token:
      {
        'id': 0
      }}
      
      const httpRequest = httpMock.expectOne('/api/Users/Register');
      expect(httpRequest.cancelled).toBeFalsy();
      expect(httpRequest.request.responseType).toEqual('json');
      httpRequest.flush(mockToken);
      
      fixture.detectChanges();
      
      expect(routerSpy.navigate).toHaveBeenCalledWith(['auth/login']);
    }));
  
    it('should not call navigate', waitForAsync(() => {
      const httpRequest = httpMock.expectOne('/api/Users/Register');
      expect(httpRequest.cancelled).toBeFalsy();
      expect(httpRequest.request.responseType).toEqual('json');
      
      httpRequest.error(new ProgressEvent('error'));
      
      fixture.detectChanges();
      
      expect(routerSpy.navigate).not.toHaveBeenCalledOnceWith(['auth/login']);
    }));
  })

});
