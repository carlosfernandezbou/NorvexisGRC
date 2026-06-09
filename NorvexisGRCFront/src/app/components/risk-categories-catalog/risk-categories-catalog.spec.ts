import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskCategoriesCatalog } from './risk-categories-catalog';

describe('RiskCategoriesCatalog', () => {
  let component: RiskCategoriesCatalog;
  let fixture: ComponentFixture<RiskCategoriesCatalog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiskCategoriesCatalog],
    }).compileComponents();

    fixture = TestBed.createComponent(RiskCategoriesCatalog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
