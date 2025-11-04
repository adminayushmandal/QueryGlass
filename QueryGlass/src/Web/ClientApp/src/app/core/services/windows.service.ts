import { inject, Injectable, signal } from '@angular/core';
import { catchError, delay, finalize, of, switchMap } from 'rxjs';
import { WindowsClient, WindowsDto } from 'src/app/web-api-client';
import { ErrorService } from './error.service';

export type Windows = Omit<WindowsDto, 'init' | 'fromJS' | 'toJSON'>

@Injectable({
  providedIn: 'root'
})
export class WindowsService {
  private _windowsClient = inject(WindowsClient)
  private _errorService = inject(ErrorService)

  private _windowsLoader = signal(false)
  public windowsLoader = this._windowsLoader.asReadonly()

  private _windows = signal<Windows[]>([])
  public windows = this._windows.asReadonly()

  public fetchWindows() {
    this._windowsLoader.set(true)
    return this._windowsClient.getWindowsServers()
      .pipe(
        delay(1600),
        switchMap(lookup => {
          const windows = lookup.windows
          this._windows.set(({ ...windows }))
          return of(lookup)
        }),
        finalize(() => this._windowsLoader.set(false)),
        catchError(error => this._errorService.handleAccessError(error)),
        catchError(error => this._errorService.handleHttpError(error))
      )
  }
}
