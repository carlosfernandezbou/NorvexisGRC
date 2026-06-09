import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';

@Component({
  selector: 'app-risk-other-catalogs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './risk-other-catalogs.html',
  styleUrl: './risk-other-catalogs.css'
})
export class RiskOtherCatalogsComponent implements OnInit {
  private readonly cdr = inject(ChangeDetectorRef);

  otherCatalogs: any = null;

  async ngOnInit(): Promise<void> {
    await this.loadOtherCatalogs();
  }

  private async loadOtherCatalogs(): Promise<void> {
    try {
      const response = await fetch('/data/RiskManagement/otherCatalogs.json');
      this.otherCatalogs = await response.json();
    } catch (error) {
      console.error('Error loading other catalogs', error);
    } finally {
      this.cdr.detectChanges();
    }
  }
}
