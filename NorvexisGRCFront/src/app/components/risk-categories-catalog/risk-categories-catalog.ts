import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';

@Component({
  selector: 'app-risk-categories-catalog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './risk-categories-catalog.html',
  styleUrl: './risk-categories-catalog.css'
})
export class RiskCategoriesCatalogComponent implements OnInit {
  private readonly cdr = inject(ChangeDetectorRef);

  riskCategories: any = null;
  loading = false;
  error = '';

  async ngOnInit(): Promise<void> {
    await this.loadRiskCategories();
  }

  private async loadRiskCategories(): Promise<void> {
    if (this.riskCategories) {
      return;
    }

    this.loading = true;
    this.error = '';

    try {
      const response = await fetch('/data/RiskManagement/RiskCategoriesCatalog.json');

      if (!response.ok) {
        throw new Error(`Risk categories request failed with status ${response.status}`);
      }

      this.riskCategories = await response.json();
    } catch (error) {
      console.error('Error loading risk categories catalog', error);
      this.error = 'Could not load the risk categories catalog.';
    } finally {
      this.loading = false;
      this.cdr.detectChanges();
    }
  }
}
