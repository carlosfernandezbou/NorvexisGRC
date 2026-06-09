import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';

interface ThreatCatalogItem {
  threat: string;
  impacts?: string[];
}

@Component({
  selector: 'app-risk-threats-catalog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './risk-threats-catalog.html',
  styleUrl: './risk-threats-catalog.css'
})
export class RiskThreatsCatalogComponent implements OnInit {
  private readonly cdr = inject(ChangeDetectorRef);

  threatsCatalog: ThreatCatalogItem[] = [];

  async ngOnInit(): Promise<void> {
    await this.loadThreatsCatalog();
  }

  private async loadThreatsCatalog(): Promise<void> {
    try {
      const response = await fetch('/data/RiskManagement/threatCatalog.json');
      const json = await response.json();
      this.threatsCatalog = json.threatsCatalog || [];
    } catch (error) {
      console.error('Error loading threats catalog', error);
      this.threatsCatalog = [];
    } finally {
      this.cdr.detectChanges();
    }
  }

  getImpactText(impacts?: string[] | null): string {
    if (!impacts || impacts.length === 0) {
      return '-';
    }

    return impacts.join(', ');
  }
}
