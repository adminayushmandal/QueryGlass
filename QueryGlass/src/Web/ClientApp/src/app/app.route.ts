import { Routes } from "@angular/router";
import { CounterComponent } from "./counter/counter.component";
import { userResolver } from "./core/utils/routeResolver";

export const routes: Routes = [
  { path: 'counter', component: CounterComponent },
  { path: '', redirectTo: 'app', pathMatch: 'full' },
  {
    resolve: {
      currentUser: userResolver
    },
    path: 'app', loadComponent: () => import('./core/ui/sidebar/sidebar.component').then(c => c.SidebarComponent),
    children: [
      { path: '', loadComponent: () => import('./pages/home/home.component').then(c => c.HomeComponent) },
    ]
  },
  {
    path: 'identity',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./pages/identity/pages/login/login.component').then(c => c.LoginComponent)
      },
    ]
  },
  { path: '**', redirectTo: 'app' }
]
