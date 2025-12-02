import { Component } from '@angular/core'

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss'
})
export class AppHeaderComponent {
  logoPath = 'assets/img/logo.png'
}
