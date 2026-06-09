import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject, ElementRef, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ServicesService } from '../../services/services';

import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-soa',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './soa.html',
  styleUrl: './soa.css'
})
export class SoaComponent implements OnInit {
  private api = inject(ServicesService);
  private cdr = inject(ChangeDetectorRef);

  data: any[] = [];
  issuesMenuOpen = false;

  formOpen = false;
  isEditMode = false;
  editingSoaId = '';
  saving = false;
  editingItemKey = '';

  formModel = {
    Control: '',
    Section: '',
    Applicable: 'true',
    Implemented: 'false',
    Tittle: '',
    Objective: '',
    Justification: '',
    Evidence: ''
  };

  get isAdmin(): boolean {
    return localStorage.getItem('userRole') === 'Admin';
  }

  ngOnInit(): void {
    this.loadSoas();
  }

  // Evita títulos vacíos
  private pickText(...values: any[]): string {
    return values.find(value => typeof value === 'string' && value.trim() !== '')?.trim() ?? '';
  }

  // ORDEN CORRECTO DE CONTROLES
  private compareControls(a: string, b: string): number {
    const aParts = String(a).split('.').map(Number);
    const bParts = String(b).split('.').map(Number);

    const maxLength = Math.max(aParts.length, bParts.length);

    for (let i = 0; i < maxLength; i++) {
      const aValue = aParts[i] ?? 0;
      const bValue = bParts[i] ?? 0;

      if (aValue !== bValue) {
        return aValue - bValue;
      }
    }

    return 0;
  }


  loadSoas(): void {
    this.api.getSoas().subscribe({
      next: (response) => {
        this.data = (response || [])
          .map((item: any) => ({
            Id: item.Id ?? item.id ?? item.documentId ?? item.DocumentId ?? '',
            Control: item.Control ?? item.control ?? '',
            Section: item.Section ?? item.section ?? '',
            Applicable: String(item.Applicable ?? item.applicable ?? false),
            Implemented: String(item.Implemented ?? item.implemented ?? false),
            Tittle: this.pickText(item.title, item.Title, item.tittle, item.Tittle),
            Objective: item.Objective ?? item.objective ?? '',
            Justification: item.Justification ?? item.justification ?? '',
            Evidence: item.Evidence ?? item.evidence ?? ''
          }))
          .sort((a: any, b: any) => this.compareControls(a.Control, b.Control));

        this.cdr.detectChanges();
      },
      error: (err) => console.error('SOA LOAD ERROR:', err)
    });
  }
  openCreateForm(): void {
    this.isEditMode = false;
    this.editingSoaId = '';
    this.editingItemKey = '';
    this.formOpen = true;

    this.formModel = {
      Control: '',
      Section: '',
      Applicable: 'true',
      Implemented: 'false',
      Tittle: '',
      Objective: '',
      Justification: '',
      Evidence: ''
    };
  }

  openEditForm(item: any): void {
    this.isEditMode = true;
    this.formOpen = true;
    this.editingSoaId = String(item.Id ?? item.id ?? item.Control);
    this.editingItemKey = this.getItemKey(item);

    this.formModel = {
      Control: item.Control,
      Section: item.Section,
      Applicable: item.Applicable,
      Implemented: item.Implemented,
      Tittle: item.Tittle,
      Objective: item.Objective,
      Justification: item.Justification,
      Evidence: item.Evidence
    };
  }

  cancelForm(): void {
    this.formOpen = false;
    this.isEditMode = false;
    this.editingSoaId = '';
    this.editingItemKey = '';
  }

  saveSoa(): void {
    if (this.saving) return;

    if (!this.formModel.Control || !this.formModel.Tittle) {
      alert('Control and title are required.');
      return;
    }

    this.saving = true;

    const payload = {
      Control: this.formModel.Control,
      Section: this.formModel.Section,
      Applicable: this.formModel.Applicable === 'true',
      Implemented: this.formModel.Implemented === 'true',
      Title: this.formModel.Tittle,
      Tittle: this.formModel.Tittle,
      Objective: this.formModel.Objective,
      Justification: this.formModel.Justification,
      Evidence: this.formModel.Evidence
    };

    const request = this.isEditMode
      ? this.api.updateSoa(this.editingSoaId, payload)
      : this.api.createSoa(payload);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.cancelForm();
        this.loadSoas();
      },
      error: (err) => {
        this.saving = false;
        console.error('SOA SAVE ERROR:', err);
        alert('Error saving SOA.');
      }
    });
  }

  deleteSoa(item: any): void {
    const id = String(item.Id ?? item.Control);

    if (!confirm(`Delete control ${item.Control}?`)) return;

    this.api.deleteSoa(id).subscribe({
      next: () => {
        this.loadSoas();
      },
      error: (err) => {
        console.error('SOA DELETE ERROR:', err);
        alert('Error deleting SOA.');
      }
    });
  }

  getItemKey(item: any): string {
    return String(item.Id ?? item.id ?? item.Control ?? '');
  }

  isEditingItem(item: any): boolean {
    return this.formOpen && this.isEditMode && this.editingItemKey === this.getItemKey(item);
  }

  private getLeadingNumberParts(value: string): number[] {
    const match = String(value ?? '').trim().match(/^\d+(?:\.\d+)*/);

    if (!match) {
      return [Number.MAX_SAFE_INTEGER];
    }

    return match[0].split('.').map(part => Number(part));
  }

  private compareNumericString(a: string, b: string): number {
    const aParts = this.getLeadingNumberParts(a);
    const bParts = this.getLeadingNumberParts(b);
    const maxLength = Math.max(aParts.length, bParts.length);

    for (let i = 0; i < maxLength; i++) {
      const aPart = aParts[i] ?? 0;
      const bPart = bParts[i] ?? 0;

      if (aPart !== bPart) {
        return aPart - bPart;
      }
    }

    return String(a ?? '').localeCompare(String(b ?? ''), undefined, {
      numeric: true,
      sensitivity: 'base'
    });
  }

  searchControl(control: string): void {
    if (!control) return;

    const element = document.getElementById('control-' + control);

    if (element) {
      element.scrollIntoView({
        behavior: 'smooth',
        block: 'center'
      });
    }
  }

  get notApplicableCount(): number {
    return this.data.filter(item => item.Applicable !== 'true').length;
  }

  get notImplementedCount(): number {
    return this.data.filter(item => item.Implemented !== 'true').length;
  }

  get controlsToFix() {
    return this.data.filter(
      item => item.Applicable !== 'true' || item.Implemented !== 'true'
    );
  }

  toggleIssuesMenu(): void {
    this.issuesMenuOpen = !this.issuesMenuOpen;
  }

  goToControl(controlId: string): void {
    this.issuesMenuOpen = false;

    const element = document.getElementById(`control-${controlId}`);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  exportToPDF(): void {

    const pdf = new jsPDF();

    const rows = this.data.map(item => [
      item.Control,
      item.Section,
      item.Tittle,
      item.Applicable === 'true' ? 'Yes' : 'No',
      item.Implemented === 'true' ? 'Yes' : 'No',
      item.Objective || '',
      item.Justification || '',
      item.Evidence || ''
    ]);

    const logo = new Image();
    logo.src = '/logo.png';

    logo.onload = () => {

      const pageWidth = pdf.internal.pageSize.getWidth();

      // ===== LOGO =====
      pdf.addImage(logo, 'PNG', pageWidth - 53, 10, 40, 40);

      // ===== TITULO =====
      pdf.setFontSize(16);
      pdf.text('Statement of Applicability (SOA)', 14, 24);

      pdf.setFontSize(11);
      pdf.text(`Total Controls: ${this.data.length}`, 14, 31);

      // ===== TABLA =====
      autoTable(pdf, {
        startY: 53,
        head: [[
          'Control',
          'Section',
          'Title',
          'Applicable',
          'Implemented',
          'Objective',
          'Justification',
          'Evidence'
        ]],
        body: rows,
        styles: {
          fontSize: 7,
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

      pdf.save('SOA.pdf');
    };
  }

  // CLOSE MENU Controls to fix
  constructor(private eRef: ElementRef) { }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    const target = event.target as HTMLElement;

    const clickedInsideMenu = target.closest('.issues-menu-wrapper');

    if (!clickedInsideMenu) {
      this.issuesMenuOpen = false;
    }
  }

}