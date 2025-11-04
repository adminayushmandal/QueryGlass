import { Component, computed, inject, OnInit } from '@angular/core';
import { WindowsService } from 'src/app/core/services/windows.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner'
import { TableModule } from 'primeng/table'
import { environment } from 'src/environments/environment';
import { ButtonDirective, ButtonLabel } from "primeng/button";
import { RouterLink } from "@angular/router";

@Component({
  selector: 'queryGlass-home',
  imports: [ProgressSpinnerModule, TableModule, ButtonDirective, ButtonLabel, RouterLink],
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  private _windowsService = inject(WindowsService)

  protected windows = this._windowsService.windows

  protected isLoading = this._windowsService.windowsLoader

  protected headers = computed(() => {
    const windows = this.windows()
    const record: Record<keyof typeof windows[0], string> = {
      id: 'Server id',
      serverName: 'Server Name',
      os: 'Operating System',
      created: 'Created On',
    }
    return Object.entries(record).map(([key, value]) => ({ field: key, header: value }))
  })

  constructor() { }
  ngOnInit(): void {
    this._windowsService.fetchWindows().subscribe(
      {
        error: error => {
          if (!environment.production) {
            console.log("An error occured during fetching servers");
          }
        }
      }
    )
  }
}
