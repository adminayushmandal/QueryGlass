import { inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, of, switchMap, tap } from 'rxjs';
import { AccessTokenResponse, LoginRequest, UserClient, UserDto } from 'src/app/web-api-client';
import { ErrorService } from './error.service';
import { BASE_URL } from 'src/main';
import { httpResource } from '@angular/common/http';

export type TokenWithIsExpired = Omit<AccessTokenResponse, 'expiresIn' | 'init' | 'fromJS' | 'toJSON'> & { isTokenExpired: boolean }

export type User = Omit<UserDto, 'init' | 'fromJS' | 'toJSON'>

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private _userClient = inject(UserClient)
  private _errorService = inject(ErrorService)
  private _router = inject(Router)

  private _loginLoader = signal(false)
  public loginLoader = this._loginLoader.asReadonly()

  private _user = signal<User | null>(null)
  public currentUser = this._user.asReadonly()

  public get authTokens(): TokenWithIsExpired | null {
    const authTokens = localStorage.getItem('authToken')
    if (!authTokens) return null;
    const tokens: AccessTokenResponse = JSON.parse(authTokens)
    const isTokenExpired = Date.now() >= tokens.expiresIn
    return { ...tokens, isTokenExpired }
  }

  public login(email: string, password: string) {
    this._loginLoader.set(true)
    const loginRequest = new LoginRequest()
    loginRequest.email = email
    loginRequest.password = password
    return this._userClient.postApiUserLogin(null, null, loginRequest)
      .pipe(
        tap(tokens => {
          const authToken: Omit<AccessTokenResponse, "init" | 'fromJS' | 'toJSON'> = ({ ...tokens, expiresIn: Date.now() + tokens.expiresIn * 1000 })
          localStorage.setItem('authToken', JSON.stringify(authToken))
          this._router.navigate(['/app'])
        }),
        finalize(() => this._loginLoader.set(false)),
        catchError(error => this._errorService.handleHttpError(error))
      )
  }

  logout() {
    localStorage.removeItem('authToken')
    this._router.navigate(['/identity/login'])
    this._user.set(null)
  }

  public fetchUser = () => this._userClient.me()
    .pipe(
      switchMap(userdto => {
        const user: User = ({ ...userdto })
        this._user.set(user)
        return of(user)
      }),
      catchError(error => this._errorService.handleHttpError(error)),
      catchError(error => this._errorService.handleAccessError(error)),
    )
}
