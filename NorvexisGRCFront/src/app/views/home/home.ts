import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject, HostListener } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { ServicesService } from '../../services/services';
import { FormsModule } from '@angular/forms';

// ================= TYPES =================
type CardStatus = 'green' | 'yellow' | 'red';

interface DashboardCard {
  title: string;
  value: string | number;
  subtitle: string;
  status: CardStatus;
}

interface MappedKpi {
  name: string;
  value: number;
  targetValue: number;
  categoryId: string;
  categoryName: string;
  responsibleId: string;
  responsibleName: string;
  createdAt?: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class HomeComponent implements OnInit {
  // ================= DEPENDENCIES =================
  private readonly api = inject(ServicesService);
  private readonly cdr = inject(ChangeDetectorRef);

  // ================= CONSTANTS =================
  private readonly securityIncidentsCategoryId = '6489570a-2829-4eab-924c-a2f6e6380579';
  private readonly cybersecurityManagerResponsibleId = '92a05614-21ad-4ce3-b206-737cba4d1e8b';
  private readonly securityIncidentsKpiName = 'Number of security incidents reported';
  private readonly openHighRisksKpiName = 'Open risks by criticality';
  private readonly riskManagementCategoryName = 'Risk Management';
  private readonly drTestsCategoryName = 'Asset & Continuity';
  private readonly drTestsKpiName = 'Disaster recovery tests completed vs planned';
  private readonly drTestsResponsibleName = 'GRC Manager';

  // ================= STATE =================
  cards: DashboardCard[] = [];
  loading = true;

  months = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];

  availableYears: number[] = [];
  selectedMonth = new Date().getMonth();
  selectedYear = new Date().getFullYear();
  showMonthFilter = false;
  readonly currentMonth = new Date().getMonth();
  readonly currentYear = new Date().getFullYear();

  monthlyKpiCount = 0;
  totalKpiDomains = 0;

  ngOnInit(): void {
    this.generateYears();
    this.loadDashboard();
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent): void {

    const target = event.target as HTMLElement;

    const clickedInsideFilter =
      target.closest('.filter-container');

    if (!clickedInsideFilter) {
      this.showMonthFilter = false;
    }
  }

  // ================= MONTHS =================
  get monthStart(): Date {
    return new Date(this.selectedYear, this.selectedMonth, 1);
  }

  get monthEnd(): Date {
    const today = new Date();
    const selectedMonthEnd = new Date(this.selectedYear, this.selectedMonth + 1, 0, 23, 59, 59, 999);

    return this.selectedYear === this.currentYear && this.selectedMonth === this.currentMonth
      ? today
      : selectedMonthEnd;
  }

  get monthRangeLabel(): string {
    const monthName = new Date(this.selectedYear, this.selectedMonth, 1).toLocaleDateString('en-US', {
      month: 'long',
    });

    return `${monthName} 1 - ${this.monthEnd.toLocaleDateString('en-US', {
      month: 'long',
      day: 'numeric',
    })}`;
  }

  toggleMonthFilter(): void {
    this.showMonthFilter = !this.showMonthFilter;
  }

  onMonthYearChange(): void {
    this.selectedMonth = Number(this.selectedMonth);
    this.selectedYear = Number(this.selectedYear);
    this.loadDashboard();
  }

  generateYears(): void {
    const currentYear = new Date().getFullYear();

    for (let year = 2000; year <= currentYear + 1; year++) {
      this.availableYears.push(year);
    }
  }

  previousMonth(): void {
    if (this.selectedMonth === 0) {
      this.selectedMonth = 11;
      this.selectedYear--;
    } else {
      this.selectedMonth--;
    }

    this.loadDashboard();
  }

  nextMonth(): void {
    if (this.selectedMonth === 11) {
      this.selectedMonth = 0;
      this.selectedYear++;
    } else {
      this.selectedMonth++;
    }

    this.loadDashboard();
  }

  goToCurrentMonth(): void {
    this.selectedMonth = this.currentMonth;
    this.selectedYear = this.currentYear;
    this.loadDashboard();
  }

  get selectedMonthLabel(): string {
    return new Date(this.selectedYear, this.selectedMonth, 1).toLocaleDateString('en-US', {
      month: 'long',
      year: 'numeric',
    });
  }

  private isWithinSelectedMonth(kpi: MappedKpi): boolean {
    const date = new Date(kpi.createdAt ?? '');
    return !isNaN(date.getTime()) && date >= this.monthStart && date <= this.monthEnd;
  }

  // ================= LOAD DATA =================
  loadDashboard(): void {
    this.loading = true;

    forkJoin({
      kpis: this.api.getKpis().pipe(catchError(() => of([]))),
      soas: this.api.getSoas().pipe(catchError(() => of([]))),
      riskIdentifications: this.api.getRiskIdentifications().pipe(catchError(() => of([]))),
      riskAssessments: this.api.getRiskAssessments().pipe(catchError(() => of([]))),
      riskTreatments: this.api.getRiskTreatment().pipe(catchError(() => of([]))),
    })
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (res: any) => this.buildDashboard(res),
        error: () => {
          this.cards = this.emptyCards();
        },
      });
  }

  // ================= DASHBOARD BUILD =================
  private buildDashboard(res: any): void {
    const allKpis = (res.kpis || []).map((kpi: any) => this.mapKpi(kpi));
    const kpis = allKpis.filter((kpi: MappedKpi) => this.isWithinSelectedMonth(kpi));
    const yearlyKpis = allKpis.filter((kpi: MappedKpi) => this.isWithinSelectedYear(kpi));
    const soas = res.soas || [];
    const risks = this.mergeRisks(
      res.riskIdentifications || [],
      res.riskAssessments || [],
      res.riskTreatments || []
    );

    this.monthlyKpiCount = kpis.length;
    this.totalKpiDomains = this.countKpiDomains(kpis);

    const calculations = this.calculateDashboardValues(kpis, yearlyKpis, soas, risks, res.riskIdentifications || []);
    this.cards = this.buildCards(calculations);
  }

  // ================= KPI MAPPING =================
  private getMappedKpisForSelectedMonth(kpis: any[]): MappedKpi[] {
    return kpis
      .map((kpi: any) => this.mapKpi(kpi))
      .filter((kpi: MappedKpi) => this.isWithinSelectedMonth(kpi));
  }

  private mapKpi(item: any): MappedKpi {
    return {
      name:
        item.kpI_Name ??
        item.KPI_Name ??
        item.kpi_Name ??
        item.kpiName ??
        item.KPIName ??
        item.kpIName ??
        item.name ??
        '',

      value: Number(item.value ?? item.Value ?? 0),
      targetValue: Number(item.targetValue ?? item.TargetValue ?? 0),

      categoryId: item.kpiCategoryId ?? item.KPICategoryId ?? item.categoryId ?? '',
      categoryName:
        item.categoryName ??
        item.CategoryName ??
        item.kpiCategoryName ??
        item.KPICategoryName ??
        item.category?.name ??
        item.Category?.Name ??
        item.category ??
        item.Category ??
        '',

      responsibleId: item.kpiResponsibleId ?? item.KPIResponsibleId ?? item.responsibleId ?? '',
      responsibleName:
        item.responsibleName ??
        item.ResponsibleName ??
        item.kpiResponsibleName ??
        item.KPIResponsibleName ??
        item.responsible?.name ??
        item.Responsible?.Name ??
        item.responsible ??
        item.Responsible ??
        '',

      createdAt: item.createdAt ?? item.CreatedAt ?? item.date ?? item.Date,
    };
  }
  private isWithinSelectedYear(kpi: MappedKpi): boolean {
    const date = new Date(kpi.createdAt ?? '');
    return !isNaN(date.getTime()) && date.getFullYear() === this.selectedYear;
  }

  // ================= CALCULATIONS =================
  private calculateDashboardValues(kpis: MappedKpi[], yearlyKpis: MappedKpi[], soas: any[], risks: any[], riskIdentifications: any[]) {
    const totalSoas = soas.length;
    const implementedSoas = soas.filter((soa: any) => this.isSoaImplemented(soa)).length;
    const soaPercentage = totalSoas ? Math.round((implementedSoas / totalSoas) * 100) : 0;

    return {
      incidents: this.sumSecurityIncidents(kpis),
      sla: this.calculateSla(kpis),
      mttr: this.averageKpiValue(kpis, ['mttr', 'resolution']),
      unauthorizedAccess: this.sumKpiValue(kpis, ['access']),
      totalSoas,
      implementedSoas,
      soaPercentage,
      openHighRisks: this.sumOpenHighRisks(kpis),
      closedRisks: this.countRisksByStatus(risks, 'closed'),
      acceptedRisks: riskIdentifications.length || 0,
      drTests: this.sumDrTests(yearlyKpis),
    };
  }

  private sumSecurityIncidents(kpis: MappedKpi[]): number {
    return kpis
      .filter(kpi =>
        this.norm(kpi.name) === this.norm(this.securityIncidentsKpiName) &&
        this.norm(kpi.categoryId) === this.norm(this.securityIncidentsCategoryId) &&
        this.norm(kpi.responsibleId) === this.norm(this.cybersecurityManagerResponsibleId)
      )
      .reduce((total, kpi) => total + kpi.value, 0);
  }

  private sumOpenHighRisks(kpis: MappedKpi[]): number {
    return kpis
      .filter(kpi =>
        this.norm(kpi.name) === this.norm(this.openHighRisksKpiName) &&
        this.isRiskManagementKpi(kpi) &&
        this.isCybersecurityManagerKpi(kpi)
      )
      .reduce((total, kpi) => total + kpi.value, 0);
  }

  private sumDrTests(kpis: MappedKpi[]): number {
    return kpis
      .filter(kpi =>
        this.norm(kpi.name).includes(this.norm(this.drTestsKpiName))
      )
      .reduce((total, kpi) => total + kpi.value, 0);
  }

  private calculateSla(kpis: MappedKpi[]): number {
    const slaKpis = kpis.filter(kpi => this.norm(kpi.name).includes('sla'));

    if (!slaKpis.length) {
      return 0;
    }

    return Math.round(
      slaKpis.reduce((total, kpi) => {
        if (!kpi.targetValue) {
          return total;
        }

        return total + (kpi.value / kpi.targetValue) * 100;
      }, 0) / slaKpis.length
    );
  }

  private sumKpiValue(kpis: MappedKpi[], nameParts: string[]): number {
    return kpis
      .filter(kpi => nameParts.some(part => this.norm(kpi.name).includes(part)))
      .reduce((total, kpi) => total + kpi.value, 0);
  }

  private averageKpiValue(kpis: MappedKpi[], nameParts: string[]): number {
    const matches = kpis.filter(kpi =>
      nameParts.some(part => this.norm(kpi.name).includes(part))
    );

    if (!matches.length) {
      return 0;
    }

    return Math.round(matches.reduce((total, kpi) => total + kpi.value, 0) / matches.length);
  }

  private countKpiDomains(kpis: MappedKpi[]): number {
    return new Set(kpis.map(kpi => kpi.categoryId).filter(Boolean)).size;
  }

  private countRisksByStatus(risks: any[], status: string): number {
    return risks.filter(
      (risk: any) => this.norm(risk.status) === this.norm(status)
    ).length;
  }

  private mergeRisks(identifications: any[], assessments: any[], treatments: any[]): any[] {
    return identifications.map(risk => {
      const riskId = risk.riskId;

      return {
        riskCategory: assessments.find(item => item.riskId === riskId)?.riskCategory,
        status: treatments.find(item => item.riskId === riskId)?.status,
        riskTreatmentOption: treatments.find(item => item.riskId === riskId)?.riskTreatmentOption,
      };
    });
  }

  // ================= CARDS =================
  private buildCards(values: any): DashboardCard[] {
    return [
      {
        title: 'Security Incidents',
        value: values.incidents,
        subtitle: 'MTD (target <15)',
        status: this.getIncidentStatus(values.incidents),
      },
      {
        title: 'SLA Compliance',
        value: `${values.sla}%`,
        subtitle: 'Target >90%',
        status: this.getSLAStatus(values.sla),
      },
      {
        title: 'MTTR',
        value: `${values.mttr}h`,
        subtitle: 'Target <72h',
        status: this.getInverseTargetStatus(values.mttr, 72),
      },
      {
        title: 'Unauthorized Access',
        value: values.unauthorizedAccess,
        subtitle: 'Target 0',
        status: this.getUnauthorizedAccesStatus(values.unauthorizedAccess),
      },
      {
        title: 'SoA Controls Implemented',
        value: `${values.soaPercentage}%`,
        subtitle: `${values.implementedSoas} of ${values.totalSoas}`,
        status: this.getSOAImplementedStatus(values.soaPercentage),
      },
      {
        title: 'Open Risks (High)',
        value: values.openHighRisks,
        subtitle: '',
        status: values.openHighRisks > 0 ? 'red' : 'green',
      },
      {
        title: 'Risks Closed',
        value: values.closedRisks,
        subtitle: '',
        status: 'green',
      },
      {
        title: 'Risks Accepted',
        value: values.acceptedRisks,
        subtitle: '',
        status: 'yellow',
      },
      {
        title: 'DR Tests',
        value: values.drTests,
        subtitle: '',
        status: this.getDRTestsStatus(values.drTests),
      },
    ];
  }

  private emptyCards(): DashboardCard[] {
    return [
      { title: 'Security Incidents', value: 0, subtitle: '', status: 'green' },
      { title: 'SLA Compliance', value: '0%', subtitle: '', status: 'red' },
      { title: 'MTTR', value: '0h', subtitle: '', status: 'green' },
      { title: 'Unauthorized Access', value: 0, subtitle: '', status: 'green' },
      { title: 'SoA Controls Implemented', value: '0%', subtitle: '', status: 'red' },
      { title: 'Open Risks (High)', value: 0, subtitle: '', status: 'green' },
      { title: 'Risks Closed', value: 0, subtitle: '', status: 'green' },
      { title: 'Risks Accepted', value: 0, subtitle: '', status: 'yellow' },
      { title: 'Audit Findings', value: 0, subtitle: '', status: 'green' },
      { title: 'DR Tests', value: 0, subtitle: '', status: 'red' },
    ];
  }

  // ================= CONDITIONS =================
  private getIncidentStatus(value: number): CardStatus {
    if (value <= 10) {
      return 'green';
    }

    if (value <= 20) {
      return 'yellow';
    }

    return 'red';
  }

  private getSLAStatus(value: number): CardStatus {
    if (value <= 49) {
      return 'red';
    }

    if (value < 90) {
      return 'yellow';
    }

    return 'green';
  }

  private getMTTRStatus(value: number): CardStatus {
    if (value >= 8) {
      return 'red';
    }

    if (value >= 2) {
      return 'yellow';
    }

    return 'green';
  }

  private getUnauthorizedAccesStatus(value: number): CardStatus {
    if (value >= 2) {
      return 'red';
    }

    if (value == 1) {
      return 'yellow';
    }

    return 'green';
  }

  private getSOAImplementedStatus(value: number): CardStatus {
    if (value <= 95) {
      return 'red';
    }

    if (value <= 99) {
      return 'yellow';
    }

    return 'green';
  }

  private getDRTestsStatus(value: number): CardStatus {
    if (value <= 2) {
      return 'red';
    }

    if (value <= 6) {
      return 'yellow';
    }

    return 'green';
  }

  private getInverseTargetStatus(value: number, target: number): CardStatus {
    if (value === 0) {
      return 'green';
    }

    return this.getMTTRStatus(100 - (value / target) * 100);
  }

  // ================= HELPERS =================
  private isRiskManagementKpi(kpi: MappedKpi): boolean {
    return (
      this.norm(kpi.categoryName) === this.norm(this.riskManagementCategoryName) ||
      this.norm(kpi.categoryName) === ''
    );
  }

  private isCybersecurityManagerKpi(kpi: MappedKpi): boolean {
    return (
      this.norm(kpi.responsibleId) === this.norm(this.cybersecurityManagerResponsibleId) ||
      this.norm(kpi.responsibleName) === this.norm('Cybersecurity Leader') ||
      this.norm(kpi.responsibleName) === ''
    );
  }

  private isSoaImplemented(soa: any): boolean {
    const value = soa.implemented ?? soa.Implemented ?? soa.isImplemented ?? soa.IsImplemented;

    if (typeof value === 'boolean') {
      return value;
    }

    return ['true', 'yes', 'implemented', '1'].includes(this.norm(value));
  }

  private norm(value: unknown): string {
    return String(value ?? '').toLowerCase().trim();
  }
}
