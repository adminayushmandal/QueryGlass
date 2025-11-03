import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { throwError } from 'rxjs';
import { ProblemDetails, SwaggerException } from 'src/app/web-api-client';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  private _messageService = inject(MessageService)

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
}
