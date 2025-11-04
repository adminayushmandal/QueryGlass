import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { throwError } from 'rxjs';
import { ProblemDetails, SwaggerException } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  private _messageService = inject(MessageService)
  private _router = inject(Router)

  public handleHttpError(error: HttpErrorResponse | SwaggerException) {
    const severity = error.status === 400 || error.status === 404 ? 'warn' : 'error';
    if (error instanceof HttpErrorResponse && (error.status === 400 || error.status === 404)) {
      const errorMessage = error.error?.message || 'An error occurred.';

      this._messageService.add({ summary: "An error occured", detail: errorMessage, severity: severity, life: 4500 });
    }

    if (error instanceof SwaggerException && (error.status === 400 || error.status === 404)) {
      const problemDetails: ProblemDetails = JSON.parse(error.response);
      this._messageService.add({ severity, summary: problemDetails.title, detail: problemDetails.detail, life: 4500 });
    }

    return throwError(() => error);
  }

  public handleAccessError(error: HttpErrorResponse | ProblemDetails) {
    if (error instanceof ProblemDetails && error.status === 401) {
      this._messageService.add({
        summary: error.title,
        detail: error.detail,
        severity: 'error',
        life: 4500
      })

      this._router.navigate(['/identity', 'login'], { queryParams: { ReturnUrl: window.location.pathname } })
    } else if (error instanceof ProblemDetails && error.status === 403) {
      this._messageService.add({
        summary: error.title,
        detail: error.detail,
        severity: 'info',
        life: 4500
      })

      this._router.navigate(['/forbidden'], { queryParams: { ReturnUrl: window.location.pathname } })
    }

    if (error.status === 401) {
      this._messageService.add({
        summary: "Unauthorized",
        detail: "Login session has expired. Please login again.",
        severity: 'error',
        life: 4500
      })

      this._router.navigate(['/identity', 'login'], { queryParams: { ReturnUrl: window.location.pathname } })
    }

    return throwError(() => error)
  }
}
