import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RiskCategoriesCatalogComponent } from '../../components/risk-categories-catalog/risk-categories-catalog';
import { RiskInstructionsComponent } from '../../components/risk-instructions/risk-instructions';
import { RiskOtherCatalogsComponent } from '../../components/risk-other-catalogs/risk-other-catalogs';
import { RiskRegisterComponent } from '../../components/risk-register/risk-register';
import { RiskThreatsCatalogComponent } from '../../components/risk-threats-catalog/risk-threats-catalog';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-risk-management',
  standalone: true,
  imports: [
    CommonModule,
    RiskInstructionsComponent,
    RiskRegisterComponent,
    RiskThreatsCatalogComponent,
    RiskCategoriesCatalogComponent,
    RiskOtherCatalogsComponent
  ],
  templateUrl: './risk-management.html',
  styleUrl: './risk-management.css'
})
export class RiskManagementComponent {
  readonly sections = [
    'Instructions',
    'Risk register',
    'Threats catalog',
    'Risk categorization',
    'Other catalogs'
  ];

  menuOpen = false;
  loading = false;
  selectedSection = 'Instructions';

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const section = params['section'];

      if (section) {
        this.selectedSection = section;
      }
    });
  }
  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  selectSection(section: string): void {
    this.selectedSection = section;
    this.menuOpen = false;
  }
}
