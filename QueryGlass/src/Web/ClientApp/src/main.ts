import {
  enableProdMode,
  APP_ID,
  importProvidersFrom,
  Provider,
  EnvironmentProviders,
  InjectionToken,
  provideZoneChangeDetection
} from '@angular/core';
import { providePrimeNG } from 'primeng/config';


import { environment } from './environments/environment';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { routes } from "./app/app.route";
import { queryGlassTheme } from './app/core/utils/queryGlassTheme';
import { MessageService } from 'primeng/api';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export const ALERT_DURATION = new InjectionToken<number>('alertDuration')
export const BASE_URL = new InjectionToken<string>('BASE_URL');

const providers: (Provider | EnvironmentProviders)[] = [
  { provide: BASE_URL, useFactory: getBaseUrl, deps: [] },
  { provide: ALERT_DURATION, useValue: 4200 }
];

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    ...providers,
    importProvidersFrom(BrowserModule, FormsModule),
    { provide: APP_ID, useValue: 'ng-cli-universal' },
    provideHttpClient(withInterceptors([authorizeInterceptor])),
    provideZoneChangeDetection(),
    provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: queryGlassTheme
      }
    }),
    MessageService
  ]
})
  .catch(err => console.log(err));
