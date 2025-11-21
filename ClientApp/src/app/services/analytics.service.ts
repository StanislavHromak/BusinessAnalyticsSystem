import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FinancialData, AnalysisReport } from '../models/financial-data.model';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private apiUrl = '/api/analytics';

  constructor(private http: HttpClient) { }

  getAllData(): Observable<FinancialData[]> {
    return this.http.get<FinancialData[]>(`${this.apiUrl}/data`);
  }

  getDataById(id: number): Observable<FinancialData> {
    return this.http.get<FinancialData>(`${this.apiUrl}/data/${id}`);
  }

  createData(data: Partial<FinancialData>): Observable<FinancialData> {
    return this.http.post<FinancialData>(`${this.apiUrl}/data`, data);
  }

  updateData(id: number, data: Partial<FinancialData>): Observable<FinancialData> {
    return this.http.put<FinancialData>(`${this.apiUrl}/data/${id}`, data);
  }

  deleteData(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/data/${id}`);
  }

  getAnalysisReport(period: string = 'Month'): Observable<AnalysisReport> {
    return this.http.get<AnalysisReport>(`${this.apiUrl}/report?period=${period}`);
  }

  getDashboardStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/dashboard`);
  }
}

