import { Routes } from '@angular/router';

import { LoginComponent } from './views/login/login';
import { IsmsComponent } from './views/isms/isms';
import { RiskManagementComponent } from './views/risk-management/risk-management';
import { SoaComponent } from './views/soa/soa';
import { HomeComponent } from './views/home/home';

import { authGuard } from './auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'isms',
    component: IsmsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'risk-management',
    component: RiskManagementComponent,
    canActivate: [authGuard]
  },
  {
    path: 'soa',
    component: SoaComponent,
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];