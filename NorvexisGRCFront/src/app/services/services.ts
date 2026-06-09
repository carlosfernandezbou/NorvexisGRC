import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServicesService {
  private http = inject(HttpClient);

  private baseUrl = 'http://localhost:5000/api';

  // KPI
  getKpis(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/kpis`);
  }

  getKpiCategories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/kpi-categories`);
  }

  getKpiResponsibles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/kpi-responsibles`);
  }

  // Create KPIs
  createKpi(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/kpis`, payload);
  }

  generateKpisForMonth(year: number, month: number) {
    return firstValueFrom(
      this.http.post(`${this.baseUrl}/kpis/generate/${year}/${month}`, {})
    );
  }

  // Update KPIs
  updateKpi(id: string, payload: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/kpis/${encodeURIComponent(id)}`, payload);
  }

  // Delete KPIs
  deleteKpi(id: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/kpis/${encodeURIComponent(id)}`);
  }

  deleteMonthKpis(year: number, month: number) {
    return firstValueFrom(
      this.http.delete(
        `${this.baseUrl}/kpis/month/${year}/${month}`
      )
    );
  }

  // SOA
  getSoas() {
    return this.http.get<any>(`${this.baseUrl}/soas`);
  }

  // Create SOA
  createSoa(payload: any) {
    return this.http.post<any>(`${this.baseUrl}/soas`, payload);
  }

  // Update SOA
  updateSoa(id: string, payload: any) {
    return this.http.put<any>(`${this.baseUrl}/soas/${id}`, payload);
  }

  // Delete SOA
  deleteSoa(id: string) {
    return this.http.delete<any>(`${this.baseUrl}/soas/${id}`);
  }

  // Risk Management
  getRiskAssessments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/risk-assessments`);
  }

  getRiskTreatment(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/risk-treatments`);
  }

  getRiskIdentifications(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/risk-identifications`);
  }

  // Create Risk
  createRiskIdentification(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/risk-identifications`, payload);
  }

  createRiskAssessment(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/risk-assessments`, payload);
  }

  createRiskTreatment(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/risk-treatments`, payload);
  }

  // Update Risk
  updateRiskIdentification(documentId: string, payload: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/risk-identifications/${encodeURIComponent(documentId)}`, payload);
  }

  updateRiskAssessment(documentId: string, payload: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/risk-assessments/${encodeURIComponent(documentId)}`, payload);
  }

  updateRiskTreatment(documentId: string, payload: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/risk-treatments/${encodeURIComponent(documentId)}`, payload);
  }

  // Delete Risk
  deleteRiskIdentification(documentId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/risk-identifications/${encodeURIComponent(documentId)}`);
  }

  deleteRiskAssessment(documentId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/risk-assessments/${encodeURIComponent(documentId)}`);
  }

  deleteRiskTreatment(documentId: string): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/risk-treatments/${encodeURIComponent(documentId)}`);
  }
}
