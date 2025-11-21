import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnalyticsService } from '../../services/analytics.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-data',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="add-data">
      <h2>Додати нові дані</h2>
      
      <form (ngSubmit)="onSubmit()" #dataForm="ngForm">
        <div class="form-group">
          <label for="date">Дата:</label>
          <input 
            type="date" 
            id="date" 
            name="date" 
            [(ngModel)]="formData.date" 
            required
            class="form-control">
        </div>

        <div class="form-group">
          <label for="fixedCosts">Фіксовані витрати (₴):</label>
          <input 
            type="number" 
            id="fixedCosts" 
            name="fixedCosts" 
            [(ngModel)]="formData.fixedCosts" 
            required
            min="0"
            step="0.01"
            class="form-control">
        </div>

        <div class="form-group">
          <label for="variableCostPerUnit">Змінні витрати на одиницю (₴):</label>
          <input 
            type="number" 
            id="variableCostPerUnit" 
            name="variableCostPerUnit" 
            [(ngModel)]="formData.variableCostPerUnit" 
            required
            min="0"
            step="0.01"
            class="form-control">
        </div>

        <div class="form-group">
          <label for="pricePerUnit">Ціна за одиницю (₴):</label>
          <input 
            type="number" 
            id="pricePerUnit" 
            name="pricePerUnit" 
            [(ngModel)]="formData.pricePerUnit" 
            required
            min="0"
            step="0.01"
            class="form-control">
        </div>

        <div class="form-group">
          <label for="unitsSold">Одиниць продано:</label>
          <input 
            type="number" 
            id="unitsSold" 
            name="unitsSold" 
            [(ngModel)]="formData.unitsSold" 
            required
            min="0"
            class="form-control">
        </div>

        <div class="form-group">
          <label for="investment">Інвестиції (₴):</label>
          <input 
            type="number" 
            id="investment" 
            name="investment" 
            [(ngModel)]="formData.investment" 
            required
            min="0"
            step="0.01"
            class="form-control">
        </div>

        <div *ngIf="error" class="error">{{ error }}</div>
        <div *ngIf="success" class="success">{{ success }}</div>

        <div class="form-actions">
          <button 
            type="submit" 
            [disabled]="!dataForm.valid || loading" 
            class="btn btn-primary">
            {{ loading ? 'Збереження...' : 'Зберегти' }}
          </button>
          <button 
            type="button" 
            (click)="resetForm()" 
            class="btn btn-secondary">
            Очистити
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .add-data {
      padding: 2rem 0;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 600;
      color: #333;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 1rem;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      margin-top: 2rem;
    }

    .btn {
      padding: 0.75rem 2rem;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-size: 1rem;
      transition: all 0.3s ease;
    }

    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .btn-primary:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-secondary {
      background: #f0f0f0;
      color: #333;
    }

    .btn-secondary:hover {
      background: #e0e0e0;
    }

    .error {
      background: #fee;
      color: #c33;
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 1rem;
    }

    .success {
      background: #efe;
      color: #3c3;
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 1rem;
    }
  `]
})
export class AddDataComponent implements OnInit {
  formData = {
    date: new Date().toISOString().split('T')[0],
    fixedCosts: 0,
    variableCostPerUnit: 0,
    pricePerUnit: 0,
    unitsSold: 0,
    investment: 0
  };

  loading = false;
  error: string | null = null;
  success: string | null = null;

  constructor(
    private analyticsService: AnalyticsService,
    private router: Router
  ) {}

  ngOnInit() {
    // Компонент готовий
  }

  onSubmit() {
    this.loading = true;
    this.error = null;
    this.success = null;

    this.analyticsService.createData(this.formData).subscribe({
      next: (data) => {
        this.success = 'Дані успішно додані!';
        this.loading = false;
        
        // Очистити форму через 2 секунди та перенаправити
        setTimeout(() => {
          this.router.navigate(['/data']);
        }, 2000);
      },
      error: (err) => {
        this.error = 'Помилка збереження: ' + (err.error?.message || err.message || 'Невідома помилка');
        this.loading = false;
      }
    });
  }

  resetForm() {
    this.formData = {
      date: new Date().toISOString().split('T')[0],
      fixedCosts: 0,
      variableCostPerUnit: 0,
      pricePerUnit: 0,
      unitsSold: 0,
      investment: 0
    };
    this.error = null;
    this.success = null;
  }
}

