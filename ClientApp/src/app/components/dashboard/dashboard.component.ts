import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnalyticsService } from '../../services/analytics.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h2>Панель управління</h2>
      
      <div *ngIf="loading" class="loading">Завантаження...</div>
      <div *ngIf="error" class="error">{{ error }}</div>
      
      <div *ngIf="stats" class="stats-grid">
        <div class="stat-card">
          <h3>Загальний дохід</h3>
          <div class="stat-value">{{ stats.totalRevenue | number:'1.2-2' }} ₴</div>
        </div>
        <div class="stat-card">
          <h3>Загальні витрати</h3>
          <div class="stat-value">{{ stats.totalExpenses | number:'1.2-2' }} ₴</div>
        </div>
        <div class="stat-card">
          <h3>Прибуток</h3>
          <div class="stat-value">{{ stats.profit | number:'1.2-2' }} ₴</div>
        </div>
        <div class="stat-card">
          <h3>Записів</h3>
          <div class="stat-value">{{ stats.totalRecords }}</div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard {
      padding: 2rem 0;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-top: 2rem;
    }

    .stat-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }

    .stat-card h3 {
      font-size: 0.9rem;
      opacity: 0.9;
      margin-bottom: 0.5rem;
    }

    .stat-value {
      font-size: 2rem;
      font-weight: bold;
    }

    .loading, .error {
      text-align: center;
      padding: 2rem;
    }

    .error {
      color: #c33;
      background: #fee;
      border-radius: 8px;
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats: any = null;
  loading = true;
  error: string | null = null;

  constructor(private analyticsService: AnalyticsService) {}

  ngOnInit() {
    this.loadDashboardStats();
  }

  loadDashboardStats() {
    this.loading = true;
    this.error = null;
    
    this.analyticsService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Помилка завантаження даних: ' + err.message;
        this.loading = false;
      }
    });
  }
}

