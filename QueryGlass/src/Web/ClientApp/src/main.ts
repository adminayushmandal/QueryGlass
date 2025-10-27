import {
  enableProdMode,
  APP_ID,
  importProvidersFrom,
  Provider,
  EnvironmentProviders,
  provideZonelessChangeDetection, InjectionToken
} from '@angular/core';
import {providePrimeNG} from 'primeng/config';
import Aura from '@primeuix/themes/aura';


import {environment} from './environments/environment';
import {HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {AuthorizeInterceptor} from 'src/api-authorization/authorize.interceptor';
import {BrowserModule, bootstrapApplication} from '@angular/platform-browser';
import {FormsModule} from '@angular/forms';
import {provideRouter} from '@angular/router';
import {AppComponent} from './app/app.component';
import {provideAnimationsAsync} from "@angular/platform-browser/animations/async";
import {routes} from "./app/app.route";

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export const ALERT_DURATION = new InjectionToken<number>('alertDuration')

const providers: (Provider | EnvironmentProviders)[] = [
  {provide: 'BASE_URL', useFactory: getBaseUrl, deps: []},
  {provide: ALERT_DURATION, useValue: 4200}
];

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    ...providers,
    importProvidersFrom(BrowserModule, FormsModule),
    {provide: APP_ID, useValue: 'ng-cli-universal'},
    {provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true},
    provideHttpClient(withInterceptorsFromDi()),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: Aura
      }
    })
  ]
})
  .catch(err => console.log(err));
