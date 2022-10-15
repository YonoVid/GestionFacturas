import { LayoutModule } from '@angular/cdk/layout';
import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { NavComponent } from './nav.component';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { Router, Routes } from '@angular/router';
import { Location, CommonModule } from '@angular/common';

describe('NavComponent', () => {
  let store: { [id: string]: any;}
  let routerSpy = {navigate: jasmine.createSpy('navigaye')};
  
  let component: NavComponent;
  let fixture: ComponentFixture<NavComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [NavComponent],
      imports: [
        CommonModule,
        NoopAnimationsModule,
        LayoutModule,
        MaterialModule
      ],
      providers: [ {provide: Router, useValue: routerSpy} ]
    }).compileComponents();
  }));

  beforeEach(() => {
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

    fixture = TestBed.createComponent(NavComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should compile', () => {
    expect(component).toBeTruthy();
  });

  it('should change if drawer is open', () => {
    let startValue = component.isOpen;
    component.toggleDrawer();

    expect(component.isOpen).not.toEqual(startValue);
  });

  it('should return true', () => {
    store = {};
    store['token'] = 'mockToken';

    expect(component.haveToken()).toBeTrue();
  });

  it('should clean session data and close drawer', () => {
    store = {};
    store['token'] = 'mockToken';
    
    expect(sessionStorage.getItem('token')).not.toBeNull();

    component.logout();

    expect(sessionStorage.getItem('token')).toBeNull();
    expect(component.isOpen).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth']);
  });
});
