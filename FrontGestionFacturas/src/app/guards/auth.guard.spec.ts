import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, Routes } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { NavComponent } from '../components/nav/nav.component';

import { Location } from '@angular/common';
import { AuthGuard } from './auth.guard';

describe('AuthGuard', () => {
  let location: Location;
  let router: Router;
  let store: { [id: string]: any;}
  let guard: AuthGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes(routes),
      ]
    });
    guard = TestBed.inject(AuthGuard);

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

    router = TestBed.inject(Router);
    location = TestBed.inject(Location);

    router.initialNavigation();
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });

  it('should return true', () => {
    store = {};
    store['token'] = 'mockToken';

    expect(guard.canActivate(new ActivatedRouteSnapshot(), router.routerState.snapshot)).toBeTrue();
  });

  it('should return false', () => {
    store = {};

    expect(guard.canActivate(new ActivatedRouteSnapshot(), router.routerState.snapshot)).toBeFalse();
  });
});

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: 'home', component: NavComponent},
  {path: 'auth/login', component: NavComponent}
];