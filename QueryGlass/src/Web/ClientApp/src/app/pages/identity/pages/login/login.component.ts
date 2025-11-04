import { Component, inject, model } from '@angular/core';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from 'src/app/core/services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'queryGlass-login',
  imports: [FloatLabelModule, InputTextModule, ButtonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private _fb = inject(FormBuilder)
  private _userService = inject(UserService)
  
  protected loading = this._userService.loginLoader

  public loginForm = this._fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  })

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  public login() {
    const email = this.email.value
    const password = this.password.value

    this._userService.login(email, password)
      .subscribe({
        error: error => {
          if (!environment.production) {
            console.log("An error occured: ", error);
          }
        }
      })
  }
}
