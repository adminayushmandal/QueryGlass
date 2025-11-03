import { inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, tap } from 'rxjs';
import { AccessTokenResponse, LoginRequest, UserClient } from 'src/app/web-api-client';
import { ErrorService } from './error.service';

export type TokenWithIsExpired = Omit<AccessTokenResponse, 'expiresIn' | 'init' | 'fromJS' | 'toJSON'> & { isTokenExpired: boolean }

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private _userClient = inject(UserClient)
  private _errorService = inject(ErrorService)
  private _router = inject(Router)

  private _loginLoader = signal(false)

  public login(email: string, password: string) {
    this._loginLoader.set(true)
    const loginRequest = new LoginRequest()
    loginRequest.email = email
    loginRequest.password = password
    return this._userClient.postApiUserLogin(null, null, loginRequest)
      .pipe(
        tap(tokens => {
          localStorage.setItem('accessToken', tokens.accessToken)
          localStorage.setItem('refreshToken', tokens.refreshToken)
          const tokenExpiry = Date.now() + (tokens.expiresIn * 1000)
          localStorage.setItem('tokenExpiry', tokenExpiry.toString())
          localStorage.setItem('tokenType', tokens.tokenType)
          this._router.navigate(['/app'])
        }),
        finalize(() => this._loginLoader.set(false)),
        catchError(error => this._errorService.handleHttpError(error))
      )
  }
}
