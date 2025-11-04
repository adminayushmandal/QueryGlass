import { Component, effect, inject, signal } from '@angular/core';
import { MenubarModule } from 'primeng/menubar'
import { MenuModule } from 'primeng/menu'
import { ButtonModule } from 'primeng/button';
import { MenuItem } from 'primeng/api';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'queryGlass-navbar',
  imports: [MenubarModule, MenuModule, ButtonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  private _userService = inject(UserService)

  private currentUser = this._userService.currentUser

  protected menuItems = signal<MenuItem[]>([
    {
      label: 'Context',
      items: [
        { label: 'Profile', routerLink: 'me', icon: 'pi pi-user' },
        { label: 'Settings', routerLink: 'settings', icon: 'pi pi-cog' },
        { label: 'Configuration', routerLink: 'configuration', icon: 'pi pi-sliders-h' },
        { separator: true },
        {
          label: "Logout", icon: 'pi pi-sign-out', command: () => {
            this._userService.logout()
          }
        }
      ]
    }
  ])

  constructor() {
    effect(() => {
      this.menuItems.update(prev => {
        if (prev.find(x => x.label === 'Context')) {
          return prev.map(item => ({ ...item, label: `Context (${this.currentUser()?.roles[0]})` }))
        }
        return prev
      })
    })
  }
}
