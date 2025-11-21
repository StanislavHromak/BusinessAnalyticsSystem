import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnalyticsService } from '../../services/analytics.service';
import { AnalysisReport } from '../../models/financial-data.model';

@Component({
  selector: 'app-analysis-report',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="analysis-report">
      <h2>Аналітичний звіт</h2>
      
      <div class="period-selector">
        <button 
          *ngFor="let period of periods" 
          [class.active]="selectedPeriod === period"
          (click)="selectPeriod(period)"
          class="btn-period">
          {{ periodLabels[period] }}
        </button>
      </div>
      
      <div *ngIf="loading" class="loading">Завантаження...</div>
      <div *ngIf="error" class="error">{{ error }}</div>
      
      <div *ngIf="report && !loading" class="report-content">
        <div class="report-table">
          <table>
            <thead>
              <tr>
                <th>Період</th>
                <th>Дохід</th>
                <th>Витрати</th>
                <th>Прибуток</th>
                <th>ROI</th>
                <th>ROS</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let label of report.labels; let i = index">
                <td>{{ label }}</td>
                <td>{{ report.revenues[i] | number:'1.2-2' }} ₴</td>
                <td>{{ report.totalCosts[i] | number:'1.2-2' }} ₴</td>
                <td>{{ report.profits[i] | number:'1.2-2' }} ₴</td>
                <td>{{ report.rois[i] | number:'1.2-2' }}%</td>
                <td>{{ report.ross[i] | number:'1.2-2' }}%</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .analysis-report {
      padding: 2rem 0;
    }

    .period-selector {
      display: flex;
      gap: 1rem;
      margin: 2rem 0;
    }

    .btn-period {
      padding: 0.75rem 1.5rem;
      border: 2px solid #667eea;
      background: white;
      color: #667eea;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .btn-period:hover {
      background: #f0f0f0;
    }

    .btn-period.active {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-color: transparent;
    }

    .report-content {
      margin-top: 2rem;
    }

    .report-table table {
      width: 100%;
      border-collapse: collapse;
      background: white;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .report-table th {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
    }

    .report-table td {
      padding: 1rem;
      border-bottom: 1px solid #eee;
    }

    .report-table tr:hover {
      background: #f9f9f9;
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
export class AnalysisReportComponent implements OnInit {
  report: AnalysisReport | null = null;
  loading = true;
  error: string | null = null;
  selectedPeriod = 'Month';
  
  periods = ['Month', 'Quarter', 'Year'];
  periodLabels: { [key: string]: string } = {
    'Month': 'Місяць',
    'Quarter': 'Квартал',
    'Year': 'Рік'
  };

  constructor(private analyticsService: AnalyticsService) {}

  ngOnInit() {
    this.loadReport();
  }

  selectPeriod(period: string) {
    this.selectedPeriod = period;
    this.loadReport();
  }

  loadReport() {
    this.loading = true;
    this.error = null;
    
    this.analyticsService.getAnalysisReport(this.selectedPeriod).subscribe({
      next: (data) => {
        this.report = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Помилка завантаження звіту: ' + err.message;
        this.loading = false;
      }
    });
  }
}

