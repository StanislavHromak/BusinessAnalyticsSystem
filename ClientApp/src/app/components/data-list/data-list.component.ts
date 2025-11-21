import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AnalyticsService } from '../../services/analytics.service';
import { FinancialData } from '../../models/financial-data.model';

@Component({
  selector: 'app-data-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="data-list">
      <h2>Список даних</h2>
      
      <div *ngIf="loading" class="loading">Завантаження...</div>
      <div *ngIf="error" class="error">{{ error }}</div>
      
      <div *ngIf="!loading && !error" class="table-container">
        <div class="table-header">
          <h3>Список фінансових даних</h3>
          <a routerLink="/data/add" class="btn-add">+ Додати нові дані</a>
        </div>
        <table class="data-table">
          <thead>
            <tr>
              <th>Дата</th>
              <th>Дохід</th>
              <th>Витрати</th>
              <th>Прибуток</th>
              <th>ROI</th>
              <th>ROS</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of data">
              <td>{{ item.date | date:'dd.MM.yyyy' }}</td>
              <td>{{ item.revenue | number:'1.2-2' }} ₴</td>
              <td>{{ item.totalCosts | number:'1.2-2' }} ₴</td>
              <td>{{ item.profit | number:'1.2-2' }} ₴</td>
              <td>{{ item.roi | number:'1.2-2' }}%</td>
              <td>{{ item.ros | number:'1.2-2' }}%</td>
            </tr>
          </tbody>
        </table>
        
        <div *ngIf="data.length === 0" class="empty-state">
          Немає даних для відображення
        </div>
      </div>
    </div>
  `,
  styles: [`
    .data-list {
      padding: 2rem 0;
    }

    .table-container {
      overflow-x: auto;
      margin-top: 2rem;
    }

    .data-table {
      width: 100%;
      border-collapse: collapse;
      background: white;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .data-table th {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
    }

    .data-table td {
      padding: 1rem;
      border-bottom: 1px solid #eee;
    }

    .data-table tr:hover {
      background: #f9f9f9;
    }

    .empty-state {
      text-align: center;
      padding: 3rem;
      color: #999;
    }

    .table-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .btn-add {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 0.75rem 1.5rem;
      border-radius: 8px;
      text-decoration: none;
      transition: all 0.3s ease;
    }

    .btn-add:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
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
export class DataListComponent implements OnInit {
  data: FinancialData[] = [];
  loading = true;
  error: string | null = null;

  constructor(private analyticsService: AnalyticsService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.error = null;
    
    this.analyticsService.getAllData().subscribe({
      next: (data) => {
        this.data = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Помилка завантаження даних: ' + err.message;
        this.loading = false;
      }
    });
  }
}

