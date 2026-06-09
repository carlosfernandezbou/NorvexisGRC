import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicesService } from '../../services/services';
import { forkJoin } from 'rxjs';

import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-isms',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './isms.html',
  styleUrl: './isms.css'
})
export class IsmsComponent implements OnInit {
  private api = inject(ServicesService);
  private cdr = inject(ChangeDetectorRef);

  kpis: any[] = [];
  filteredKpis: any[] = [];

  categories: any[] = [];
  responsibles: any[] = [];

  selectedCategory = 'all';
  readonly fallbackKpiCategories = [
    'Security',
    'Continuity',
    'Compliance',
    'Risk'
  ];

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

  totalKpis = 0;
  complianceScore = 0;

  currentMonth = new Date();
  isFormOpen = false;
  isEditMode = false;
  isSaving = false;
  editingKpiId = '';

  formModel = {
    name: '',
    value: 0,
    targetValue: 0,
    kpiCategoryId: '',
    kpiResponsibleId: '',
    comments: ''
  };

  get isAdmin(): boolean {
    return localStorage.getItem('userRole') === 'Admin';
  }

  get currentMonthLabel(): string {
    return this.currentMonth.toLocaleDateString('en-US', {
      month: 'long',
      year: 'numeric'
    });
  }

  get currentMonthLabelWithQ(): string | undefined {
    const month = this.currentMonth.getMonth();

    if (month === 2) return 'Q1 Summary';
    if (month === 5) return 'Q2 Summary';
    if (month === 8) return 'Q3 Summary';
    if (month === 11) return 'Annual Summary';

    return undefined;
  }

  get formTitle(): string {
    return this.isEditMode ? 'Edit KPI' : 'Add KPI';
  }

  get formSubtitle(): string {
    return this.isEditMode
      ? 'Update the selected KPI and save the changes.'
      : 'Complete the fields below and create a new KPI.';
  }

  get categoryFilterOptions(): string[] {
    const categoriesFromDb = this.categories
      .map((category: any) => this.getCategoryLabel(category))
      .filter((category: string) => !!category);

    const categoriesFromKpis = this.kpis
      .map((kpi: any) => kpi.category)
      .filter((category: string) => !!category);

    const categories = categoriesFromDb.length
      ? categoriesFromDb
      : categoriesFromKpis.length
        ? categoriesFromKpis
        : this.fallbackKpiCategories;

    return Array.from(new Set(categories)).sort((a, b) => a.localeCompare(b));
  }

  get submitButtonLabel(): string {
    if (this.isSaving) {
      return this.isEditMode ? 'Saving...' : 'Creating...';
    }

    return this.isEditMode ? 'Save changes' : 'Create KPI';
  }

  ngOnInit(): void {
    this.generateYears();
    this.loadKpis();
  }

  loadKpis(): void {
    forkJoin({
      kpis: this.api.getKpis(),
      categories: this.api.getKpiCategories(),
      responsibles: this.api.getKpiResponsibles()
    }).subscribe({
      next: ({ kpis, categories, responsibles }) => {
        this.categories = categories || [];
        this.responsibles = responsibles || [];

        this.kpis = (kpis || []).map((item: any) => {
          const categoryId =
            item.kpiCategoryId ??
            item.KPICategoryId ??
            item.kpiCategoryID ??
            item.kpICategoryId ??
            item.kpiCategory?.id ??
            item.categoryId;

          const responsibleId =
            item.kpiResponsibleId ??
            item.KPIResponsibleId ??
            item.kpiResponsibleID ??
            item.kpIResponsibleId ??
            item.responsibleId;

          return {
            category: this.getCategoryName(categoryId),
            name:
              item.kpI_Name ??
              item.KPI_Name ??
              item.kpi_Name ??
              item.kpiName ??
              item.KPIName ??
              item.kpIName ??
              item.name ??
              '',
            responsible: this.getResponsibleName(responsibleId),
            value: item.value ?? item.Value ?? 0,
            targetValue: item.targetValue ?? item.TargetValue ?? null,
            comments: item.comments ?? item.Comments ?? '',
            createdAt: item.createdAt ?? item.CreatedAt,
            id: item.id ?? item.Id,
            kpiCategoryId: String(categoryId ?? ''),
            kpiResponsibleId: String(responsibleId ?? '')
          };
        });

        this.ensureFormSelections();

        setTimeout(() => {
          // this.goToCurrentMonth();
          this.applyMonthFilter();
          this.cdr.detectChanges();
        }, 0);
      },
      error: (err) => {
        console.error('Error loading KPIs', err);
        this.kpis = [];
        this.filteredKpis = [];
        this.categories = [];
        this.responsibles = [];
        this.totalKpis = 0;
        this.complianceScore = 0;
        this.cdr.detectChanges();
      }
    });
  }

  getCategoryName(id: string | number): string {
    const normalizedId = String(id);
    const category = this.categories.find((c: any) => String(c.id ?? c.Id) === normalizedId);

    return (
      category?.kpI_Category ??
      category?.kpi_Category ??
      category?.KPI_Category ??
      category?.name ??
      category?.title ??
      normalizedId
    );
  }

  getResponsibleName(id: string | number): string {
    const normalizedId = String(id);
    const responsible = this.responsibles.find((r: any) => String(r.id ?? r.Id) === normalizedId);

    return (
      responsible?.responsible ??
      responsible?.name ??
      responsible?.fullName ??
      responsible?.title ??
      normalizedId
    );
  }

  getCategoryId(category: any): string {
    return String(category?.id ?? category?.Id ?? '');
  }

  getCategoryLabel(category: any): string {
    return (
      category?.kpI_Category ??
      category?.kpi_Category ??
      category?.KPI_Category ??
      category?.name ??
      category?.title ??
      this.getCategoryId(category)
    );
  }

  getResponsibleId(responsible: any): string {
    return String(responsible?.id ?? responsible?.Id ?? '');
  }

  getResponsibleLabel(responsible: any): string {
    return (
      responsible?.responsible ??
      responsible?.name ??
      responsible?.fullName ??
      responsible?.title ??
      this.getResponsibleId(responsible)
    );
  }


  applyMonthFilter(): void {
    const month = this.currentMonth.getMonth();
    const year = this.currentMonth.getFullYear();

    this.filteredKpis = this.kpis.filter((kpi: any) => {
      if (!kpi.createdAt) return false;

      const date = new Date(kpi.createdAt);
      if (isNaN(date.getTime())) return false;

      const matchesMonth = date.getMonth() === month && date.getFullYear() === year;
      const matchesCategory =
        this.selectedCategory === 'all' || kpi.category === this.selectedCategory;

      return matchesMonth && matchesCategory;
    });

    // ORDEN ALFABÉTICO
    this.filteredKpis.sort((a, b) =>
      a.category.localeCompare(b.category)
    );

    this.totalKpis = this.filteredKpis.length;

    const validKpis = this.filteredKpis.filter((kpi: any) =>
      kpi.value !== null &&
      kpi.value !== undefined &&
      kpi.targetValue !== null &&
      kpi.targetValue !== undefined
    );

    const totalCompliance = validKpis.reduce((sum: number, kpi: any) => {
      const value = Number(kpi.value);
      const targetValue = Number(kpi.targetValue);

      let kpiCompliance = 0;

      // Si target = 0
      if (targetValue === 0) {
        kpiCompliance = value === 0 ? 100 : 0;
      } else {
        kpiCompliance = Math.min((value / targetValue) * 100, 100);
      }

      return sum + kpiCompliance;
    }, 0);

    this.complianceScore = validKpis.length
      ? Math.round(totalCompliance / validKpis.length)
      : 0;
  }

  onCategoryFilterChange(): void {
    this.applyMonthFilter();
    this.cdr.detectChanges();
  }

  generateYears(): void {
    const currentYear = new Date().getFullYear();

    for (let year = 2000; year <= currentYear + 1; year++) {
      this.availableYears.push(year);
    }
  }

  onMonthYearChange(): void {

    this.currentMonth = new Date(
      this.selectedYear,
      this.selectedMonth,
      1
    );

    this.applyMonthFilter();
    this.cdr.detectChanges();
  }

  previousMonth(): void {

    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() - 1,
      1
    );

    this.selectedMonth = this.currentMonth.getMonth();
    this.selectedYear = this.currentMonth.getFullYear();

    this.applyMonthFilter();
    this.cdr.detectChanges();
  }

  nextMonth(): void {

    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + 1,
      1
    );

    this.selectedMonth = this.currentMonth.getMonth();
    this.selectedYear = this.currentMonth.getFullYear();

    this.applyMonthFilter();
    this.cdr.detectChanges();
  }

  goToCurrentMonth(): void {

    const today = new Date();

    this.currentMonth = new Date(
      today.getFullYear(),
      today.getMonth(),
      1
    );

    this.selectedMonth = today.getMonth();
    this.selectedYear = today.getFullYear();

    this.applyMonthFilter();
    this.cdr.detectChanges();
  }

  toggleMonthFilter(): void {
    this.showMonthFilter = !this.showMonthFilter;
  }

  addKpi(): void {
    this.isEditMode = false;
    this.editingKpiId = '';
    this.resetForm();
    this.ensureFormSelections();
    this.isFormOpen = true;
    this.scrollToElement('kpi-create-form');
  }

  editKpi(kpi: any): void {
    this.isEditMode = true;
    this.editingKpiId = String(kpi.id ?? kpi.Id ?? '');
    this.formModel = {
      name:
        kpi.name ??
        kpi.kpI_Name ??
        kpi.KPI_Name ??
        kpi.kpi_Name ??
        kpi.kpiName ??
        kpi.KPIName ??
        kpi.kpIName ??
        '',
      value: Number(kpi.value ?? kpi.Value ?? 0),
      targetValue: Number(kpi.targetValue ?? kpi.TargetValue ?? 0),
      kpiCategoryId: String(kpi.kpiCategoryId ?? kpi.KPICategoryId ?? ''),
      kpiResponsibleId: String(kpi.kpiResponsibleId ?? kpi.KPIResponsibleId ?? ''),
      comments: kpi.comments ?? kpi.Comments ?? '',
    };
    this.ensureFormSelections();
    this.isFormOpen = true;
    this.scrollToElement(this.getEditFormElementId(this.editingKpiId));
  }

  isEditingRow(kpi: any): boolean {
    return this.isFormOpen && this.isEditMode && this.editingKpiId === String(kpi.id ?? kpi.Id ?? '');
  }

  cancelForm(): void {
    this.isFormOpen = false;
    this.isEditMode = false;
    this.isSaving = false;
    this.editingKpiId = '';
    this.resetForm();
  }

  submitKpiForm(): void {
    if (this.isSaving) return;

    const payload = this.buildPayload();
    if (!payload) return;

    this.isSaving = true;


    if (this.isEditMode) {
      if (!this.editingKpiId) {
        this.isSaving = false;
        window.alert('The selected KPI has no valid id.');
        return;
      }

      this.api.updateKpi(this.editingKpiId, payload).subscribe({
        next: () => {
          this.cancelForm();
          this.loadKpis();
        },
        error: (err) => {
          this.isSaving = false;
          console.error('Error updating KPI', err);
          window.alert(this.getApiErrorMessage(err, 'Could not update KPI.'));
        }
      });

      return;
    }

    this.api.createKpi(payload).subscribe({
      next: () => {
        this.cancelForm();
        this.loadKpis();
      },
      error: (err) => {
        this.isSaving = false;
        console.error('Error creating KPI', err);
        window.alert(this.getApiErrorMessage(err, 'Could not create KPI.'));
      }
    });
  }

  deleteKpi(kpi: any): void {
    const id = String(kpi.id ?? kpi.Id ?? '');
    if (!id) {
      window.alert('The selected KPI has no valid id.');
      return;
    }

    const confirmed = window.confirm(`Are you sure you want to delete "${kpi.name}"?`);
    if (!confirmed) return;

    this.api.deleteKpi(id).subscribe({
      next: () => this.loadKpis(),
      error: (err) => {
        console.error('Error deleting KPI', err);
        window.alert('Could not delete KPI. Check the API endpoint.');
      }
    });
  }

  getEditFormElementId(id: string): string {
    return `kpi-edit-form-${id}`;
  }

  private buildPayload(): any | null {
    const name = String(this.formModel.name ?? '').trim();
    const categoryId = String(this.formModel.kpiCategoryId ?? '').trim();
    const responsibleId = String(this.formModel.kpiResponsibleId ?? '').trim();
    const value = Number(this.formModel.value);
    const targetValue = Number(this.formModel.targetValue);
    const comments = String(this.formModel.comments ?? '').trim();

    if (!name) {
      window.alert('KPI name is required.');
      return null;
    }

    if (!categoryId) {
      window.alert('Category is required.');
      return null;
    }

    if (!responsibleId) {
      window.alert('Responsible is required.');
      return null;
    }

    if (Number.isNaN(value)) {
      window.alert('KPI value must be numeric.');
      return null;
    }
    if (value < 0 || value > 100) {
      window.alert('Value must be between 0 and 100.');
      return null;
    }

    if (Number.isNaN(targetValue)) {
      window.alert('Target value must be numeric.');
      return null;
    }

    if (targetValue < 0 || targetValue > 100) {
      window.alert('Target value must be between 0 and 100.');
      return null;
    }

    return {
      KPICategoryId: categoryId,
      KpiName: name,
      KPIResponsibleId: responsibleId,
      Value: value,
      TargetValue: targetValue,
      Comments: comments
    };
  }


  private getApiErrorMessage(err: any, fallback: string): string {
    const serverMessage =
      err?.error?.error ??
      err?.error?.title ??
      err?.error?.message ??
      err?.message;

    if (serverMessage) {
      return `${fallback} ${serverMessage}`;
    }

    return fallback;
  }

  private resetForm(): void {
    this.formModel = {
      name: '',
      value: 0,
      targetValue: 0,
      kpiCategoryId: '',
      kpiResponsibleId: '',
      comments: ''
    };
  }

  private ensureFormSelections(): void {
    if (!this.formModel.kpiCategoryId && this.categories.length) {
      this.formModel.kpiCategoryId = this.getCategoryId(this.categories[0]);
    }

    if (!this.formModel.kpiResponsibleId && this.responsibles.length) {
      this.formModel.kpiResponsibleId = this.getResponsibleId(this.responsibles[0]);
    }
  }

  private scrollToElement(elementId: string): void {
    setTimeout(() => {
      const element = document.getElementById(elementId);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }, 0);
  }

  exportToPDF(): void {

    // Asegurar que el filtro del mes está aplicado
    this.applyMonthFilter();

    const pdf = new jsPDF();

    const rows = this.filteredKpis.map(item => [
      item.category || '',
      item.name || '',
      item.responsible || '',
      item.value ?? '',
      item.targetValue ?? '',
      item.comments || ''
    ]);

    const logo = new Image();
    logo.src = '/logo.png';

    logo.onload = () => {

      const pageWidth = pdf.internal.pageSize.getWidth();

      // ===== LOGO =====
      pdf.addImage(logo, 'PNG', pageWidth - 50, 10, 40, 40);

      // ===== TITULOS =====
      pdf.setFontSize(16);
      pdf.text('Information Security Management System (ISMS)', 14, 18);

      pdf.setFontSize(11);
      pdf.text(`KPI Report - ${this.currentMonthLabel}`, 14, 27);
      pdf.text(`Total KPIs: ${this.totalKpis}`, 14, 34);
      pdf.text(`Compliance Score: ${this.complianceScore}%`, 14, 41);

      // ===== TABLA =====
      autoTable(pdf, {
        startY: 53,
        head: [[
          'Category',
          'KPI',
          'Responsible',
          'Value',
          'Target Value',
          'Comments'
        ]],
        body: rows,
        styles: {
          fontSize: 8,
          cellPadding: 2
        },
        headStyles: {
          fillColor: [225, 15, 28]
        },
        didDrawPage: (data) => {
          pdf.setFontSize(8);
          pdf.text(
            `Page ${pdf.getNumberOfPages()}`,
            data.settings.margin.left,
            pdf.internal.pageSize.height - 5
          );
        }
      });

      // ===== NOMBRE ARCHIVO =====
      const fileMonth = this.currentMonthLabel.replace(/\s+/g, '-');
      pdf.save(`KPI-${fileMonth}.pdf`);
    };
  }

  async deleteCurrentMonthKpis(): Promise<void> {
    const confirmed = window.confirm(
      `Delete all KPIs for ${this.currentMonthLabel}?`
    );

    if (!confirmed) return;

    try {
      const year = this.currentMonth.getFullYear();
      const month = this.currentMonth.getMonth() + 1;

      await this.api.deleteMonthKpis(year, month);

      this.loadKpis();
    } catch (error) {
      console.error('Error deleting month KPIs', error);
      window.alert('Error deleting KPIs');
    }
  }

  async reloadCurrentMonthKpis(): Promise<void> {
    try {
      const year = this.currentMonth.getFullYear();
      const month = this.currentMonth.getMonth() + 1;

      await this.api.generateKpisForMonth(year, month);

      this.loadKpis();
    } catch (error) {
      console.error('Error reloading month KPIs', error);
      window.alert('Error reloading month KPIs');
    }
  }

}
