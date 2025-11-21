import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar">
      <div class="nav-container">
        <h1 class="nav-title">Business Analytics System</h1>
        <ul class="nav-menu">
          <li><a routerLink="/dashboard" routerLinkActive="active">Панель управління</a></li>
          <li><a routerLink="/data" routerLinkActive="active">Дані</a></li>
          <li><a routerLink="/data/add" routerLinkActive="active">Додати дані</a></li>
          <li><a routerLink="/reports" routerLinkActive="active">Звіти</a></li>
        </ul>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 1rem 0;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    }

    .nav-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 2rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .nav-title {
      font-size: 1.5rem;
      margin: 0;
    }

    .nav-menu {
      display: flex;
      list-style: none;
      gap: 2rem;
      margin: 0;
    }

    .nav-menu a {
      color: white;
      text-decoration: none;
      padding: 0.5rem 1rem;
      border-radius: 6px;
      transition: background 0.3s ease;
    }

    .nav-menu a:hover,
    .nav-menu a.active {
      background: rgba(255, 255, 255, 0.2);
    }
  `]
})
export class NavbarComponent {
}

