import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SplitterModule } from 'primeng/splitter';
import { NavbarComponent } from "../navbar/navbar.component";

@Component({
  selector: 'queryGlass-sidebar',
  imports: [RouterOutlet, SplitterModule, NavbarComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
}
