import { Component, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MatSidenav } from '@angular/material/sidenav';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  token: string | null = null;
  isOpen: boolean = false;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  constructor(private breakpointObserver: BreakpointObserver,
              private router: Router) {}

  ngOnInit(): void
  {
  }
  /**
   * Toggle the drawer menú
   */
  toggleDrawer()
  {
    this.isOpen = !this.isOpen;
  }

  haveToken(): boolean
  {
    if(sessionStorage.getItem("token") === null)
    {
      return false;
    }
    return true;
  }

  logout()
  {
    this.isOpen = false;
    sessionStorage.clear();
    this.router.navigate(['/auth']);
  }

}
