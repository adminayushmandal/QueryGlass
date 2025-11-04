import { inject, Inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse, HttpInterceptorFn } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { UserService } from 'src/app/core/services/user.service';
import { ErrorService } from 'src/app/core/services/error.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeInterceptor implements HttpInterceptor {
  loginUrl: string;

  constructor(@Inject('BASE_URL') baseUrl: string) {
    this.loginUrl = `${baseUrl}Identity/Account/Login`;
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.url?.startsWith(this.loginUrl)) {
          window.location.href = `${this.loginUrl}?ReturnUrl=${window.location.pathname}`;
        }
        return throwError(() => error);
      }),
      // HACK: As of .NET 8 preview 5, some non-error responses still need to be redirected to login page.
      map((event: HttpEvent<any>) => {
        2
        if (event instanceof HttpResponse && event.url?.startsWith(this.loginUrl)) {
          window.location.href = `${this.loginUrl}?ReturnUrl=${window.location.pathname}`;
        }
        return event;
      }));
  }
}

export const authorizeInterceptor: HttpInterceptorFn = (req, next) => {
  const userService = inject(UserService)
  const errorService = inject(ErrorService)

  const tokens = userService.authTokens

  if (!tokens?.accessToken || tokens?.isTokenExpired) {
    return next(req).pipe(
      catchError(error => errorService.handleAccessError(error)),
      catchError(error => errorService.handleHttpError(error))
    )
  }

  const authReq = req.clone({
    setHeaders: {
      'Authorization': `Bearer ${tokens?.accessToken}`
    }
  })

  return next(authReq)
    .pipe(
      catchError(error => errorService.handleAccessError(error)),
      catchError(error => errorService.handleHttpError(error))
    )
}