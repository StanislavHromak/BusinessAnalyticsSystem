import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DataListComponent } from './components/data-list/data-list.component';
import { AnalysisReportComponent } from './components/analysis-report/analysis-report.component';
import { AddDataComponent } from './components/add-data/add-data.component';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'data', component: DataListComponent },
  { path: 'data/add', component: AddDataComponent },
  { path: 'data/edit/:id', component: AddDataComponent },
  { path: 'reports', component: AnalysisReportComponent }
];

