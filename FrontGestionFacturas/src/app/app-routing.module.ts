import { NgModule } from '@angular/core';
import { PreloadAllModules, PreloadingStrategy, RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { NetworkAwarePreloadStrategy } from './routes/preloading-strategies/network-aware-preloading-strategy';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'home'
  },
  {
    path:'home',
    loadChildren: () => import('./modules/pages/home/home.module').then(m => m.HomeModule),
    canActivate: [ AuthGuard ]
  },
  {
    path:'auth',
    loadChildren: () => import('./modules/pages/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path:'**',
    loadChildren: () => import('./modules/pages/not-found/not-found.module').then(m => m.NotFoundModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes,
    {
      preloadingStrategy: NetworkAwarePreloadStrategy
    })],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
